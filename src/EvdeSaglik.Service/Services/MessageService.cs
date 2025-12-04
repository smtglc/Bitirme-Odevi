using EvdeSaglik.Entity.Entities;
using EvdeSaglik.Service.DTOs.Message;
using EvdeSaglik.Service.Interfaces;
using EvdeSaglik.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace EvdeSaglik.Service.Services;

public class MessageService : IMessageService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly UserManager<User> _userManager;

    public MessageService(IUnitOfWork unitOfWork, UserManager<User> userManager)
    {
        _unitOfWork = unitOfWork;
        _userManager = userManager;
    }

    public async Task<List<MessageDto>> GetUserMessagesAsync(Guid userId)
    {
        var messages = await _unitOfWork.Messages.GetUserMessagesAsync(userId);
        
        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            SenderName = m.Sender.FirstName + " " + m.Sender.LastName,
            ReceiverId = m.ReceiverId,
            ReceiverName = m.Receiver.FirstName + " " + m.Receiver.LastName,
            Content = m.Content,
            IsRead = m.IsRead,
            CreatedAt = m.CreatedAt,
            ServiceId = m.ServiceId,
            ServiceName = m.Service?.Name
        }).ToList();
    }

    public async Task<List<MessageDto>> GetConversationAsync(Guid userId, Guid otherUserId)
    {
        var messages = await _unitOfWork.Messages.GetConversationAsync(userId, otherUserId);
        
        return messages.Select(m => new MessageDto
        {
            Id = m.Id,
            SenderId = m.SenderId,
            SenderName = m.Sender.FirstName + " " + m.Sender.LastName,
            ReceiverId = m.ReceiverId,
            ReceiverName = m.Receiver.FirstName + " " + m.Receiver.LastName,
            Content = m.Content,
            IsRead = m.IsRead,
            CreatedAt = m.CreatedAt,
            ServiceId = m.ServiceId,
            ServiceName = m.Service?.Name
        }).ToList();
    }

    public async Task<MessageDto> SendMessageAsync(Guid senderId, SendMessageDto dto)
    {
        
        var patient = await _unitOfWork.Patients.GetByUserIdAsync(senderId);
        var doctor = await _unitOfWork.Doctors.GetByUserIdAsync(senderId);
        
      
        if (patient == null && doctor == null)
        {
            var user = await _userManager.FindByIdAsync(senderId.ToString());
            if (user == null)
            {
                throw new Exception("User not found");
            }

            var roles = await _userManager.GetRolesAsync(user);
            var userRole = roles.FirstOrDefault();

            if (userRole?.Equals("Doctor", StringComparison.OrdinalIgnoreCase) == true)
            {
                
                doctor = new Doctor
                {
                    UserId = user.Id,
                    Specialization = "General",
                    LicenseNumber = "",
                    YearsOfExperience = 0,
                    HourlyRate = 0,
                    IsApproved = false
                };
                await _unitOfWork.Doctors.AddAsync(doctor);
                await _unitOfWork.SaveChangesAsync();
            }
            else if (userRole?.Equals("Patient", StringComparison.OrdinalIgnoreCase) == true)
            {
              
                patient = new Patient
                {
                    UserId = user.Id
                };
                await _unitOfWork.Patients.AddAsync(patient);
                await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                throw new Exception("User must have either Patient or Doctor role");
            }
        }

        var message = new Message
        {
            SenderId = senderId,
            ReceiverId = dto.ReceiverId,
            Content = dto.Content,
            ServiceId = dto.ServiceId,
            IsRead = false
        };

        await _unitOfWork.Messages.AddAsync(message);
        await _unitOfWork.SaveChangesAsync();

       
        var createdMessage = await _unitOfWork.Messages.GetUserMessagesAsync(senderId);
        var latestMessage = createdMessage.FirstOrDefault(m => m.Id == message.Id);

        if (latestMessage == null)
            throw new Exception("Message not found after creation");

        return new MessageDto
        {
            Id = latestMessage.Id,
            SenderId = latestMessage.SenderId,
            SenderName = latestMessage.Sender.FirstName + " " + latestMessage.Sender.LastName,
            ReceiverId = latestMessage.ReceiverId,
            ReceiverName = latestMessage.Receiver.FirstName + " " + latestMessage.Receiver.LastName,
            Content = latestMessage.Content,
            IsRead = latestMessage.IsRead,
            CreatedAt = latestMessage.CreatedAt,
            ServiceId = latestMessage.ServiceId,
            ServiceName = latestMessage.Service?.Name
        };
    }

    public async Task MarkAsReadAsync(Guid messageId)
    {
        var message = await _unitOfWork.Messages.GetByIdAsync(messageId);
        if (message != null)
        {
            message.IsRead = true;
            message.ReadAt = DateTime.UtcNow;
            _unitOfWork.Messages.Update(message);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
