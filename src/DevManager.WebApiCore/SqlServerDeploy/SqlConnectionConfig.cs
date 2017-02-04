using System.Data.SqlClient;

namespace DevManager.WebApiCore.SqlServerDeploy
{
    public class SqlConnectionConfig
    {
        private string _targetServer { get; set; }
        private string _targetDatabase { get; set; }

        public string TargetServer { get { return _targetServer; } }
        public string TargetDatabase { get { return _targetDatabase; } }

        public SqlConnectionConfig(string targetServer, string targetDatabase)
        {
            _targetServer = targetServer;
            _targetDatabase = targetDatabase;
        }

        public string ConnectionString
        {
            get
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder();
                builder.DataSource = _targetServer;
                builder.InitialCatalog = _targetDatabase;
                builder.IntegratedSecurity = false;
                builder.UserID = "appUser";
                builder.Password = "Password2";
                return builder.ConnectionString;
            }
        }
    }
}