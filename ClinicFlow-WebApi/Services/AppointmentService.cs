using ClinicFlow_WebApi.DTOs;
using ClinicFlow_WebApi.Models;
using ClinicFlow_WebApi.Repositories;
using ClinicFlow_WebApi.Utilities.Enums;

namespace ClinicFlow_WebApi.Services
{
    public class AppointmentService
    {
        private readonly IAppointmentRepository appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            this.appointmentRepository = appointmentRepository;
        }

        public async Task<ApiResponseDto<bool>> AddAppointment(AppointmentDto appointmentDto)
        {
            try
            {
                var appointment = new Appointment
                {
                    ApponitmentId = GenerateAppointmentId(appointmentDto.DoctorId, appointmentDto.PatientId),
                    DoctorId = appointmentDto.DoctorId,
                    PatientId = appointmentDto.PatientId,
                    AppointmentDate = appointmentDto.AppointmentDate,
                    Status = AppointmentStatus.Scheduled
                };
                var result = await appointmentRepository.AddAppointment(appointment);
                return new ApiResponseDto<bool> { Data = true, Success = true, Message = ResponseMessages.Create_Success.ToString(), ErrorMessage = null };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<bool> { Data = false, Success = false, Message = ResponseMessages.Create_Failed.ToString(), ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponseDto<List<Appointment>>> GetAllAppointmentList()
        {
            try
            {
                var result = await appointmentRepository.GetAllAppointmentList();
                return new ApiResponseDto<List<Appointment>> { Data = result, Success = true, Message = ResponseMessages.Retrieve_Success.ToString(), ErrorMessage = null };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<List<Appointment>> { Data = null, Success = false, Message = ResponseMessages.Retrieve_Failed.ToString(), ErrorMessage = ex.Message };
            }
        }

        public async Task<Appointment> GetAppointmentById(string id) => await appointmentRepository.GetAppointmentById(id);
        public async Task<ApiResponseDto<List<Appointment>>> GetAppointmentsByDoctorId(string doctorId)
        {
            try
            {
                var result = await appointmentRepository.GetAppointmentsByDoctorId(doctorId);

                return new ApiResponseDto<List<Appointment>> { Data = result, Success = true, Message = ResponseMessages.Retrieve_Success.ToString(), ErrorMessage = null };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<List<Appointment>> { Data = null, Success = false, Message = ResponseMessages.Retrieve_Failed.ToString(), ErrorMessage = ex.Message };
            }
        }

        public async Task UpdateAppointment(string id, AppointmentDto appointmentDto)
        {
            var appointment = await appointmentRepository.GetAppointmentById(id);
            if (appointment != null)
            {
                appointment.AppointmentDate = appointmentDto.AppointmentDate;
                await appointmentRepository.UpdateAppointment(appointment);
            }
        }

        public async Task CancelAppointment(string id)
        {
            var appointment = await appointmentRepository.GetAppointmentById(id);
            if (appointment != null)
            {
                appointment.Status = AppointmentStatus.Cancelled;
                await appointmentRepository.UpdateAppointment(appointment);
            }
        }

        //==========UTILITY METHODS==========//
        public string GenerateAppointmentId(string doctorId, string patientId)
        {
            const string TOKEN = "TOKEN";
            string doctorIdPart = doctorId.Length >= 3 ? doctorId.Substring(doctorId.Length - 3) : "000";
            string patientIdPart = patientId.Length >= 3 ? patientId.Substring(patientId.Length - 3) : "000";
            Random random = new Random();
            int randomNumber = random.Next(0, 10000);
            string serialNumberPart = randomNumber.ToString("D4");
            string appointmentId = $"{TOKEN}-{doctorIdPart}-{patientIdPart}-{serialNumberPart}";
            return appointmentId;
        }
    }

}
