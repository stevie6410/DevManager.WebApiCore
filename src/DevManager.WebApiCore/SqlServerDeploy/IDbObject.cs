namespace DevManager.WebApiCore.SqlServerDeploy
{
    public interface IDbObject
    { 
        string DatabaseName { get; set; }
        string SchemaName { get; set; }
        string ObjectName { get; set; }
        string DeployType { get; set; }
        string ObjectType { get; set; }
        int DeployPackageDBObjectId { get; set; }
        string Script { get; set; }
        int DeployOrder { get; set; }
        string TwoPartName { get; }

    }
}