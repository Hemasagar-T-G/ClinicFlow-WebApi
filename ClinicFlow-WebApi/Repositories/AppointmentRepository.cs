using ClinicFlow_WebApi.Configuration;
using ClinicFlow_WebApi.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ClinicFlow_WebApi.Repositories
{
    public class AppointmentRepository : IAppointmentRepository
    {
        private readonly IMongoCollection<Appointment> appointmentCollection;

        public AppointmentRepository(IOptions<MongoDbSettings> settings)
        {
            var mongoClient = new MongoClient(settings.Value.ConnectionString);
            var mongoDatabase = mongoClient.GetDatabase(settings.Value.DatabaseName);
            appointmentCollection = mongoDatabase.GetCollection<Appointment>("Appointments");
        }

        public async Task<bool> AddAppointment(Appointment appointment)
        {
            try
            {
                await appointmentCollection.InsertOneAsync(appointment);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<List<Appointment>> GetAllAppointmentList()
        {
            try
            {
                return await appointmentCollection.Find(appointment => true).ToListAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<Appointment> GetLatestAppointmentByPatientId(string patientId)
        {
            return await appointmentCollection
                .Find(a => a.PatientId == patientId)
                .SortByDescending(a => a.AppointmentDate)
                .FirstOrDefaultAsync();
        }


        public async Task<Appointment> GetAppointmentById(string id)
        {
            try
            {
                return await appointmentCollection.Find(a => a.Id == id).FirstOrDefaultAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<List<Appointment>> GetAppointmentsByDoctorId(string doctorId)
        {
            try
            {
                return await appointmentCollection.Find(a => a.DoctorId == doctorId).ToListAsync();
            }
            catch (Exception ex) 
            {
                throw new Exception(ex.Message, ex);
            }
        }

        public async Task<bool> UpdateAppointment(Appointment appointment)
        {
            try
            {
                var result = await appointmentCollection.ReplaceOneAsync(a => a.Id == appointment.Id, appointment);
                return result.IsAcknowledged && result.ModifiedCount > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public async Task<bool> DeleteAppointmentById(string id)
        {
            try
            {
                var result = await appointmentCollection.DeleteOneAsync(a => a.Id == id);
                return result.IsAcknowledged && result.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }

}
