using ClinicFlow_WebApi.DTOs;
using ClinicFlow_WebApi.Models;
using ClinicFlow_WebApi.Repositories;
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
        private readonly AppointmentService appointmentService;

        public PatientController(PatientService patientService, AppointmentService appointmentService)
        {
            this.patientService = patientService;
            this.appointmentService = appointmentService;
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

        [HttpGet("GetAppointmentReceiptByPatientId")]
        public async Task<IActionResult> GetAppointmentReceiptByPatientId(string patientId)
        {
            var response = await appointmentService.GetAppointmentReceiptByPatientId(patientId);

            if (!response.Success)
            {
                return BadRequest(new { message = response.Message });
            }

            // Create a file result to download the PDF
            return File(response.Data, "application/pdf", "Receipt.pdf");
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
