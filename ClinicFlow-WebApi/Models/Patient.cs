using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;

namespace ClinicFlow_WebApi.Models
{
    public class Patient
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string PatientId { get; set; } 
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
    }

}
