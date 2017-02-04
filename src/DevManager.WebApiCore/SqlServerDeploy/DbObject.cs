namespace DevManager.WebApiCore.SqlServerDeploy
{
    public class DbObject
    {
        public string DatabaseName { get; set; }
        public string SchemaName { get; set; }
        public string ObjectName { get; set; }
        public string DeployType { get; set; }
        public string ObjectType { get; set; }
        public int DeployPackageDBObjectId { get; set; }
        public string Script { get; set; }
        public int DeployOrder { get; set; }
        public string TwoPartName
        {
            get
            {
                return string.Format("[{0}].[{1}]", SchemaName, ObjectName);
            }
        }
    }
}