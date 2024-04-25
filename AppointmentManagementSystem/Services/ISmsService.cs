namespace AppointmentManagementSystem.Services
{
    public interface ISmsService
    {
        #region Methods
        void StoreAppointmentsDue();
        void SendSms(string sMessage);
        #endregion
    }
}