using ClinicFlow_WebApi.DTOs;
using ClinicFlow_WebApi.Models;
using ClinicFlow_WebApi.Repositories;
using ClinicFlow_WebApi.Utilities.Enums;
using System.Text;

namespace ClinicFlow_WebApi.Services
{
    public class PatientService
    {
        private readonly IPatientRepository patientRepository;

        public PatientService(IPatientRepository patientRepository)
        {
            this.patientRepository = patientRepository;
        }

        public async Task<ApiResponseDto<bool>> AddPatient(PatientDto patientDto)
        {
            if (string.IsNullOrWhiteSpace(patientDto.Name) ||
                string.IsNullOrWhiteSpace(patientDto.PhoneNumber) ||
                string.IsNullOrWhiteSpace(patientDto.Email))
            {
                return new ApiResponseDto<bool> { Data = false, Success = false, Message = ResponseMessages.Validation_Failed.ToString(), ErrorMessage = "Name, Phone Number, and Email are required." };
            }
            try
            {
                var patient = new Patient
                {
                    PatientId = GeneratePatientId(patientDto),
                    Name = patientDto.Name,
                    PhoneNumber = patientDto.PhoneNumber,
                    Email = patientDto.Email
                };

                var result = await patientRepository.AddPatient(patient);
                return new ApiResponseDto<bool> { Data = result, Success = true, Message = ResponseMessages.Create_Success.ToString(), ErrorMessage = null };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<bool> { Data = false, Success = false, Message = ResponseMessages.Create_Failed.ToString(), ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponseDto<List<Patient>>> GetAllPatients()
        {
            try
            {
                var result = await patientRepository.GetAllPatients();
                return new ApiResponseDto<List<Patient>> { Data = result, Success = true, Message = ResponseMessages.Retrieve_Success.ToString(), ErrorMessage = null };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<List<Patient>> { Data = null, Success = false, Message = ResponseMessages.Retrieve_Failed.ToString(), ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponseDto<Patient>> GetPatientById(string id)
        {
            try
            {
                var result = await patientRepository.GetPatientById(id);

                if (result == null)
                {
                    return new ApiResponseDto<Patient> { Data = null, Success = false, Message = ResponseMessages.Not_Found.ToString(), ErrorMessage = "Patient_Not_Found." };
                }

                return new ApiResponseDto<Patient> { Data = result, Success = true, Message = ResponseMessages.Retrieve_Success.ToString(), ErrorMessage = null };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<Patient> { Data = null, Success = false, Message = ResponseMessages.Retrieve_Failed.ToString(), ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponseDto<bool>> UpdatePatientById(string id, PatientDto patientDto)
        {
            try
            {
                var patient = await patientRepository.GetPatientById(id);

                if (patient == null)
                {
                    return new ApiResponseDto<bool> { Data = false, Success = false, Message = ResponseMessages.Not_Found.ToString(), ErrorMessage = "Patient_Not_Found." };
                }

                patient.Name = patientDto.Name;
                patient.PhoneNumber = patientDto.PhoneNumber;
                patient.Email = patientDto.Email;

                bool result = await patientRepository.UpdatePatient(patient);
                return new ApiResponseDto<bool> { Data = result, Success = true, Message = ResponseMessages.Update_Success.ToString(), ErrorMessage = null };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<bool> { Data = false, Success = false, Message = ResponseMessages.Update_Failed.ToString(), ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponseDto<bool>> DeletePatientById(string id)
        {
            var existingPatient = await patientRepository.GetPatientById(id);
            if (existingPatient == null)
            {
                return new ApiResponseDto<bool> { Data = false, Success = false, Message = ResponseMessages.Retrieve_Failed.ToString(), ErrorMessage = $"Patient with ID {id} not found." };
            }
            try
            {
                await patientRepository.DeletePatient(id);
                return new ApiResponseDto<bool> { Data = true, Success = true, Message = ResponseMessages.Delete_Success.ToString(), ErrorMessage = null };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<bool> { Data = false, Success = false, Message = ResponseMessages.Delete_Failed.ToString(), ErrorMessage = ex.Message };
            }
        }

        /*------UTILITY METHODS------*/
        public string GeneratePatientId(PatientDto patientDto)
        {
            string GetValidCharacters(string input, int requiredCount)
            {
                StringBuilder validChars = new StringBuilder();
                foreach (char c in input)
                {
                    if (char.IsLetter(c) || char.IsDigit(c))
                    {
                        validChars.Append(c);
                    }

                    if (validChars.Length >= requiredCount)
                        break;
                }
                return validChars.Length < requiredCount ? validChars.ToString() : validChars.ToString().Substring(0, requiredCount);
            }
            var namePart = GetValidCharacters(patientDto.Name, 3).ToUpper();
            var phonePart = GetValidCharacters(patientDto.PhoneNumber, 3).ToUpper();
            Random random = new Random();
            int randomNumber = random.Next(101, 1000);
            return $"PAT-{namePart}-{phonePart}-{randomNumber}";
        }

    }

}
