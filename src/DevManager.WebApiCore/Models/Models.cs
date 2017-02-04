using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;

namespace DevManager.WebApiCore.Models
{
    public class ReportSyncDbContext : DbContext
    {
        public ReportSyncDbContext(DbContextOptions<ReportSyncDbContext> options)
        : base(options)
        { }

        //List all of the entitys for report sync
        public DbSet<Package> Packages { get; set; }
    }

    public class Package
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public string TicketRef { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string ModifiedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public Workflow Workflow { get; set; }

        public List<PackageDbObject> DbObjects { get; set; }
        public List<PackageSSRSReport> SSRSReports { get; set; }
    }

    public class PackageDbObject
    {
        public int Id { get; set; }
        public Guid Guid { get; set; }
        public string ObjectKey { get; set; }
        public string DatabaseName { get; set; }
        public string SchemaName { get; set; }
        public string ObjectName { get; set; }
        public string LastEventType { get; set; }
        public string LastEventDDL { get; set; }
        public DateTime CreatedOn { get; set; }
        public string CreatedBy { get; set; }
        public DateTime ModifiedOn { get; set; }
        public string ModifiedBy { get; set; }
        public string ObjectType { get; set; }
        public string AttatchType { get; set; }
        public int DeployOrder { get; set; }
        public Package Package { get; set; }
    }

    public class PackageSSRSReport
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ReportId { get; set; }
        public string Path { get; set; }
        public int Type { get; set; }
        public Package Package { get; set; }
    }

    public class Deployments
    {
        public int Id { get; set; }
        public string Status { get; set; }
        public string CreatedBy { get; set; }
        public DateTime CreatedOn { get; set; }
        public string DeployedBy { get; set; }
        public DateTime DeployedOn { get; set; }
        public DeployEnvironment Environment { get; set; }
    }

    public class Workflow
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<WorkflowStage> Stages { get; set; }
    }

    public class WorkflowStage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Sequence { get; set; }
        public DeployEnvironment Environment { get; set; }
        public Workflow Workflow { get; set; }
        public Database Database { get; set; }
        public ReportServer ReportServer { get; set; }
    }

    public class DeployEnvironment
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cat { get; set; }
    }

    public class Database
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string DatabaseName { get; set; }
        public string ServerName { get; set; }
        public bool UseWindowsAuth { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
    }

    public class ReportServer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string ReportServerAddress { get; set; }
        public string ReportManagerAddress { get; set; }
        public DeployEnvironment Environment { get; set; }
    }
}