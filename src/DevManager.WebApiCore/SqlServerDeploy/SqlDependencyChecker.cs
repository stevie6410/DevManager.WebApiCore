using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace DevManager.WebApiCore.SqlServerDeploy
{
    public class SqlDependencyChecker
    {
        private SqlConnectionConfig _connConfig;
        private SqlConnection _conn;

        private string _dependencySQL;

        public SqlDependencyChecker(SqlConnectionConfig connectionConfig)
        {
            _connConfig = connectionConfig;
            _conn = new SqlConnection(_connConfig.ConnectionString);

            try
            {
                _conn.Open();
                Debug.WriteLine("Connected to Sql Server");
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Failed to connect to the Sql Server");
                Debug.WriteLine(ex.Message);
            }

            _dependencySQL = @"WITH cteDeps AS (
	                                SELECT	
		                                 0 AS [Level]
		                                ,SCHEMA_NAME(o.schema_id) AS [Schema]
		                                ,o.name AS [ObjectName]
		                                ,OBJECT_ID(SCHEMA_NAME(o.schema_id) + '.' + o.name) AS [ObjectId]
		                                ,o.type AS [ObjectType]
		                                ,o.type_desc AS [ObjectTypeDesc]
	                                FROM
		                                sys.objects AS o
	                                WHERE
		                                o.name = '@Object' AND o.schema_id = SCHEMA_ID('@Schema')

                                UNION ALL

	                                SELECT
	                                    PARENT.Level + 1  AS Level
	                                   ,CHILD.referenced_schema_name AS [Schema]
	                                   ,CHILD.referenced_entity_name AS [ObjectName]
	                                   ,CHILD.referenced_id AS [ObjectId]
	                                   ,COBJECT.type AS [ObjectType]
	                                   ,COBJECT.type_desc [ObjectTypeDesc]
	                                FROM
		                                cteDeps AS PARENT
	                                CROSS APPLY
		                                sys.dm_sql_referenced_entities(PARENT.[Schema] + '.' + PARENT.ObjectName, 'OBJECT') AS CHILD
	                                INNER JOIN
		                                sys.objects AS COBJECT ON (COBJECT.object_id = referenced_id) 
	                                WHERE 
		                                COBJECT.type NOT IN ('SN')
                                )

                                SELECT
	                                MAX(D.Level) DependencyLevel
                                   ,D.[Schema]
                                   ,D.ObjectName
                                   ,D.ObjectId
                                   ,D.ObjectType
                                   ,D.ObjectTypeDesc
                                FROM
	                                cteDeps AS D
                                GROUP BY
                                    D.[Schema]
                                   ,D.ObjectName
                                   ,D.ObjectId
                                   ,D.ObjectType
                                   ,D.ObjectTypeDesc
                                ORDER BY
	                                DependencyLevel DESC";
        }


        public List<IDbObjectDependency> GetDependencies(string schemaName, string objectName)
        {
            List<IDbObjectDependency> results = new List<IDbObjectDependency>();
            //string objectIdentifier = string.Format("{0}.{1}", schemaName, objectName);
            var sql = _dependencySQL.Replace("@Schema", schemaName);
            sql = sql.Replace("@Object", objectName);

            using (var cmd = new SqlCommand())
            {
                cmd.Connection = _conn;
                cmd.CommandText = sql;
                var reader = cmd.ExecuteReader();

                while (reader.Read())
                {
                    var newResult = new SqlObjectDependency();
                    newResult.DependencyLevel = reader.GetInt32(0);
                    newResult.Database = _connConfig.TargetDatabase;
                    newResult.Schema = reader.GetString(1);
                    newResult.ObjectName = reader.GetString(2);
                    newResult.ObjectId = reader.GetInt32(3);
                    newResult.ObjectType = reader.GetString(4);
                    newResult.ObjectTypeDescription = reader.GetString(5);
                    results.Add(newResult);
                }
                reader.Dispose();
            }

            return results;
        }

        public List<IDbObjectDependency> GetDependencies(IEnumerable<IDbObject> objects)
        {
            var results = new List<IDbObjectDependency>();
            foreach (var obj in objects)
            {
                var deps = GetDependencies(obj.SchemaName, obj.ObjectName);
                foreach (var d in deps)
                {
                    //Check for duplicates before adding
                    if (!results.Where(r => r.Schema == d.Schema && r.ObjectName == d.ObjectName).Any())
                    {
                        results.Add(d);
                    }
                }
            }
            return results;
        }
    }
}