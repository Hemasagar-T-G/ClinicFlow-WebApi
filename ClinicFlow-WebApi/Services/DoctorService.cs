using ClinicFlow_WebApi.DTOs;
using ClinicFlow_WebApi.Models;
using ClinicFlow_WebApi.Repositories;
using System.Text;

namespace ClinicFlow_WebApi.Services
{
    public class DoctorService
    {
        private readonly IDoctorRepository doctorRepository;

        public DoctorService(IDoctorRepository doctorRepository)
        {
            this.doctorRepository = doctorRepository;
        }

        public async Task<Doctor> GetDoctorById(string id) => await doctorRepository.GetDoctorById(id);
        public async Task<List<Doctor>> GetAllDoctors() => await doctorRepository.GetAllDoctors();
        public async Task AddDoctor(DoctorDto doctorDto)
        {
            var doctor = new Doctor
            {
                DoctorId = GenerateDoctorId(doctorDto),
                Name = doctorDto.Name,
                Specialty = doctorDto.Specialty,
                Email = doctorDto.Email
            };
            await doctorRepository.AddDoctor(doctor);
        }

        public async Task UpdateDoctor(string id, DoctorDto doctorDto)
        {
            var doctor = await doctorRepository.GetDoctorById(id);
            if (doctor != null)
            {
                doctor.Name = doctorDto.Name;
                doctor.Specialty = doctorDto.Specialty;
                doctor.Email = doctorDto.Email;
                await doctorRepository.UpdateDoctor(doctor);
            }
        }

        public async Task DeleteDoctor(string id) => await doctorRepository.DeleteDoctor(id);

        /*------UTILITY METHODS------*/
        public string GenerateDoctorId(DoctorDto doctorDto)
        {
            // Helper function to get valid characters
            string GetValidCharacters(string input, int requiredCount)
            {
                StringBuilder validChars = new StringBuilder();

                foreach (char c in input)
                {
                    if (char.IsLetter(c) && validChars.Length < requiredCount)
                    {
                        validChars.Append(c);
                    }
                }

                // Return what we have, ensuring it doesn't exceed the required count
                return validChars.Length < requiredCount ? validChars.ToString() : validChars.ToString().Substring(0, requiredCount);
            }

            // Get the first three valid characters from the name and specialty
            var namePart = GetValidCharacters(doctorDto.Name, 3).ToUpper();
            var specialtyPart = GetValidCharacters(doctorDto.Specialty, 3).ToUpper();

            // Generate a random number between 101 and 999
            Random random = new Random();
            int randomNumber = random.Next(101, 1000);

            // Construct the DoctorId
            return $"DOC-{namePart}-{specialtyPart}-{randomNumber}";
        }

    }

}
