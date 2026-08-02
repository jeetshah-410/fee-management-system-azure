using System.Threading.Tasks;
using FeeManagement.Domain.Entities;

namespace FeeManagement.Domain.Interfaces;

public interface INotificationService
{
    Task SendReminderAsync(Student student);
}
