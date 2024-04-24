using AppointmentManagementSystem.Areas.Identity.Data;
using AppointmentManagementSystem.Data;
using AppointmentManagementSystem.Models;
using System;

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
            //Store the number of hours that the SMS notification needs to be sent before the appointment
            int iDurationBeforeAppt = 48;

            //Store the list of appointments that are due in iDurationBeforeAppt hours
            IEnumerable<Appointment> ieAppointments = _appointmentDbContext.appointments.ToList();

            Console.WriteLine("ieAppointments: " + ieAppointments.Count());

            //Use a HashSet to track appointment times that have already been processed
            HashSet<DateTime> hsProcessedAppointmentTimes = new HashSet<DateTime>();

            /*foreach (Appointment oAppointment in ieAppointments)
            {
                //Check if the appointment time has already been processed
                if (!hsProcessedAppointmentTimes.Contains(oAppointment.AppointmentTime))
                {
                    //Mark the appointment time as processed
                    hsProcessedAppointmentTimes.Add(oAppointment.AppointmentTime);
                    //Send the SMS notification
                    SendSms(CreateSms(oAppointment));
                }
            }*/

            foreach (Appointment oAppointment in ieAppointments)
            {
                //Combine the Appointment Date and Appointment Time and store into a DateTime object
                DateTime dtAppointment = new DateTime(
                    oAppointment.AppointmentDate.Year,
                    oAppointment.AppointmentDate.Month,
                    oAppointment.AppointmentDate.Day,
                    oAppointment.AppointmentTime.Hour,
                    oAppointment.AppointmentTime.Minute,
                    oAppointment.AppointmentTime.Second
                );

                //Store the current time
                DateTime dtCurrentTime = DateTime.Now;
                //Calculate the difference
                TimeSpan tsDifference = dtAppointment - dtCurrentTime;
                //Calculate the difference in hours
                double dbAbsoluteDifference = Math.Abs(tsDifference.TotalHours);
                Console.WriteLine("Time Difference: " + tsDifference.Hours.ToString());
                //If the appointment is within the next 48 hours
                if (dbAbsoluteDifference <= iDurationBeforeAppt)
                {
                    //Create and send a SMS notification message to the user
                    SendSms(CreateSms(oAppointment));
                }
            }
        }

        public string CreateSms(Appointment oAppointment)
        {
            //Store the Appointment Date
            DateTime dtDate = oAppointment.AppointmentDate.Date;
            DateOnly doDate = new DateOnly(dtDate.Year, dtDate.Month, dtDate.Day);
            //Store the Appointment Time
            DateTime tsTime = oAppointment.AppointmentTime;
            //Store the Appointment Subject
            string sSubject = oAppointment.AppointmentSubject;
            //Store the User Email
            string sEmail = oAppointment.UserEmail;

            //IEnumerable<AppUser> ieUsers = _accountDbContext.Users.ToList();

            //Using the email, fetch the necessary data from the AspNetUsers table in the Accounts database
            var vUser = _accountDbContext.Users.FirstOrDefault(u => u.Email == sEmail);

            //Handle the case where no user is found with the associated email
            if (vUser == null) return $"No vUser found with email: {sEmail}";

            //Extract the user's first name, last name, and phone number
            string sFirstName = vUser.FirstName;
            string sLastName = vUser.LastName;
            string? sPhoneNumber = vUser.PhoneNumber;

            //Create the SMS message
            return $"Message sent to {sPhoneNumber} at {DateTime.Now}:\n" +
                   $"Hi {sFirstName} {sLastName}, you have an appointment on {doDate} at {tsTime.ToString("HH:mm")} for a {sSubject}. See you soon!\n";
        }

        public void SendSms(string sMessage)
        {
            Console.WriteLine("SendSms function in the SmsService class is invoked\n\n" + sMessage);

            //Specify the path to the log file
            string sLogFilePath = @"SmsLog.txt";

            try
            {
                //Append the message to the log file
                File.AppendAllText(sLogFilePath, sMessage + Environment.NewLine);
            }
            catch (Exception ex)
            {
                //Print to console if there is an exception
                Console.WriteLine($"Error writing to log file: {ex.Message}");
            }
        }
    }
}