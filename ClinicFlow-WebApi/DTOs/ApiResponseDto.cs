namespace ClinicFlow_WebApi.DTOs
{
    public class ApiResponseDto<T>
    {
        public T Data { get; set; }
        public bool Success { get; set; }
        public string Message { get; set; }
        public string ErrorMessage { get; set; }

    }

}
