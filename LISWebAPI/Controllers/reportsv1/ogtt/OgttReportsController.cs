using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using LISWebAPI.Data;
using Microsoft.AspNetCore.Authorization;
using LISWebAPI.Data.ogtt;

namespace LISWebAPI.Controllers.reportsv1.ogtt
{
    [Authorize]
    [ApiExplorerSettings(GroupName = "reports")]
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class OGTTReportsController : ControllerBase
    {
        private readonly DatabaseDBContext _context;

        public OGTTReportsController(DatabaseDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets 50 OGTT reports
        /// </summary>
        /// <response code="200">Returns a list of OGTT reports</response>
        // GET: api/OgttReports
        [HttpGet]
        public IEnumerable<OgttReport> GetOgttReports()
        {
            return _context.OgttReports.Take(50);
        }

        /// <summary>
        /// Get an OGTT report by id
        /// </summary>
        /// <param name="id">The OGTT report id</param>
        /// <response code="200">Returns an OGTT report</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // GET: api/OgttReports/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOgttReport([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ogttReport = await _context.OgttReports.FindAsync(id);

            if (ogttReport == null)
            {
                return NotFound();
            }

            return Ok(ogttReport);
        }

        /// <summary>
        /// Edit an OGTT report
        /// </summary>
        /// <param name="id">The OGTT report id</param>
        /// <param name="ogttReport">The OGTT report entity</param>
        /// <response code="204">Returns no content when updated</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // PUT: api/OgttReports/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOgttReport([FromRoute] int id, [FromBody] OgttReport ogttReport)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ogttReport.Id)
            {
                return BadRequest();
            }

            _context.Entry(ogttReport).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OgttReportExists(id))
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
        /// Create a new OGTT report
        /// </summary>
        /// <param name="ogttReport">The OGTT report entity</param>
        /// <response code="201">Returns the OGTT report entity that was created</response>
        /// <response code="400">If bad request</response>
        // POST: api/OgttReports
        [HttpPost]
        public async Task<IActionResult> PostOgttReport([FromBody] OgttReport ogttReport)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.OgttReports.Add(ogttReport);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOgttReport", new { id = ogttReport.Id }, ogttReport);
        }

        /// <summary>
        /// Delete an OGTT report
        /// </summary>
        /// <param name="id">The OGTT report id</param>
        /// <response code="200">Returns the OGTT report that was deleted</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // DELETE: api/OgttReports/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOgttReport([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ogttReport = await _context.OgttReports.FindAsync(id);
            if (ogttReport == null)
            {
                return NotFound();
            }

            List<OgttResult> ogttResults = await _context.OgttResults.Where(w => w.OgttReportId == id).ToListAsync();
            if(ogttResults?.Count > 0)
            {
                foreach(OgttResult ogttResult in ogttResults)
                {
                    _context.Remove(ogttResult);
                }
            }
            _context.OgttReports.Remove(ogttReport);
            await _context.SaveChangesAsync();

            return Ok(ogttReport);
        }

        private bool OgttReportExists(int id)
        {
            return _context.OgttReports.Any(e => e.Id == id);
        }
    }
}