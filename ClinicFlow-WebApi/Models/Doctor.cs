using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ClinicFlow_WebApi.Models
{
    public class Doctor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string DoctorId { get; set; }
        public string Name { get; set; }
        public string Specialty { get; set; }
        public string Email { get; set; }
        public List<Appointment> Appointments { get; set; } = new List<Appointment>();
    }

}
