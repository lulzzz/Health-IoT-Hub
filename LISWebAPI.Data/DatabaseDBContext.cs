using Microsoft.EntityFrameworkCore;
using RCL.LISConnector.DataEntity.SQL;

namespace LISWebAPI.Data
{
    public partial class DatabaseDBContext : DbContext
    {
        public DatabaseDBContext(DbContextOptions<DatabaseDBContext> options)
             : base(options)
        {}

        public DbSet<Patient> Patients { get; set; }
        public DbSet<DiagnosticReport> DiagnosticReports { get; set; }
        public DbSet<Result> Results { get; set; }
        public DbSet<TestCode> TestCodes { get; set; }
    }
}
