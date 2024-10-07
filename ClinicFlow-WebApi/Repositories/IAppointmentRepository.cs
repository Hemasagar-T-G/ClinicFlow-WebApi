﻿using ClinicFlow_WebApi.Models;

namespace ClinicFlow_WebApi.Repositories
{
    public interface IAppointmentRepository
    {
        Task<bool> AddAppointment(Appointment appointment);

        Task<List<Appointment>> GetAllAppointmentList();

        Task<Appointment> GetAppointmentById(string id);

        Task<List<Appointment>> GetAppointmentsByDoctorId(string doctorId);

        Task<bool> UpdateAppointment(Appointment appointment);

        Task<bool> DeleteAppointment(string id);
    }

}
