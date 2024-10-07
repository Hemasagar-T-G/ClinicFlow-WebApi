using ClinicFlow_WebApi.DTOs;
using ClinicFlow_WebApi.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DoctorController : ControllerBase
    {
        private readonly DoctorService doctorService;

        public DoctorController(DoctorService doctorService)
        {
            this.doctorService = doctorService;
        }

        [HttpPost("AddDoctor")]
        public async Task<IActionResult> AddDoctor([FromBody] DoctorDto doctorDto)
        {
            await doctorService.AddDoctor(doctorDto);
            return CreatedAtAction(nameof(GetDoctorById), new { id = doctorDto.Name }, doctorDto);
        }

        [HttpGet("GetAllDoctorList")]
        public async Task<IActionResult> GetAllDoctors() => Ok(await doctorService.GetAllDoctors());


        [HttpGet("GetDoctorById")]
        public async Task<IActionResult> GetDoctorById(string id)
        {
            var doctor = await doctorService.GetDoctorById(id);
            if (doctor == null) return NotFound();
            return Ok(doctor);
        }

        [HttpPut("UpdateDoctorById")]
        public async Task<IActionResult> UpdateDoctorById(string id, [FromBody] DoctorDto doctorDto)
        {
            await doctorService.UpdateDoctor(id, doctorDto);
            return NoContent();
        }

        [HttpDelete("DeleteDoctorById")]
        public async Task<IActionResult> DeleteDoctor(string id)
        {
            await doctorService.DeleteDoctor(id);
            return NoContent();
        }
    }

}
