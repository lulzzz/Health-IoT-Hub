using LISWebAPI.Data.ogtt;
using Microsoft.EntityFrameworkCore;

namespace LISWebAPI.Data
{
    public partial class DatabaseDBContext
    {
        public DbSet<OgttReport> OgttReports { get; set; }
        public DbSet<OgttResult> OgttResults { get; set; }
    }
}
