namespace ClinicFlow_WebApi.DTOs
{
    public class AppointmentDto
    {
        public string DoctorId { get; set; }
        public string PatientId { get; set; }
        public DateTime AppointmentDate { get; set; }
    }

}
