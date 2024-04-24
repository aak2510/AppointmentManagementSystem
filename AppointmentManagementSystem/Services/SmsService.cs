using AppointmentManagementSystem.Areas.Identity.Data;
using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;

namespace AppointmentManagementSystem.Services
{
    public class SmsService : ISmsService
    {
        //Store the Accounts dataset
        private readonly AccountDbContext _accountDbContext;
        //Store the Appointments dataset
        private readonly AppointmentDbContext _appointmentDbContext;

        public SmsService(AccountDbContext accountDbContext, AppointmentDbContext appointmentDbContext)
        {
            _accountDbContext = accountDbContext;
            _appointmentDbContext = appointmentDbContext;
        }

        public void StoreAppointmentsDue()
        {
            //Store the number of hours that the sms notification needs to be sent before the appointment
            int iDurationBeforeAppt = 48;
            //Store the current time
            var vCurrentTime = DateTime.UtcNow;
            var startTime = vCurrentTime.AddHours(iDurationBeforeAppt);
            var endTime = startTime.AddHours(iDurationBeforeAppt); // Assuming the duration of the appointment is also 48 hours

            //Store the list of appointments that are due in iDurationBeforeAppt hours
            IEnumerable<Appointment> ieAppointments = _appointmentDbContext.appointments.ToList();

            Console.WriteLine("ieAppointments: " + ieAppointments.Count());

            foreach (Appointment oAppointment in ieAppointments)
            {
                string sMessage = CreateSms(oAppointment);
                SendSms(sMessage);
                /*if (oAppointment.AppointmentTime >= startTime && oAppointment.AppointmentTime <= endTime)
                {
                    string sMessage = CreateSms(oAppointment);
                    SendSms(sMessage);
                }*/
            }
        }

        public string CreateSms(Appointment oAppointment)
        {
            //Store the list of users
            List<AppUser> vUsers = _accountDbContext.Users.ToList();

            //Store the Appointment Date
            DateTime dtDate = oAppointment.AppointmentDate;
            //Store the Appointment Time
            DateTime dtTime = oAppointment.AppointmentTime;
            //Store the Appointment Subject
            string sSubject = oAppointment.AppointmentSubject;
            //Store the User Email
            string sEmail = oAppointment.UserEmail;
            //Using the email, fetch the necessary data from the AspNetUsers table in the Accounts database
            string sFirstName = "";
            string sLastName = "";
            string sPhoneNumber = "";
            //Create the message
            return $"Message Sent to {sPhoneNumber} on {DateTime.UtcNow}:\n" +
                   $"Dear {sFirstName} {sLastName}, you have an appointment on {dtDate} at {dtTime} for a {sSubject}";
        }

        public void SendSms(string sMessage)
        {
            Console.WriteLine("SendSms function in the SmsService class is invoked\n\n" + sMessage);
        }
    }
}
