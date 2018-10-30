using Newtonsoft.Json;
using RCL.LISConnector.DataEntity.SQL;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LISWebAPI.Data.ogtt
{
    [Table(name: "lisc_ogttReport")]
    public class OgttReport
    {
        [Key]
        public int Id { get; set; }

        [DataType(DataType.Date)]
        public DateTime ReportDate { get; set; }

        [Required]
        public int PatientId { get; set; }

        [ForeignKey("PatientId")]
        public Patient Patient { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(50)]
        public string GlucoseAmount { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(50)]
        public string Doctor { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(50)]
        public string ClientId { get; set; }

        public ICollection<OgttResult> OgttResults { get; set; }
    }

    [Table(name: "lisc_ogttReportResult")]
    public class OgttResult
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int ResultId { get; set; }

        [ForeignKey("ResultId")]
        public Result Result { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(50)]
        public string ResultTitle { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(150)]
        public string NormalRange { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(50)]
        public string UrineGlucose { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(50)]
        public string UrineProtein { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(50)]
        public string UrineKetone { get; set; }

        [DataType(DataType.Text)]
        [MaxLength(50)]
        public string ClientId { get; set; }

        [Required]
        public int OgttReportId { get; set; }

        [JsonIgnore]
        [ForeignKey("OgttReportId")]
        public OgttReport OgttReport { get; set; }
    }
}
