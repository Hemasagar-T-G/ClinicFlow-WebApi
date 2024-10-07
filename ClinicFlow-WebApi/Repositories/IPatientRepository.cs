using ClinicFlow_WebApi.Models;

namespace ClinicFlow_WebApi.Repositories
{
    public interface IPatientRepository
    {
        Task<bool> AddPatient(Patient patient);
        Task<List<Patient>> GetAllPatients();
        Task<Patient> GetPatientById(string id);
        Task<bool> UpdatePatient(Patient patient);
        Task<bool> DeletePatient(string id);
    }

}
