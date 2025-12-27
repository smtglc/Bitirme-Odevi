using EvdeSaglik.Entity.Entities;
using EvdeSaglik.Entity.Enums;
using EvdeSaglik.Repositories.Interfaces;
using EvdeSaglik.Service.DTOs.Chat;
using EvdeSaglik.Service.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace EvdeSaglik.Service.Services;

public class ChatService : IChatService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly string _apiKey;
    private readonly string _model;
    private readonly HttpClient _httpClient;
    private const string OPENROUTER_API_URL = "https://openrouter.ai/api/v1/chat/completions";

    public ChatService(IUnitOfWork unitOfWork, IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _unitOfWork = unitOfWork;
        _apiKey = configuration["OpenRouter:ApiKey"] ?? throw new Exception("OpenRouter API key not configured");
        _model = configuration["OpenRouter:Model"] ?? "deepseek/deepseek-r1-0528:free";
        _httpClient = httpClientFactory.CreateClient();
    }

    public async Task<ChatResponseDto> GetChatResponseAsync(Guid userId, ChatRequestDto request)
    {
        try
        {
            // Determine user role
            var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
            var isDoctor = doctor != null;
            
            var contextInfo = await BuildContextAsync(userId, request.Message);
            
            string systemMessage;
            if (isDoctor)
            {
                systemMessage = $@"Sen bir doktor asistanısın. Doktorlara randevuları, hastaları ve hizmetleri hakkında bilgi veriyorsun.
Kullanıcı bir DOKTOR'dur. Ona 'Sayın Doktor' diye hitap et.
Türkçe konuş, kibar ve profesyonel ol. Kısa ve öz cevaplar ver.

{contextInfo}";
            }
            else
            {
                systemMessage = $@"Sen bir hasta asistanısın. Hastalara randevuları, doktorları ve hizmetler hakkında bilgi veriyorsun.
Kullanıcı bir HASTA'dır. Ona adıyla hitap edebilirsin.
Türkçe konuş, kibar ve profesyonel ol. Kısa ve öz cevaplar ver.

{contextInfo}";
            }

            var messages = new List<object>
            {
                new { role = "system", content = systemMessage },
                new { role = "user", content = request.Message }
            };

            var openRouterRequest = new
            {
                model = _model,
                messages = messages
                // Tools removed - DeepSeek doesn't support function calling
            };

            var jsonContent = JsonSerializer.Serialize(openRouterRequest);
            var content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

            Console.WriteLine($"=== OPENROUTER REQUEST ===");
            Console.WriteLine($"Model: {_model}");
            Console.WriteLine($"Request: {jsonContent}");

            var httpRequest = new HttpRequestMessage(HttpMethod.Post, OPENROUTER_API_URL);
            httpRequest.Headers.Add("Authorization", $"Bearer {_apiKey}");
            httpRequest.Headers.Add("HTTP-Referer", "http://localhost:5013"); // Required by OpenRouter
            httpRequest.Content = content;

            var response = await _httpClient.SendAsync(httpRequest);
            var responseContent = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"=== OPENROUTER RESPONSE ===");
            Console.WriteLine($"Status: {response.StatusCode}");
            Console.WriteLine($"Response: {responseContent}");

            if (!response.IsSuccessStatusCode)
            {
                return new ChatResponseDto
                {
                    Message = $"API Hatası: {response.StatusCode}",
                    Timestamp = DateTime.UtcNow
                };
            }

            var openRouterResponse = JsonSerializer.Deserialize<OpenRouterResponse>(responseContent);
            var choice = openRouterResponse?.Choices?.FirstOrDefault();

            if (choice == null)
            {
                return new ChatResponseDto
                {
                    Message = "Yanıt alınamadı.",
                    Timestamp = DateTime.UtcNow
                };
            }

            // Check if AI wants to call a function
            if (choice.Message?.ToolCalls != null && choice.Message.ToolCalls.Any())
            {
                Console.WriteLine($"=== FUNCTION CALL DETECTED ===");
                
                foreach (var toolCall in choice.Message.ToolCalls)
                {
                    Console.WriteLine($"Function: {toolCall.Function?.Name}");
                    Console.WriteLine($"Arguments: {toolCall.Function?.Arguments}");

                    var functionResult = await ExecuteFunctionAsync(userId, 
                        toolCall.Function?.Name ?? "", 
                        toolCall.Function?.Arguments ?? "{}");

                    Console.WriteLine($"Function Result: {functionResult}");

                    // Add function result to messages
                    messages.Add(new { role = "assistant", content = choice.Message.Content ?? "", tool_calls = choice.Message.ToolCalls });
                    messages.Add(new { role = "tool", tool_call_id = toolCall.Id, content = functionResult });
                }

                // Get final response from AI with function results
                var finalRequest = new
                {
                    model = _model,
                    messages = messages
                };

                var finalJsonContent = JsonSerializer.Serialize(finalRequest);
                var finalContent = new StringContent(finalJsonContent, Encoding.UTF8, "application/json");

                var finalHttpRequest = new HttpRequestMessage(HttpMethod.Post, OPENROUTER_API_URL);
                finalHttpRequest.Headers.Add("Authorization", $"Bearer {_apiKey}");
                finalHttpRequest.Headers.Add("HTTP-Referer", "http://localhost:5013");
                finalHttpRequest.Content = finalContent;

                var finalResponse = await _httpClient.SendAsync(finalHttpRequest);
                var finalResponseContent = await finalResponse.Content.ReadAsStringAsync();

                var finalOpenRouterResponse = JsonSerializer.Deserialize<OpenRouterResponse>(finalResponseContent);
                var finalText = finalOpenRouterResponse?.Choices?.FirstOrDefault()?.Message?.Content ?? "İşlem tamamlandı.";

                return new ChatResponseDto
                {
                    Message = finalText,
                    Timestamp = DateTime.UtcNow
                };
            }

            // Normal response without function call
            var responseText = choice.Message?.Content ?? "Yanıt oluşturulamadı.";

            return new ChatResponseDto
            {
                Message = responseText,
                Timestamp = DateTime.UtcNow
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ChatService Exception: {ex.Message}");
            Console.WriteLine($"Stack Trace: {ex.StackTrace}");

            return new ChatResponseDto
            {
                Message = $"Hata: {ex.Message}",
                Timestamp = DateTime.UtcNow
            };
        }
    }

    private object[] GetToolDefinitions()
    {
        return new[]
        {
            new
            {
                type = "function",
                function = new
                {
                    name = "create_service",
                    description = "Yeni bir hizmet oluşturur. Kullanıcı 'yeni hizmet oluştur', 'hizmet ekle' gibi ifadeler kullandığında bu fonksiyonu çağır.",
                    parameters = new
                    {
                        type = "object",
                        properties = new
                        {
                            name = new { type = "string", description = "Hizmet adı" },
                            description = new { type = "string", description = "Hizmet açıklaması (opsiyonel)" },
                            basePrice = new { type = "number", description = "Hizmet ücreti (TL cinsinden)" },
                            durationMinutes = new { type = "integer", description = "Hizmet süresi dakika cinsinden (varsayılan: 60)" },
                            specialization = new { type = "string", description = "Uzmanlık alanı (örn: Psikiyatri, Kardiyoloji)" }
                        },
                        required = new[] { "name", "basePrice", "specialization" }
                    }
                }
            }
        };
    }

    private async Task<string> ExecuteFunctionAsync(Guid userId, string functionName, string argumentsJson)
    {
        try
        {
            switch (functionName)
            {
                case "create_service":
                    return await CreateServiceAsync(userId, argumentsJson);
                default:
                    return $"Bilinmeyen fonksiyon: {functionName}";
            }
        }
        catch (Exception ex)
        {
            return $"Fonksiyon hatası: {ex.Message}";
        }
    }

    private async Task<string> CreateServiceAsync(Guid userId, string argumentsJson)
    {
        try
        {
            var args = JsonDocument.Parse(argumentsJson).RootElement;

            var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
            if (doctor == null)
            {
                return "Doktor bulunamadı.";
            }

            var service = new Entity.Entities.Service
            {
                DoctorId = doctor.Id,
                Name = args.GetProperty("name").GetString() ?? "",
                Description = args.TryGetProperty("description", out var desc) ? desc.GetString() ?? "" : "",
                BasePrice = args.GetProperty("basePrice").GetDecimal(),
                DurationMinutes = args.TryGetProperty("durationMinutes", out var dur) ? dur.GetInt32() : 60,
                Specialization = ParseMedicalSpecialization(args.GetProperty("specialization").GetString() ?? "Other"),
                IsActive = true
            };

            await _unitOfWork.Services.AddAsync(service);
            await _unitOfWork.SaveChangesAsync();

            return $"✅ '{service.Name}' hizmeti {service.BasePrice} TL ile başarıyla oluşturuldu.";
        }
        catch (Exception ex)
        {
            return $"Hizmet oluşturma hatası: {ex.Message}";
        }
    }

    private MedicalSpecialization ParseMedicalSpecialization(string spec)
    {
        return spec.ToLower() switch
        {
            "psikiyatri" or "psychiatry" => MedicalSpecialization.Psychiatry,
            "kardiyoloji" or "cardiology" => MedicalSpecialization.Cardiology,
            "nöroloji" or "neurology" => MedicalSpecialization.Neurology,
            "ortopedi" or "orthopedics" => MedicalSpecialization.Orthopedics,
            "genel cerrahi" or "general surgery" => MedicalSpecialization.GeneralSurgery,
            "fizik tedavi" or "physical therapy" => MedicalSpecialization.PhysicalTherapy,
            "hemşirelik" or "nursing" => MedicalSpecialization.NursingServices,
            "laboratuvar" or "laboratory" => MedicalSpecialization.LaboratoryServices,
            "dermatoloji" or "dermatology" => MedicalSpecialization.Dermatology,
            "pediatri" or "pediatrics" => MedicalSpecialization.Pediatrics,
            "jinekoloji" or "gynecology" => MedicalSpecialization.Gynecology,
            _ => MedicalSpecialization.Other
        };
    }

    private async Task<string> BuildContextAsync(Guid userId, string message)
    {
        var messageLower = message.ToLower();
        var contextInfo = new StringBuilder();

        try
        {
            // Check if user is a doctor
            var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(userId);
            if (doctor != null)
            {
                return await BuildDoctorContextAsync(doctor, messageLower);
            }

            // Check if user is a patient
            var patient = await _unitOfWork.Patients.GetByUserIdAsync(userId);
            if (patient != null)
            {
                return await BuildPatientContextAsync(patient, messageLower);
            }
        }
        catch
        {
            // Return empty context on error
        }

        return contextInfo.ToString();
    }

    private async Task<string> BuildDoctorContextAsync(Doctor doctor, string messageLower)
    {
        var contextInfo = new StringBuilder();

        if (messageLower.Contains("randevu") || messageLower.Contains("appointment") ||
            messageLower.Contains("bugün") || messageLower.Contains("yarın") || messageLower.Contains("yakın") ||
            messageLower.Contains("geçmiş") || messageLower.Contains("tamamlanan") || messageLower.Contains("bekleyen"))
        {
            var appointments = await _unitOfWork.Appointments.GetByDoctorIdAsync(doctor.Id);
            var upcomingAppointments = appointments
                .Where(a => a.ScheduledDateTime >= DateTime.UtcNow && a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.ScheduledDateTime)
                .ToList();

            contextInfo.AppendLine("\n--- RANDEVU BİLGİLERİ ---");
            contextInfo.AppendLine($"Toplam randevu: {appointments.Count()}");
            contextInfo.AppendLine($"Yaklaşan randevu: {upcomingAppointments.Count}");

            if (upcomingAppointments.Any())
            {
                var nextAppointment = upcomingAppointments.First();
                contextInfo.AppendLine($"En yakın randevu: {nextAppointment.ScheduledDateTime:dd.MM.yyyy HH:mm} - {nextAppointment.Patient.User.FirstName} {nextAppointment.Patient.User.LastName}");
            }

            var todayAppointments = upcomingAppointments
                .Where(a => a.ScheduledDateTime.Date == DateTime.UtcNow.Date)
                .ToList();
            contextInfo.AppendLine($"Bugünkü randevu: {todayAppointments.Count}");

            if (todayAppointments.Any())
            {
                contextInfo.AppendLine("Bugünkü randevular:");
                foreach (var apt in todayAppointments.Take(5))
                {
                    contextInfo.AppendLine($"  - {apt.ScheduledDateTime:HH:mm} {apt.Patient.User.FirstName} {apt.Patient.User.LastName} ({apt.Service.Name})");
                }
            }

            // Pending appointments
            var pendingAppointments = appointments
                .Where(a => a.Status == AppointmentStatus.Pending)
                .OrderBy(a => a.ScheduledDateTime)
                .Take(5)
                .ToList();

            if (pendingAppointments.Any())
            {
                contextInfo.AppendLine($"\nOnay Bekleyen Randevular ({pendingAppointments.Count}):");
                foreach (var apt in pendingAppointments)
                {
                    contextInfo.AppendLine($"  - {apt.ScheduledDateTime:dd.MM.yyyy HH:mm} - {apt.Patient.User.FirstName} {apt.Patient.User.LastName} ({apt.Service.Name})");
                }
            }

            // Completed appointments
            var completedAppointments = appointments
                .Where(a => a.Status == AppointmentStatus.Completed)
                .OrderByDescending(a => a.ScheduledDateTime)
                .Take(5)
                .ToList();

            if (completedAppointments.Any())
            {
                contextInfo.AppendLine($"\nTamamlanan Randevular (son {completedAppointments.Count}):");
                foreach (var apt in completedAppointments)
                {
                    contextInfo.AppendLine($"  - {apt.ScheduledDateTime:dd.MM.yyyy HH:mm} - {apt.Patient.User.FirstName} {apt.Patient.User.LastName} ({apt.Service.Name})");
                }
            }

            // Cancelled appointments
            var cancelledAppointments = appointments
                .Where(a => a.Status == AppointmentStatus.Cancelled)
                .OrderByDescending(a => a.ScheduledDateTime)
                .Take(3)
                .ToList();

            if (cancelledAppointments.Any())
            {
                contextInfo.AppendLine($"\nİptal Edilen Randevular (son {cancelledAppointments.Count}):");
                foreach (var apt in cancelledAppointments)
                {
                    contextInfo.AppendLine($"  - {apt.ScheduledDateTime:dd.MM.yyyy HH:mm} - {apt.Patient.User.FirstName} {apt.Patient.User.LastName} ({apt.Service.Name})");
                }
            }
        }

        if (messageLower.Contains("hizmet") || messageLower.Contains("service"))
        {
            var services = await _unitOfWork.Services.GetServicesByDoctorIdAsync(doctor.Id);
            contextInfo.AppendLine("\n--- HİZMET BİLGİLERİ ---");
            contextInfo.AppendLine($"Toplam hizmet: {services.Count()}");
            contextInfo.AppendLine($"Aktif hizmet: {services.Count(s => s.IsActive)}");

            if (services.Any())
            {
                contextInfo.AppendLine("Hizmetler:");
                foreach (var service in services.Take(5))
                {
                    contextInfo.AppendLine($"  - {service.Name} ({service.Specialization}) - {service.BasePrice} TL");
                }
            }
        }

        return contextInfo.ToString();
    }

    private async Task<string> BuildPatientContextAsync(Patient patient, string messageLower)
    {
        var contextInfo = new StringBuilder();

        if (messageLower.Contains("randevu") || messageLower.Contains("appointment") ||
            messageLower.Contains("bugün") || messageLower.Contains("yarın") || messageLower.Contains("yakın") ||
            messageLower.Contains("geçmiş") || messageLower.Contains("tamamlanan"))
        {
            var appointments = await _unitOfWork.Appointments.GetByPatientIdAsync(patient.Id);
            var upcomingAppointments = appointments
                .Where(a => a.ScheduledDateTime >= DateTime.UtcNow && a.Status != AppointmentStatus.Cancelled)
                .OrderBy(a => a.ScheduledDateTime)
                .ToList();

            contextInfo.AppendLine("\n--- RANDEVU BİLGİLERİNİZ ---");
            contextInfo.AppendLine($"Toplam randevu: {appointments.Count()}");
            contextInfo.AppendLine($"Yaklaşan randevu: {upcomingAppointments.Count}");

            if (upcomingAppointments.Any())
            {
                var nextAppointment = upcomingAppointments.First();
                contextInfo.AppendLine($"En yakın randevu: {nextAppointment.ScheduledDateTime:dd.MM.yyyy HH:mm}");
                contextInfo.AppendLine($"Doktor: Dr. {nextAppointment.Doctor.User.FirstName} {nextAppointment.Doctor.User.LastName}");
                contextInfo.AppendLine($"Hizmet: {nextAppointment.Service.Name}");
                contextInfo.AppendLine($"Durum: {GetStatusText(nextAppointment.Status)}");
                contextInfo.AppendLine($"Ücret: {nextAppointment.TotalAmount} TL");
            }

            // Pending appointments
            var pendingAppointments = appointments
                .Where(a => a.Status == AppointmentStatus.Pending)
                .ToList();

            if (pendingAppointments.Any())
            {
                contextInfo.AppendLine($"\nOnay bekleyen randevu: {pendingAppointments.Count}");
            }

            // Today's appointments
            var todayAppointments = upcomingAppointments
                .Where(a => a.ScheduledDateTime.Date == DateTime.UtcNow.Date)
                .ToList();

            if (todayAppointments.Any())
            {
                contextInfo.AppendLine($"\nBugünkü randevu: {todayAppointments.Count}");
                foreach (var apt in todayAppointments)
                {
                    contextInfo.AppendLine($"  - {apt.ScheduledDateTime:HH:mm} Dr. {apt.Doctor.User.FirstName} {apt.Doctor.User.LastName} ({apt.Service.Name})");
                }
            }

            // Completed appointments
            var completedAppointments = appointments
                .Where(a => a.Status == AppointmentStatus.Completed)
                .OrderByDescending(a => a.ScheduledDateTime)
                .Take(5)
                .ToList();

            if (completedAppointments.Any())
            {
                contextInfo.AppendLine($"\nGeçmiş Randevular (son {completedAppointments.Count}):");
                foreach (var apt in completedAppointments)
                {
                    contextInfo.AppendLine($"  - {apt.ScheduledDateTime:dd.MM.yyyy HH:mm} - Dr. {apt.Doctor.User.FirstName} {apt.Doctor.User.LastName} ({apt.Doctor.Specialization}) - {apt.Service.Name}");
                }
            }
        }

        // Service search for patients
        if (messageLower.Contains("hizmet") || messageLower.Contains("service") ||
            messageLower.Contains("doktor") || messageLower.Contains("branş") ||
            messageLower.Contains("psikiyatri") || messageLower.Contains("kardiyoloji") ||
            messageLower.Contains("nöroloji") || messageLower.Contains("ortopedi") ||
            messageLower.Contains("fizik tedavi") || messageLower.Contains("pediatri"))
        {
            var activeServicesEnumerable = await _unitOfWork.Services.GetAllActiveServicesAsync();
            var activeServices = activeServicesEnumerable.ToList();

            if (activeServices.Any())
            {
                contextInfo.AppendLine("\n--- MEVCUT HİZMETLER ---");
                contextInfo.AppendLine($"Toplam aktif hizmet: {activeServices.Count}");

                var servicesBySpecialization = activeServices
                    .GroupBy(s => s.Specialization)
                    .OrderBy(g => g.Key)
                    .ToList();

                contextInfo.AppendLine("\nBranşlara göre hizmetler:");
                foreach (var group in servicesBySpecialization)
                {
                    var specializationText = GetSpecializationText(group.Key);
                    contextInfo.AppendLine($"\n{specializationText}:");
                    foreach (var service in group.Take(3))
                    {
                        contextInfo.AppendLine($"  - {service.Name} - Dr. {service.Doctor.User.FirstName} {service.Doctor.User.LastName} - {service.BasePrice} TL");
                    }
                }
            }
            else
            {
                contextInfo.AppendLine("\n--- HİZMET BİLGİSİ ---");
                contextInfo.AppendLine("Şu anda aktif hizmet bulunmamaktadır.");
            }
        }

        return contextInfo.ToString();
    }

    private string GetStatusText(AppointmentStatus status)
    {
        return status switch
        {
            AppointmentStatus.Pending => "Onay Bekliyor",
            AppointmentStatus.Confirmed => "Onaylandı",
            AppointmentStatus.Completed => "Tamamlandı",
            AppointmentStatus.Cancelled => "İptal Edildi",
            _ => status.ToString()
        };
    }

    private string GetSpecializationText(MedicalSpecialization specialization)
    {
        return specialization switch
        {
            MedicalSpecialization.GeneralMedicine => "Genel Tıp",
            MedicalSpecialization.Cardiology => "Kardiyoloji",
            MedicalSpecialization.Neurology => "Nöroloji",
            MedicalSpecialization.Orthopedics => "Ortopedi",
            MedicalSpecialization.GeneralSurgery => "Genel Cerrahi",
            MedicalSpecialization.PhysicalTherapy => "Fizik Tedavi",
            MedicalSpecialization.NursingServices => "Hemşirelik Hizmetleri",
            MedicalSpecialization.LaboratoryServices => "Laboratuvar Hizmetleri",
            MedicalSpecialization.Dermatology => "Dermatoloji",
            MedicalSpecialization.Pediatrics => "Pediatri",
            MedicalSpecialization.Gynecology => "Jinekoloji",
            MedicalSpecialization.Psychiatry => "Psikiyatri",
            _ => specialization.ToString()
        };
    }

    // OpenRouter API Response Models
    private class OpenRouterResponse
    {
        [JsonPropertyName("choices")]
        public List<Choice>? Choices { get; set; }
    }

    private class Choice
    {
        [JsonPropertyName("message")]
        public MessageResponse? Message { get; set; }
    }

    private class MessageResponse
    {
        [JsonPropertyName("content")]
        public string? Content { get; set; }

        [JsonPropertyName("tool_calls")]
        public List<ToolCall>? ToolCalls { get; set; }
    }

    private class ToolCall
    {
        [JsonPropertyName("id")]
        public string? Id { get; set; }

        [JsonPropertyName("type")]
        public string? Type { get; set; }

        [JsonPropertyName("function")]
        public FunctionCall? Function { get; set; }
    }

    private class FunctionCall
    {
        [JsonPropertyName("name")]
        public string? Name { get; set; }

        [JsonPropertyName("arguments")]
        public string? Arguments { get; set; }
    }
}
