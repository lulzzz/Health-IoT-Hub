using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using LISWebAPI.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RCL.LISConnector.DataEntity.SQL;

namespace LISWebAPI.Controllers.apiv1
{
    [Authorize]
    [ApiExplorerSettings(GroupName = "datastore")]
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class DiagnosticReportsController : ControllerBase
    {
        private readonly DatabaseDBContext _context;

        public DiagnosticReportsController(DatabaseDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get a diagnostic report by id
        /// </summary>
        /// <param name="id">The diagnostic report id</param>
        /// <response code="200">Returns a diagnostic report</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // GET: api/DiagnosticReports/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(DiagnosticReport), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetDiagnosticReport([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var diagnosticReport = await _context.DiagnosticReports.FindAsync(id);

            if (diagnosticReport == null)
            {
                return NotFound();
            }

            return Ok(diagnosticReport);
        }

        /// <summary>
        /// Get diagnostic reports by patient id
        /// </summary>
        /// <param name="patientId">The patient id</param>
        /// <response code="200">Returns a list of diagnostic reports for a patient</response>
        // GET: api/DiagnosticReports
        [HttpGet("patientid/{patientId}")]
        [ProducesResponseType(typeof(IEnumerable<DiagnosticReport>), 200)]
        public async Task<IEnumerable<DiagnosticReport>> GetDiagnosticReportsByPatientId(int patientId)
        {
            return await _context.DiagnosticReports.Where(w => w.PatientId == patientId).ToListAsync();
        }

        /// <summary>
        /// Get diagnostic reports by date
        /// </summary>
        /// <param name="startDate">The start date - string("ddMMyyyy") [eg: "14021980"]</param>
        /// <param name="endDate">The end date - string("ddMMyyyy") [eg: "14021980"]</param>
        /// <response code="200">Returns a list of diagnostic reports by date</response>
        // GET: api/DiagnosticReports/startdate
        [HttpGet("startdate/{startDate}/enddate/{endDate}")]
        [ProducesResponseType(typeof(IEnumerable<DiagnosticReport>), 200)]
        public async Task<IEnumerable<DiagnosticReport>> GetDiagnosticReportsByDate(string startDate, string endDate)
        {
            DateTime sDate;
            DateTime.TryParseExact(startDate,"ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out sDate);
            DateTime eDate;
            DateTime.TryParseExact(endDate, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out eDate);

            return await _context.DiagnosticReports
                .Where(w => w.AnalyzerDateTime <= eDate.Date && w.AnalyzerDateTime.Value >= sDate.Date)
                .ToListAsync();
        }

        /// <summary>
        /// Add a new diagnostic report
        /// </summary>
        /// <param name="diagnosticReport">The diagnostic report entity</param>
        /// <response code="201">Returns the diagnostic report entity that was created</response>
        /// <response code="400">If bad request</response>
        // POST: api/DiagnosticReports
        [HttpPost]
        [ProducesResponseType(typeof(DiagnosticReport), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostDiagnosticReport([FromBody] DiagnosticReport diagnosticReport)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.DiagnosticReports.Add(diagnosticReport);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetDiagnosticReport", new { id = diagnosticReport.Id }, diagnosticReport);
        }


        /// <summary>
        /// Edit a diagnostic report
        /// </summary>
        /// <param name="id">The diagnostic report id</param>
        /// <param name="diagnosticReport">The diagnostic report entity</param>
        /// <response code="204">Returns no content when updated</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // PUT: api/DiagnosticReports/5
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DiagnosticReport([FromRoute] int id, [FromBody] DiagnosticReport diagnosticReport)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != diagnosticReport.Id)
            {
                return BadRequest();
            }

            _context.Entry(diagnosticReport).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!DiagnosticReportExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }


        /// <summary>
        /// Delete a diagnostic report
        /// </summary>
        /// <param name="id">The diagnostic report id</param>
        /// <response code="200">Returns the diagnostic report that was deleted</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // DELETE: api/DiagnosticReports/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(DiagnosticReport), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteDiagnosticReport([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var diagnosticReport = await _context.DiagnosticReports.FindAsync(id);
            if (diagnosticReport == null)
            {
                return NotFound();
            }

            List<Result> results = _context.Results.Where(w => w.DiagnosticReportId == diagnosticReport.Id).ToList();
            if(results?.Count > 0)
            {
                foreach(Result result in results)
                {
                    _context.Results.Remove(result);
                    _context.SaveChanges();
                }
            }

            _context.DiagnosticReports.Remove(diagnosticReport);
            await _context.SaveChangesAsync();

            return Ok(diagnosticReport);
        }

        private bool DiagnosticReportExists(int id)
        {
            return _context.DiagnosticReports.Any(e => e.Id == id);
        }
    }
}
