using ClinicFlow_WebApi.Configuration;
using ClinicFlow_WebApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClinicFlow_WebApi.Repositories
{
    public class DoctorRepository : IDoctorRepository
    {
        private readonly IMongoCollection<Doctor> _doctors;

        public DoctorRepository(IOptions<MongoDbSettings> settings)
        {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
            _doctors = mongoDatabase.GetCollection<Doctor>("Doctors");
        }

        public async Task<Doctor> GetDoctorById(string id) => await _doctors.Find(d => d.Id == id).FirstOrDefaultAsync();
        public async Task<List<Doctor>> GetAllDoctors() => await _doctors.Find(d => true).ToListAsync();
        public async Task AddDoctor(Doctor doctor) => await _doctors.InsertOneAsync(doctor);
        public async Task UpdateDoctor(Doctor doctor) => await _doctors.ReplaceOneAsync(d => d.Id == doctor.Id, doctor);
        public async Task DeleteDoctor(string id) => await _doctors.DeleteOneAsync(d => d.Id == id);
    }

}
