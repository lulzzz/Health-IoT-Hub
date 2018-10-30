using System;
using System.Collections.Generic;
using System.Globalization;
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
    public class PatientsController : ControllerBase
    {
        private readonly DatabaseDBContext _context;

        public PatientsController(DatabaseDBContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Get first 50 patients 
        /// </summary>
        /// <response code="200">Returns a list of patients</response>
        // GET: api/Patients
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<Patient>), 200)]
        public IEnumerable<Patient> GetPatients()
        {
            return _context.Patients.Take(50);
        }

        /// <summary>
        /// Get a patient by id
        /// </summary>
        /// <param name="id">The patient id</param>
        /// <response code="200">Returns a patient</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // GET: api/Patients/5
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(Patient), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPatient([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var patient = await _context.Patients.FindAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        /// <summary>
        /// Get patients by first name and last name
        /// </summary>
        /// <param name="firstName">The patient's first name (first three letters)</param>
        /// <param name="lastName">The patient's last name (first three letters)</param>
        /// <response code="200">Returns a list of patients</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // GET: api/Patients/firstname/john/lastname/doe
        [HttpGet("firstname/{firstName}/lastname/{lastName}")]
        [ProducesResponseType(typeof(List<Patient>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPatientByName([FromRoute] string firstName, string lastName)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName))
            {
                return BadRequest(ModelState);
            }

            string fName = firstName.Substring(0, 3).ToLower();
            string lName = lastName.Substring(0, 3).ToLower();

            var lstPatient = await _context.Patients.Where
                (w => w.GivenName.ToLower().Contains(fName) && w.FamilyName.ToLower().Contains(lName))
                .ToListAsync();

            if (lstPatient?.Count() < 1)
            {
                return NotFound();
            }

            return Ok(lstPatient);
        }

        /// <summary>
        /// Get a patient by first name , last name and date of birth
        /// </summary>
        /// <param name="firstName">The patient's first name</param>
        /// <param name="lastName">The patient's last name</param>
        /// <param name="dateOfBirth">The patient's date of birth - string("ddMMyyyy") [eg: "14021980"]</param>
        /// <response code="200">Returns a patient</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // GET: api/Patients/firstname/john/lastname/doe/dateofbirth/14021980
        [HttpGet("firstname/{firstName}/lastname/{lastName}/dateofbirth/{dateOfBirth}")]
        [ProducesResponseType(typeof(List<Patient>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetPatientByNameAndDateOfBirth([FromRoute] string firstName, string lastName, string dateOfBirth)
        {
            if (string.IsNullOrEmpty(firstName) || string.IsNullOrEmpty(lastName) || string.IsNullOrEmpty(dateOfBirth))
            {
                return BadRequest(ModelState);
            }

            string fName = firstName.ToLower();
            string lName = lastName.ToLower();
            DateTime dob;
            DateTime.TryParseExact(dateOfBirth, "ddMMyyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out dob);

            Patient patient = await _context.Patients.Where
                (w => w.GivenName.ToLower() == fName && w.FamilyName.ToLower() == lastName && w.DateOfBirth.Value.Date == dob.Date )
                .FirstOrDefaultAsync();

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(patient);
        }

        /// <summary>
        /// Edit a patient
        /// </summary>
        /// <param name="id">The patient id</param>
        /// <param name="patient">The patient entity</param>
        /// <response code="204">Returns no content when updated</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // PUT: api/Patients/5
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> PutPatient([FromRoute] int id, [FromBody] Patient patient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != patient.Id)
            {
                return BadRequest();
            }

            if (string.IsNullOrEmpty(patient.FamilyName) || string.IsNullOrEmpty(patient.GivenName))
                return BadRequest();

            if (patient.DateOfBirth == null)
                return BadRequest();

            if (string.IsNullOrEmpty(patient.MiddleName))
                patient.MiddleName = "-";

            patient.Discriminator = $"{patient.GivenName.ToLower()}{patient.MiddleName.ToLower()}{patient.FamilyName.ToLower()}{patient.DateOfBirth.Value.ToString("ddMMyyyy")}";

            _context.Entry(patient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PatientExists(id))
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
        /// Create a new patient
        /// </summary>
        /// <param name="patient">The patient entity</param>
        /// <response code="201">Returns the patient entity that was created</response>
        /// <response code="400">If bad request</response>
        // POST: api/Patients
        [HttpPost]
        [ProducesResponseType(typeof(Patient), 201)]
        [ProducesResponseType(400)]
        public async Task<IActionResult> PostPatient([FromBody] Patient patient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (string.IsNullOrEmpty(patient.FamilyName) || string.IsNullOrEmpty(patient.GivenName))
                return BadRequest();

            if (patient.DateOfBirth == null)
                return BadRequest();

            if (string.IsNullOrEmpty(patient.MiddleName))
                patient.MiddleName = "-";

            patient.Discriminator = $"{patient.GivenName.ToLower()}{patient.MiddleName.ToLower()}{patient.FamilyName.ToLower()}{patient.DateOfBirth.Value.ToString("ddMMyyyy")}";

            // TODO -- CHECK DISCRIMINATOR ------

            if (patient?.Active != true)
                patient.Active = true;

            _context.Patients.Add(patient);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetPatient", new { id = patient.Id }, patient);
        }

        /// <summary>
        /// Delete a patient
        /// </summary>
        /// <param name="id">The patient id</param>
        /// <response code="200">Returns the patient that was deleted</response>
        /// <response code="400">If bad request</response>
        /// <response code="404">If not found</response>
        // DELETE: api/Patients/5
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(Patient), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> DeletePatient([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var patient = await _context.Patients.FindAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            List<DiagnosticReport> reports =  _context.DiagnosticReports.Where(w => w.PatientId == patient.Id).ToList();
            if(reports?.Count > 0)
            {
                foreach(DiagnosticReport report in reports)
                {
                    List<Result> results = _context.Results.Where(w => w.DiagnosticReportId == report.Id).ToList();
                    if(results?.Count > 0)
                    {
                        foreach(Result result in results)
                        {
                            _context.Results.Remove(result);
                            _context.SaveChanges();
                        }
                    }

                    _context.DiagnosticReports.Remove(report);
                    _context.SaveChanges();
                }
            }

            _context.Patients.Remove(patient);
            await _context.SaveChangesAsync();

            return Ok(patient);
        }

        private bool PatientExists(int id)
        {
            return _context.Patients.Any(e => e.Id == id);
        }
    }
}