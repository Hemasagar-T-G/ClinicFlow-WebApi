using ClinicFlow_WebApi.DTOs;
using ClinicFlow_WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientController : ControllerBase
    {
        private readonly PatientService patientService;

        public PatientController(PatientService patientService)
        {
            this.patientService = patientService;
        }

        [HttpPost("AddPatient")]
        public async Task<IActionResult> AddPatient([FromBody] PatientDto patientDto)
        {
            var result = await patientService.AddPatient(patientDto);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("GetAllPatientList")]
        public async Task<IActionResult> GetAllPatientList()
        {
            var result = await patientService.GetAllPatients();
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpGet("GetPatientById")]
        public async Task<IActionResult> GetPatientById(string id)
        {
            var result = await patientService.GetPatientById(id);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpPut("UpdatePatientById")]
        public async Task<IActionResult> UpdatePatientById(string id, [FromBody] PatientDto patientDto)
        {
            var result = await patientService.UpdatePatientById(id, patientDto);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }

        [HttpDelete("DeletePatientById")]
        public async Task<IActionResult> DeletePatientById(string id)
        {
           var result = await patientService.DeletePatientById(id);
            if (result.Success) return Ok(result);
            return BadRequest(result);
        }
    }

}
