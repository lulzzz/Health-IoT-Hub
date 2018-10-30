using System;
using System.Collections.Generic;
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
    [ApiExplorerSettings(IgnoreApi = true)]
    [Authorize]
    //[ApiExplorerSettings(GroupName = "datastore")]
    [Produces("application/json")]
    [Route("api/v1/[controller]")]
    [ApiController]
    public class TestCodesController : ControllerBase
    {
        private readonly DatabaseDBContext _context;

        public TestCodesController(DatabaseDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get 250 test codes
        /// </summary>
        /// <response code="200">Returns a list of test codes</response>
        // GET: api/TestCodes
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<TestCode>), 200)]
        public IEnumerable<TestCode> GetTestCodes()
        {
            return _context.TestCodes.Take(250);
        }

        /// <summary>
        /// Get test code by id
        /// </summary>
        /// <param name="id">The test code id</param>
        /// <response code="200">Returns a test code</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // GET: api/TestCodes/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(TestCode), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetTestCode([FromRoute] string id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var testCode = await _context.TestCodes.FindAsync(id);

            if (testCode == null)
            {
                return NotFound();
            }

            return Ok(testCode);
        }

        /// <summary>
        /// Create a new test code
        /// </summary>
        /// <param name="testCode">The test code entity</param>
        /// <response code="201">Returns the test code entity that was created</response>
        /// <response code="400">If bad request</response>
        // POST: api/TestCodes
        [HttpPost]
        [ProducesResponseType(typeof(TestCode), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostTestCode([FromBody] TestCode testCode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.TestCodes.Add(testCode);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetTestCode", new { id = testCode.Id }, testCode);
        }

        /// <summary>
        /// Edit a test code
        /// </summary>
        /// <param name="id">The test code id</param>
        /// <param name="testcode">The test code entity</param>
        /// <response code="204">Returns no content when updated</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // PUT: api/TestCodes/5
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutTestCode([FromRoute] int id, [FromBody] TestCode testcode)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != testcode.Id)
            {
                return BadRequest();
            }

            _context.Entry(testcode).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!TestCodeExists(id))
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
        /// Delete a test code
        /// </summary>
        /// <param name="id">The test code id</param>
        /// <response code="200">Returns the test code entity that was deleted</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // DELETE: api/TestCode/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(TestCode), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeleteTestCode([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var testCode = await _context.TestCodes.FindAsync(id);
            if (testCode == null)
            {
                return NotFound();
            }

            _context.TestCodes.Remove(testCode);
            await _context.SaveChangesAsync();

            return Ok(testCode);
        }

        private bool TestCodeExists(int id)
        {
            return _context.TestCodes.Any(e => e.Id == id);
        }
    }
}
