using ClinicFlow_WebApi.DTOs;
using ClinicFlow_WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AppointmentController : ControllerBase
    {
        private readonly AppointmentService appointmentService;

        public AppointmentController(AppointmentService appointmentService)
        {
            this.appointmentService = appointmentService;
        }

        [HttpPost("AddAppointment")]
        public async Task<IActionResult> AddAppointment([FromBody] AppointmentDto appointmentDto)
        {
            var result = await appointmentService.AddAppointment(appointmentDto);
            if (result.Success) { return Ok(result); }
            return BadRequest(result);
        }

        [HttpGet("GetAppointmentById")]
        public async Task<IActionResult> GetAppointmentById(string id)
        {
            var appointment = await appointmentService.GetAppointmentById(id);
            if (appointment == null) return NotFound();
            return Ok(appointment);
        }

        [HttpGet("GetAppointmentsByDoctorId")]
        public async Task<IActionResult> GetAppointmentsByDoctorId(string doctorId)
            => Ok(await appointmentService.GetAppointmentsByDoctorId(doctorId));

        

        [HttpPut("UpdateAppointmentById")]
        public async Task<IActionResult> UpdateAppointmentById(string id, [FromBody] AppointmentDto appointmentDto)
        {
            await appointmentService.UpdateAppointment(id, appointmentDto);
            return NoContent();
        }

        [HttpDelete("CancelAppointmentById")]
        public async Task<IActionResult> CancelAppointment(string id)
        {
            await appointmentService.CancelAppointment(id);
            return NoContent();
        }
    }

}
