using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace DevManager.WebApiCore.SqlServerDeploy
{
    public class SqlDeploy
    {
        private readonly SqlConnectionConfig _config;
        private readonly IEnumerable<IDbObject> _deployObjects;
        private SqlConnection _transconn;
        private SqlConnection _conn;
        private SqlTransaction _trans;

        public List<string> EventLog = new List<string>();

        public SqlDeploy(SqlConnectionConfig deployConfig, IEnumerable<IDbObject> deployObjects)
        {
            _config = deployConfig;
            _deployObjects = deployObjects;
            _transconn = new SqlConnection(_config.ConnectionString);
            _conn = new SqlConnection(_config.ConnectionString);
        }

        public void DeployToTarget()
        {
            Log(string.Format("Connecting to {0}.{1}", _config.TargetServer, _config.TargetDatabase));
            try
            {
                _transconn.Open();
                _trans = _transconn.BeginTransaction();
                _conn.Open();
            }
            catch (Exception ex)
            {
                Log(ex.Message);
                throw ex;
            }

            Log(string.Format("Connection Status: {0}", _transconn.State.ToString()));

            try
            {
                foreach (var obj in _deployObjects.OrderByDescending(o => o.DeployOrder))
                {

                    Log("==============================");

                    Log(string.Format("Picked up {0} {1}.{2}.{3}", obj.ObjectType, obj.DatabaseName, obj.SchemaName, obj.ObjectName));

                    Log(string.Format("Attempting to {0}", obj.DeployType));


                    HandleDeploy(obj);
                }
                _trans.Commit();
            }
            catch (Exception ex)
            {
                _trans.Rollback();
                //Handle the error
                Log("Rolling back changes due to deployment error");
                Log(string.Format("ERROR: {0}", ex.Message));
                throw new Exception("Deploy Failed");
            }

        }

        private void HandleDeploy(IDbObject obj)
        {
            //Sort into each deploy type, CREATE, UPDATE, DROP            
            switch (obj.DeployType)
            {
                case "CREATE":
                    HandleCreate(obj);
                    break;
                case "ALTER":
                    HandleAlter(obj);
                    break;
                case "DROP":
                    HandleDrop(obj);
                    break;
                default:
                    Log(string.Format("Deploy Type {0} is not spported", obj.DeployType));
                    break;
            }
        }

        private void HandleCreate(IDbObject obj)
        {
            //Check to see if the object already exists
            //If so then change the create to an alter
            if (DBObjExists(obj))
            {
                Log("Change the CREATE to ALTER");
                Log("Attempting to ALTER");
                ExecScript(obj.Script.ToAlterScript(obj.ObjectType));
            }
            else
            {
                ExecScript(obj.Script);
            }
        }

        private void HandleAlter(IDbObject obj)
        {
            //Check to see if the object already exists
            //If not then change the alter to create
            if (DBObjExists(obj))
            {
                ExecScript(obj.Script);
            }
            else
            {
                Log("Change the ALTER to CREATE");
                Log("Attempting to CREATE");
                ExecScript(obj.Script.ToCreateScript(obj.ObjectType));
            }
        }

        private void HandleDrop(IDbObject obj)
        {
            //Check to see if the object already exists
            //If not then change the alter to create
            if (DBObjExists(obj))
            {
                ExecScript(obj.Script);
            }
            else
            {
                Log("Skipping as object does not exist");
            }
        }

        private bool DBObjExists(IDbObject obj)
        {
            var cmdTxt = String.Format(@"SELECT COUNT(*) FROM dbo.sysobjects WHERE id = object_id(N'[{0}].[{1}]')", obj.SchemaName, obj.ObjectName);
            var cmd = new SqlCommand(cmdTxt, _conn);
            Int32 rowsAffected = (Int32)cmd.ExecuteScalar();
            var result = !(rowsAffected == 0);
            if (result)
                Log("Already exists");
            else
                Log("Does not exist");
            return result;
        }

        private void ExecScript(string script)
        {
            try
            {
                SqlCommand cmd = new SqlCommand(script, _transconn);
                //Log(cmd.CommandText);
                cmd.Transaction = _trans;
                cmd.ExecuteNonQuery();
                Log("Successfuly Deployed!");
            }
            catch (Exception ex)
            {
                //Log(ex.Message);
                Debug.WriteLine(ex.Message);
                throw;
            }
        }

        private void Log(string msg)
        {
            Debug.WriteLine("");
            Debug.WriteLine(msg);
            this.EventLog.Add(msg);
        }
    }
}