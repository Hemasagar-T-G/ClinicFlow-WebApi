using ClinicFlow_WebApi.DTOs;
using ClinicFlow_WebApi.Models;
using ClinicFlow_WebApi.Repositories;
using ClinicFlow_WebApi.Utilities.Enums;
using iText.IO.Font.Constants;
using iText.Kernel.Pdf;
using System;
using System.Drawing;
using System.Drawing.Printing;
using static System.Net.Mime.MediaTypeNames;
using System.Xml.Linq;
using PdfSharp.Pdf;
using PdfSharp.Drawing;
using System.IO;
using static iText.StyledXmlParser.Jsoup.Select.Evaluator;

namespace ClinicFlow_WebApi.Services
{
    public class AppointmentService
    {
        private readonly IAppointmentRepository appointmentRepository;

        public AppointmentService(IAppointmentRepository appointmentRepository)
        {
            this.appointmentRepository = appointmentRepository;
        }

        public async Task<ApiResponseDto<bool>> AddAppointment(AppointmentDto appointmentDto)
        {
            try
            {
                var appointment = new Appointment
                {
                    ApponitmentId = GenerateAppointmentId(appointmentDto.DoctorId, appointmentDto.PatientId),
                    DoctorId = appointmentDto.DoctorId,
                    PatientId = appointmentDto.PatientId,
                    AppointmentDate = appointmentDto.AppointmentDate,
                    Status = AppointmentStatus.Scheduled
                };
                var result = await appointmentRepository.AddAppointment(appointment);
                return new ApiResponseDto<bool> { Data = true, Success = true, Message = ResponseMessages.Create_Success.ToString(), ErrorMessage = null };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<bool> { Data = false, Success = false, Message = ResponseMessages.Create_Failed.ToString(), ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponseDto<List<Appointment>>> GetAllAppointmentList()
        {
            try
            {
                var result = await appointmentRepository.GetAllAppointmentList();
                return new ApiResponseDto<List<Appointment>> { Data = result, Success = true, Message = ResponseMessages.Retrieve_Success.ToString(), ErrorMessage = null };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<List<Appointment>> { Data = null, Success = false, Message = ResponseMessages.Retrieve_Failed.ToString(), ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponseDto<byte[]>> GetAppointmentReceiptByPatientId(string patientId)
        {
            try
            {
                var latestAppointment = await appointmentRepository.GetLatestAppointmentByPatientId(patientId);
                if (latestAppointment == null)
                {
                    return new ApiResponseDto<byte[]>
                    {
                        Success = false,
                        Message = "No appointments found for the patient.",
                        Data = null
                    };
                }

                var pdfBytes = GenerateAppointmentReceiptPdf(latestAppointment);

                return new ApiResponseDto<byte[]>
                {
                    Data = pdfBytes,
                    Success = true,
                    Message = ResponseMessages.Receipt_Generated_Successfully.ToString(),
                    ErrorMessage = null
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<byte[]>
                {
                    Success = false,
                    Message = "An error occurred while generating the receipt.",
                    ErrorMessage = ex.Message,
                    Data = null
                };
            }
        }

        //public async Task<Appointment> GetAppointmentById(string id) => await appointmentRepository.GetAppointmentById(id);
        public async Task<ApiResponseDto<Appointment>> GetAppointmentById(string Id)
        {
            try
            {
                var result = await appointmentRepository.GetAppointmentById(Id);
                return new ApiResponseDto<Appointment> { Data = result, Success = true, Message = ResponseMessages.Retrieve_Success.ToString(), ErrorMessage = null };
            }
            catch(Exception ex)
            {
                return new ApiResponseDto<Appointment> { Data = null, Success = false, Message = ResponseMessages.Retrieve_Failed.ToString(), ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponseDto<List<Appointment>>> GetAppointmentsByDoctorId(string doctorId)
        {
            try
            {
                var result = await appointmentRepository.GetAppointmentsByDoctorId(doctorId);

                return new ApiResponseDto<List<Appointment>> { Data = result, Success = true, Message = ResponseMessages.Retrieve_Success.ToString(), ErrorMessage = null };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<List<Appointment>> { Data = null, Success = false, Message = ResponseMessages.Retrieve_Failed.ToString(), ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponseDto<Appointment>> UpdateAppointment(string id, AppointmentDto appointmentDto)
        {
            try
            {
                var appointment = await appointmentRepository.GetAppointmentById(id);

                if (appointment == null)
                {
                    return new ApiResponseDto<Appointment>
                    {
                        Success = false,
                        Message = ResponseMessages.Not_Found.ToString(),
                        ErrorMessage = "The appointment with the provided ID does not exist."
                    };
                }

                appointment.AppointmentDate = appointmentDto.AppointmentDate;
                await appointmentRepository.UpdateAppointment(appointment);

                return new ApiResponseDto<Appointment>
                {
                    Data = appointment,
                    Success = true,
                    Message = ResponseMessages.Update_Success.ToString(),
                    ErrorMessage = null
                };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<Appointment> { Data = null, Success = false, Message = ResponseMessages.Update_Failed.ToString(), ErrorMessage = ex.Message };
            }
        }

        public async Task<ApiResponseDto<Appointment>> CancelAppointment(string id)
        {
            try
            {
                var result = await appointmentRepository.GetAppointmentById(id);

                if (result == null)
                {
                    return new ApiResponseDto<Appointment>
                    {
                        Data = null,
                        Success = false,
                        Message = ResponseMessages.Not_Found.ToString(),
                        ErrorMessage = "The appointment with the provided ID does not exist."
                    };
                }

                result.Status = AppointmentStatus.Cancelled;

                var updateResult = await appointmentRepository.UpdateAppointment(result);
                if (!updateResult)
                {
                    return new ApiResponseDto<Appointment> { Data = null, Success = false, Message = ResponseMessages.Update_Failed.ToString(), ErrorMessage = "An error occurred while updating the appointment." };
                }

                return new ApiResponseDto<Appointment> { Data = result, Success = true, Message = ResponseMessages.Update_Success.ToString(), ErrorMessage = null };
            }
            catch (Exception ex)
            {
                return new ApiResponseDto<Appointment> { Success = false, Message = ResponseMessages.Update_Failed.ToString(), ErrorMessage = ex.Message };
            }
        }

        //==========UTILITY METHODS==========//
        public string GenerateAppointmentId(string doctorId, string patientId)
        {
                const string TOKEN = "TOKEN";
                string doctorIdPart = doctorId.Length >= 3 ? doctorId.Substring(doctorId.Length - 3) : "000";
                string patientIdPart = patientId.Length >= 3 ? patientId.Substring(patientId.Length - 3) : "000";
                Random random = new Random();
                int randomNumber = random.Next(0, 10000);
                string serialNumberPart = randomNumber.ToString("D4");
                string appointmentId = $"{TOKEN}-{doctorIdPart}-{patientIdPart}-{serialNumberPart}";
                return appointmentId;
        }

        private byte[] GenerateAppointmentReceiptPdf(Appointment appointment)
        {
            using (var memoryStream = new MemoryStream())
            {
                // Create a new PDF document
                var pdfDocument = new PdfSharp.Pdf.PdfDocument();
                var page = pdfDocument.AddPage();
                var gfx = XGraphics.FromPdfPage(page);

                // Define fonts
                var titleFont = new XFont("Arial", 20);
                var headerFont = new XFont("Arial", 16);
                var font = new XFont("Arial", 14);

                // Set up margins
                double margin = 20;

                // Title
                gfx.DrawString("#--- RECEIPT ---#", titleFont, XBrushes.Blue,
                    new XRect(0, margin, page.Width, 40), XStringFormats.TopCenter);
                gfx.DrawString("APPOINTMENT DETAILS", headerFont, XBrushes.Black,
                    new XRect(margin, margin + 50, page.Width, 20), XStringFormats.Center);

                double currentY = margin + 80;
                double cellHeight = 25;

                gfx.DrawRectangle(XPens.Black, new XRect(margin, currentY, (page.Width - (2 * margin)) / 2, cellHeight));
                gfx.DrawString("Appointment ID:", font, XBrushes.Black,
                    new XRect(margin, currentY, (page.Width - (2 * margin)) / 2, cellHeight), XStringFormats.Center);

                gfx.DrawRectangle(XPens.Black, new XRect(margin + (page.Width - (2 * margin)) / 2, currentY, (page.Width - (2 * margin)) / 2, cellHeight));
                gfx.DrawString(appointment.ApponitmentId, font, XBrushes.Black,
                    new XRect(margin + (page.Width - (2 * margin)) / 2, currentY, (page.Width - (2 * margin)) / 2, cellHeight), XStringFormats.Center);

                currentY += cellHeight;

                gfx.DrawRectangle(XPens.Black, new XRect(margin, currentY, (page.Width - (2 * margin)) / 2, cellHeight));
                gfx.DrawString("Doctor ID:", font, XBrushes.Black,
                    new XRect(margin, currentY, (page.Width - (2 * margin)) / 2, cellHeight), XStringFormats.Center);

                gfx.DrawRectangle(XPens.Black, new XRect(margin + (page.Width - (2 * margin)) / 2, currentY, (page.Width - (2 * margin)) / 2, cellHeight));
                gfx.DrawString(appointment.DoctorId, font, XBrushes.Black,
                    new XRect(margin + (page.Width - (2 * margin)) / 2, currentY, (page.Width - (2 * margin)) / 2, cellHeight), XStringFormats.Center);

                currentY += cellHeight;

                gfx.DrawRectangle(XPens.Black, new XRect(margin, currentY, (page.Width - (2 * margin)) / 2, cellHeight));
                gfx.DrawString("Patient ID:", font, XBrushes.Black,
                    new XRect(margin, currentY, (page.Width - (2 * margin)) / 2, cellHeight), XStringFormats.Center);

                gfx.DrawRectangle(XPens.Black, new XRect(margin + (page.Width - (2 * margin)) / 2, currentY, (page.Width - (2 * margin)) / 2, cellHeight));
                gfx.DrawString(appointment.PatientId, font, XBrushes.Black,
                    new XRect(margin + (page.Width - (2 * margin)) / 2, currentY, (page.Width - (2 * margin)) / 2, cellHeight), XStringFormats.Center);

                currentY += cellHeight;

                gfx.DrawRectangle(XPens.Black, new XRect(margin, currentY, (page.Width - (2 * margin)) / 2, cellHeight));
                gfx.DrawString("Appointment Date:", font, XBrushes.Black,
                    new XRect(margin, currentY, (page.Width - (2 * margin)) / 2, cellHeight), XStringFormats.Center);

                gfx.DrawRectangle(XPens.Black, new XRect(margin + (page.Width - (2 * margin)) / 2, currentY, (page.Width - (2 * margin)) / 2, cellHeight));
                gfx.DrawString(appointment.AppointmentDate.ToString("MM/dd/yyyy HH:mm"), font, XBrushes.Black,
                    new XRect(margin + (page.Width - (2 * margin)) / 2, currentY, (page.Width - (2 * margin)) / 2, cellHeight), XStringFormats.Center);

                currentY += cellHeight;

                gfx.DrawRectangle(XPens.Black, new XRect(margin, currentY, (page.Width - (2 * margin)) / 2, cellHeight));
                gfx.DrawString("Status:", font, XBrushes.Black,
                    new XRect(margin, currentY, (page.Width - (2 * margin)) / 2, cellHeight), XStringFormats.Center);

                gfx.DrawRectangle(XPens.Black, new XRect(margin + (page.Width - (2 * margin)) / 2, currentY, (page.Width - (2 * margin)) / 2, cellHeight));
                gfx.DrawString(appointment.Status.ToString(), font, XBrushes.Black,
                    new XRect(margin + (page.Width - (2 * margin)) / 2, currentY, (page.Width - (2 * margin)) / 2, cellHeight), XStringFormats.Center);

                currentY += cellHeight + 20;
                gfx.DrawString("Thank you for choosing our service!", font, XBrushes.Black,
                    new XRect(0, currentY, page.Width, 20), XStringFormats.TopCenter);

                pdfDocument.Save(memoryStream, false);
                return memoryStream.ToArray();
            }
        }

    }

}
