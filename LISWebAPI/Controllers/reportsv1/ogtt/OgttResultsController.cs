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
    public class OGTTResultsController : ControllerBase
    {
        private readonly DatabaseDBContext _context;

        public OGTTResultsController(DatabaseDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Gets 50 OGTT results
        /// </summary>
        /// <response code="200">Returns a list of OGTT results</response>
        // GET: api/OgttResults
        [HttpGet]
        public IEnumerable<OgttResult> GetOgttResults()
        {
            return _context.OgttResults.Take(50);
        }

        /// <summary>
        /// Get an OGTT result by id
        /// </summary>
        /// <param name="id">The OGTT result id</param>
        /// <response code="200">Returns an OGTT result</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // GET: api/OgttResults/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetOgttResult([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ogttResult = await _context.OgttResults.FindAsync(id);

            if (ogttResult == null)
            {
                return NotFound();
            }

            return Ok(ogttResult);
        }

        /// <summary>
        /// Edit an OGTT result
        /// </summary>
        /// <param name="id">The OGTT result id</param>
        /// <param name="ogttResult">The OGTT result entity</param>
        /// <response code="204">Returns no content when updated</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // PUT: api/OgttResults/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOgttResult([FromRoute] int id, [FromBody] OgttResult ogttResult)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != ogttResult.Id)
            {
                return BadRequest();
            }

            _context.Entry(ogttResult).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OgttResultExists(id))
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
        /// Create a new OGTT result
        /// </summary>
        /// <param name="ogttResult">The OGTT result entity</param>
        /// <response code="201">Returns the OGTT result entity that was created</response>
        /// <response code="400">If bad request</response>
        // POST: api/OgttResults
        [HttpPost]
        public async Task<IActionResult> PostOgttResult([FromBody] OgttResult ogttResult)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.OgttResults.Add(ogttResult);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOgttResult", new { id = ogttResult.Id }, ogttResult);
        }

        /// <summary>
        /// Delete an OGTT result
        /// </summary>
        /// <param name="id">The OGTT result id</param>
        /// <response code="200">Returns the OGTT result that was deleted</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // DELETE: api/OgttResults/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOgttResult([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var ogttResult = await _context.OgttResults.FindAsync(id);
            if (ogttResult == null)
            {
                return NotFound();
            }

            _context.OgttResults.Remove(ogttResult);
            await _context.SaveChangesAsync();

            return Ok(ogttResult);
        }

        private bool OgttResultExists(int id)
        {
            return _context.OgttResults.Any(e => e.Id == id);
        }
    }
}