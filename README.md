# Appointment Management System

Appointment Management System is a web application built using ASP .NET Core MVC, designed to simplify the process of scheduling and managing appointments for businesses or organisations.

## Key Features

- **User Authentication:** Secure login and account creation for administrators and customers, with password recovery functionality.
- **Appointment Management:** Schedule, edit, and delete appointments, with automatic removal of expired appointments and archiving of past appointments.
- **User Roles:** Administrators have full access, while customers have restricted access tailored to their appointment needs.
- **Automated SMS Notifications:** Real-time notifications for appointment actions, with a text file-based log for easy tracking.
- **System Administration:** Comprehensive CRUD functionality for managing users and appointments, with the option to create additional admins.

## Technologies Used

- ASP .NET Core MVC
- Relational Database Management
- Automated SMS Notification Integration

## Usage

1. Clone the repository to your local machine.
2. Ensure ASP .NET Core MVC is installed.
3. Set up the database according to the provided schema.
4. Run the application and navigate to the appropriate URLs for admin or customer functionalities.

## Contributors

- **Anish**
- **Moeez**
- **Tom**
- **Riya**


##
There was another repository that was utilised during the creation of this app, where we constructed proof of concepts before integrating them into our project. Please see the following repositories:
- https://github.com/aak2510/AppointmentManagementSystemPOC.git

## Assumptions

- Application runs off of local system time (this impacts the auto-deletion of expired appointments).
- SMS is sent to a text file `SmsLog.txt` in the project directory.

## Start-up guide

Upon downloading the application, you will need to run two commands within the Package Manager Console:

- `update-database -Context AccountDbContext`
- `update-database -Context AppointmentDbContext`

After doing this, the application should open without issue. There are some initial accounts seeded into the database upon initialization.

**Admin account**
- Email: admin@admin.com
- Password: Secure@1

**User accounts**
1. - Email: test@test.com
   - Password: Secure@2

2. - Email: email@email.com
   - Password: Secure@3

## License

This project is licensed under the MIT License. See the [LICENSE.txt](LICENSE.txt) file for details.
