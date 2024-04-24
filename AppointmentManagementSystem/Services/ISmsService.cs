using AppointmentManagementSystem.Models;

namespace AppointmentManagementSystem.Services
{
    public interface ISmsService
    {
        void StoreAppointmentsDue();
        void SendSms(string sMessage);
    }
}