using ClinicFlow_WebApi.Configuration;
using ClinicFlow_WebApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System.Linq.Expressions;

namespace ClinicFlow_WebApi.Repositories
{
    public class PatientRepository : IPatientRepository
    {
        private readonly IMongoCollection<Patient> patientCollection;

        public PatientRepository(IOptions<MongoDbSettings> settings)
        {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
            patientCollection = mongoDatabase.GetCollection<Patient>("Patients");
        }

        public async Task<bool> AddPatient(Patient patient)
        {
            try
            {
                await patientCollection.InsertOneAsync(patient);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<Patient> GetPatientById(string id)
        {
            try
            {
                return await patientCollection.Find(p => p.PatientId == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<Patient>> GetAllPatients()
        {
            try
            {
                return await patientCollection.Find(_ => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> UpdatePatient(Patient patient)
        {
            try
            {
                var updateResult = await patientCollection.ReplaceOneAsync(p => p.Id == patient.Id, patient);
                return updateResult.IsAcknowledged && updateResult.MatchedCount > 0;
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> DeletePatient(string id)
        {
            try
            {
                var result = await patientCollection.DeleteOneAsync(p => p.Id == id);
                return result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }
    }

}
