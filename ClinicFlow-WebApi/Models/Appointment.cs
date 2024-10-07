using ClinicFlow_WebApi.Utilities.Enums;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ClinicFlow_WebApi.Models
{
    public class Appointment
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)] 
        public string Id { get; set; }
        public string ApponitmentId { get; set; }
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
        public DateTime AppointmentDate { get; set; }
        public AppointmentStatus Status { get; set; }
    }

}
