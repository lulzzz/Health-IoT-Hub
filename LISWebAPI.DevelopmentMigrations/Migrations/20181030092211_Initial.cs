using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LISWebAPI.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "lisc_patient",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ExternalPatientId = table.Column<string>(maxLength: 50, nullable: true),
                    InternalPatientId = table.Column<string>(maxLength: 50, nullable: true),
                    AlternatePatientId = table.Column<string>(maxLength: 50, nullable: true),
                    Active = table.Column<bool>(nullable: false),
                    FamilyName = table.Column<string>(maxLength: 50, nullable: true),
                    GivenName = table.Column<string>(maxLength: 50, nullable: true),
                    MiddleName = table.Column<string>(maxLength: 50, nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: true),
                    Sex = table.Column<string>(maxLength: 50, nullable: true),
                    Race = table.Column<string>(maxLength: 50, nullable: true),
                    Address = table.Column<string>(maxLength: 350, nullable: true),
                    CountryCode = table.Column<string>(maxLength: 50, nullable: true),
                    PhoneNumberHome = table.Column<string>(maxLength: 50, nullable: true),
                    PhoneNumberBusiness = table.Column<string>(maxLength: 50, nullable: true),
                    PrimaryLanguage = table.Column<string>(maxLength: 50, nullable: true),
                    MaritalStatus = table.Column<string>(maxLength: 50, nullable: true),
                    Religion = table.Column<string>(maxLength: 50, nullable: true),
                    AccountNumber = table.Column<string>(maxLength: 50, nullable: true),
                    SSNNumber = table.Column<string>(maxLength: 50, nullable: true),
                    DriversLicenseNumber = table.Column<string>(maxLength: 50, nullable: true),
                    Citizenship = table.Column<string>(maxLength: 50, nullable: true),
                    NationalityCode = table.Column<string>(maxLength: 50, nullable: true),
                    Discriminator = table.Column<string>(maxLength: 100, nullable: true),
                    ClientId = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lisc_patient", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lisc_testCode",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Code = table.Column<string>(maxLength: 50, nullable: true),
                    TestTitle = table.Column<string>(maxLength: 50, nullable: true),
                    ClientId = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lisc_testCode", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "lisc_diagnosticReport",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PatientInternalId = table.Column<string>(maxLength: 50, nullable: true),
                    VisitId = table.Column<string>(maxLength: 50, nullable: true),
                    FamilyName = table.Column<string>(maxLength: 50, nullable: true),
                    GivenName = table.Column<string>(maxLength: 50, nullable: true),
                    Sex = table.Column<string>(maxLength: 50, nullable: true),
                    DateOfBirth = table.Column<DateTime>(nullable: true),
                    TestCodes = table.Column<string>(maxLength: 250, nullable: true),
                    AnalyzerName = table.Column<string>(maxLength: 50, nullable: true),
                    AnalyzerDateTime = table.Column<DateTime>(nullable: true),
                    OperatorId = table.Column<string>(maxLength: 50, nullable: true),
                    SendingApplication = table.Column<string>(maxLength: 50, nullable: true),
                    SendingFacility = table.Column<string>(maxLength: 50, nullable: true),
                    ReceivingApplication = table.Column<string>(maxLength: 50, nullable: true),
                    ReceivingFacility = table.Column<string>(maxLength: 50, nullable: true),
                    ClientId = table.Column<string>(maxLength: 50, nullable: true),
                    PatientId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lisc_diagnosticReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lisc_diagnosticReport_lisc_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "lisc_patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lisc_ogttReport",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ReportDate = table.Column<DateTime>(nullable: false),
                    PatientId = table.Column<int>(nullable: false),
                    GlucoseAmount = table.Column<string>(maxLength: 50, nullable: true),
                    Doctor = table.Column<string>(maxLength: 50, nullable: true),
                    ClientId = table.Column<string>(maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lisc_ogttReport", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lisc_ogttReport_lisc_patient_PatientId",
                        column: x => x.PatientId,
                        principalTable: "lisc_patient",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lisc_result",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TestCode = table.Column<string>(maxLength: 50, nullable: true),
                    Value = table.Column<string>(maxLength: 50, nullable: true),
                    Units = table.Column<string>(maxLength: 50, nullable: true),
                    ReferenceRange = table.Column<string>(maxLength: 50, nullable: true),
                    Comments = table.Column<string>(maxLength: 250, nullable: true),
                    AbnormalFlags = table.Column<string>(maxLength: 50, nullable: true),
                    ResultDateTime = table.Column<DateTime>(nullable: true),
                    ClientId = table.Column<string>(maxLength: 50, nullable: true),
                    DiagnosticReportId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lisc_result", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lisc_result_lisc_diagnosticReport_DiagnosticReportId",
                        column: x => x.DiagnosticReportId,
                        principalTable: "lisc_diagnosticReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "lisc_ogttReportResult",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ResultId = table.Column<int>(nullable: false),
                    ResultTitle = table.Column<string>(maxLength: 50, nullable: true),
                    NormalRange = table.Column<string>(maxLength: 150, nullable: true),
                    UrineGlucose = table.Column<string>(maxLength: 50, nullable: true),
                    UrineProtein = table.Column<string>(maxLength: 50, nullable: true),
                    UrineKetone = table.Column<string>(maxLength: 50, nullable: true),
                    ClientId = table.Column<string>(maxLength: 50, nullable: true),
                    OgttReportId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_lisc_ogttReportResult", x => x.Id);
                    table.ForeignKey(
                        name: "FK_lisc_ogttReportResult_lisc_ogttReport_OgttReportId",
                        column: x => x.OgttReportId,
                        principalTable: "lisc_ogttReport",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_lisc_ogttReportResult_lisc_result_ResultId",
                        column: x => x.ResultId,
                        principalTable: "lisc_result",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_lisc_diagnosticReport_PatientId",
                table: "lisc_diagnosticReport",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_lisc_ogttReport_PatientId",
                table: "lisc_ogttReport",
                column: "PatientId");

            migrationBuilder.CreateIndex(
                name: "IX_lisc_ogttReportResult_OgttReportId",
                table: "lisc_ogttReportResult",
                column: "OgttReportId");

            migrationBuilder.CreateIndex(
                name: "IX_lisc_ogttReportResult_ResultId",
                table: "lisc_ogttReportResult",
                column: "ResultId");

            migrationBuilder.CreateIndex(
                name: "IX_lisc_result_DiagnosticReportId",
                table: "lisc_result",
                column: "DiagnosticReportId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "lisc_ogttReportResult");

            migrationBuilder.DropTable(
                name: "lisc_testCode");

            migrationBuilder.DropTable(
                name: "lisc_ogttReport");

            migrationBuilder.DropTable(
                name: "lisc_result");

            migrationBuilder.DropTable(
                name: "lisc_diagnosticReport");

            migrationBuilder.DropTable(
                name: "lisc_patient");
        }
    }
}
