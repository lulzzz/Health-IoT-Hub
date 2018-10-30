using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using LISWebAPI.Data;
using Microsoft.AspNetCore.Authorization;
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
    public class ResultsController : ControllerBase
    {
        private readonly DatabaseDBContext _context;

        public ResultsController(DatabaseDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get results by diagnostic report id
        /// </summary>
        /// <param name="diagnosticReportId">The diagnostic report id</param>
        /// <response code="200">Returns a list of results in a diagnostic report</response>
        // GET: api/Results/diagnosticreportid/5
        [HttpGet("diagnosticreportid/{diagnosticReportId}")]
        [ProducesResponseType(typeof(IEnumerable<Result>), 200)]
        public async Task<IEnumerable<Result>> GetResultsByDiagnosticReportId(int diagnosticReportId)
        {
            return await _context.Results.Where(w => w.DiagnosticReportId == diagnosticReportId).ToListAsync();
        }


        /// <summary>
        /// Get a result by id
        /// </summary>
        /// <param name="id">The result id</param>
        /// <response code="200">Returns a result</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // GET: api/Results/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Result), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetResult([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _context.Results.FindAsync(id);

            if (result == null)
            {
                return NotFound();
            }

            return Ok(result);
        }

        /// <summary>
        /// Add a new result
        /// </summary>
        /// <param name="result">The result entity</param>
        /// <response code="201">Returns the result entity that was created</response>
        /// <response code="400">If bad request</response>
        // POST: api/Results
        [HttpPost]
        [ProducesResponseType(typeof(Result), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostResult([FromBody] Result result)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Results.Add(result);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetResult", new { id = result.Id }, result);
        }

        /// <summary>
        /// Edit a result
        /// </summary>
        /// <param name="id">The result id</param>
        /// <param name="result">The result entity</param>
        /// <response code="204">Returns no content when updated</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // PUT: api/Results/5
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> Result([FromRoute] int id, [FromBody] Result result)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != result.Id)
            {
                return BadRequest();
            }

            _context.Entry(result).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResultExists(id))
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
        /// Delete a result
        /// </summary>
        /// <param name="id">The result id</param>
        /// <response code="200">Returns the result that was deleted</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // DELETE: api/Results/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Result), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteResult([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var result = await _context.Results.FindAsync(id);
            if (result == null)
            {
                return NotFound();
            }

            _context.Results.Remove(result);
            await _context.SaveChangesAsync();

            return Ok(result);
        }

        private bool ResultExists(int id)
        {
            return _context.Results.Any(e => e.Id == id);
        }
    }
}
