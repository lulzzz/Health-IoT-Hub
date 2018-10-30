using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using LISWebAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RCL.LISConnector.DataEntity.IOT;
using RCL.LISConnector.DataEntity.SQL;

namespace LISWebAPI.Controllers.datastorev1
{
    [Authorize]
    [ApiExplorerSettings(GroupName = "datastore")]
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class PatientDiagnosticRecordsController : ControllerBase
    {
        private readonly DatabaseDBContext _context;

        public PatientDiagnosticRecordsController(DatabaseDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Post patient diagnostic record to save in a database
        /// </summary>
        /// <param name="patientDiagnosticRecords">A list of patient diagnostic record</param>
        /// <response code="200">Returns a list of Patient Diagnostic Record</response>
        /// <response code="400">If bad request</response>
        // POST: api/PatientDiagnosticRecords
        [HttpPost]
        [ProducesResponseType(typeof(IEnumerable<PatientDiagnosticRecord>), 200)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostPatientDiagnosticRecords([FromBody] List<PatientDiagnosticRecord> patientDiagnosticRecords)
        {
            
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            List<PatientDiagnosticRecord> records = new List<PatientDiagnosticRecord>();

            try
            {   
                if(patientDiagnosticRecords?.Count > 0)
                {
                    foreach(var record in patientDiagnosticRecords)
                    {
                        PatientDiagnosticRecord _record = new PatientDiagnosticRecord();

                        Patient patient = record?.patient;
                        if(!string.IsNullOrEmpty(patient?.InternalPatientId))
                        {
                            int patientId = await GetPatientIdAsync(patient);
                            if(patientId > 0)
                            {
                                _record.patient = await _context.Patients.FindAsync(patientId);

                                DiagnosticReport diagnosticReport = record?.diagnosticReport;
                                if(diagnosticReport != null)
                                {
                                    diagnosticReport.PatientId = patientId;
                                    _context.DiagnosticReports.Add(diagnosticReport);
                                    _context.SaveChanges();
                                    int diagnosticReportId = diagnosticReport.Id;
                                    if(diagnosticReportId > 0)
                                    {
                                        _record.diagnosticReport = await _context.DiagnosticReports.FindAsync(diagnosticReportId);

                                        List<Result> results = record?.results;
                                        if(results?.Count > 0)
                                        {
                                            List<Result> _results = new List<Result>();

                                            foreach(var result in results)
                                            {
                                                result.DiagnosticReportId = diagnosticReportId;
                                                _context.Results.Add(result);
                                                _context.SaveChanges();

                                                Result newResult = await _context.Results.FindAsync(result.Id);
                                                _results.Add(newResult);
                                            }

                                            _record.results = _results;
                                        }
                                        else
                                        {
                                            return BadRequest();
                                        }
                                    }
                                    else
                                    {
                                        return BadRequest();
                                    }

                                }
                                else
                                {
                                    return BadRequest();
                                }
                            }
                            else
                            {
                                return BadRequest();
                            }
                        }
                        else
                        {
                            return BadRequest();
                        }

                        records.Add(_record);
                    }
                }
                else
                {
                    return BadRequest();
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return BadRequest();
            }

            return Ok(records);
        }

        private async Task<int> GetPatientIdAsync(Patient patient)
        {
            int r = 0;

            try
            {
                Patient existingPatient = await _context.Patients
                    .Where(w => w.InternalPatientId == patient.InternalPatientId)
                    .FirstOrDefaultAsync();
                if(existingPatient == null)
                {
                    if(IsPatientValid(patient))
                    {
                        string fname = patient.GivenName.ToLower();
                        string mname = patient?.MiddleName?.ToLower() ?? "-";
                        string lname = patient.FamilyName.ToLower();
                        string dob = patient.DateOfBirth.Value.ToString("ddMMyyyy");
                        string discriminator = $"{fname}{mname}{lname}{dob}";
                        patient.Discriminator = discriminator;

                        _context.Patients.Add(patient);
                        await _context.SaveChangesAsync();
                        r = patient.Id;
                    }
                    else
                    {
                        r = 0;
                    }
                }
                else
                {
                    r = existingPatient.Id;
                }
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                r = 0;
            }

            return r;
        }

        private bool IsPatientValid(Patient patient)
        {
            bool b = true;

            try
            {
                if (string.IsNullOrEmpty(patient?.InternalPatientId))
                    return false;
                if (string.IsNullOrEmpty(patient?.FamilyName))
                    return false;
                if (string.IsNullOrEmpty(patient?.GivenName))
                    return false;
                if (patient?.DateOfBirth == null)
                    return false;
            }
            catch(Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }

            return b;
        }
    }
}
