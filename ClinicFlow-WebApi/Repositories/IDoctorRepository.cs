using ClinicFlow_WebApi.Models;

namespace ClinicFlow_WebApi.Repositories
{
    public interface IDoctorRepository
    {
        Task<Doctor> GetDoctorById(string id);
        Task<List<Doctor>> GetAllDoctors();
        Task AddDoctor(Doctor doctor);
        Task UpdateDoctor(Doctor doctor);
        Task DeleteDoctor(string id);
    }

}
