using System;

namespace DevManager.WebApiCore.SqlServerDeploy
{
    public class SqlObjectDependency : IDbObjectDependency
    {
        public Int32 DependencyLevel { get; set; }
        public string Database { get; set; }
        public string Schema { get; set; }
        public string ObjectName { get; set; }
        public Int32 ObjectId { get; set; }
        public string ObjectType { get; set; }
        public string ObjectTypeDescription { get; set; }
    }
}