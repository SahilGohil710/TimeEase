Project Overview: 
TimeEase is an Employee Management System designed to streamline attendance tracking, leave approvals, and secure authentication. It includes separate functionalities for Admins and Employees, with automated email notifications for key actions.
How It Works: 
Authentication & Password Reset: 
Upon visiting the system, users will see the Login Page.

Admins have their own login credentials.

New employees will receive a welcome email after being added to the system, but their password will initially be unset.

When a new employee tries to log in, they will see a message:
➝ "Your password has not been set yet. Please click on 'Forgot Password' to create your password."

Clicking Forgot Password sends a one-time secret key to the employee's email.

Employees must enter the secret key and create a new password before logging in.

Employee Features: 
View personal attendance records in the Self Attendance section.

Apply for leave through the Apply Leave screen.

Once a leave request is submitted, an email notification is sent to the HR/Admin for approval.

Admin Features: 
Add Employees: Admins can create new employee accounts via the Add Employee screen.

Leave Approval: Admins can approve or reject leave requests.

Approved leaves update the attendance records automatically, and employees receive an email notification about the decision.

Attendance Management: Admins can upload attendance records via Excel files.

Key Feature: 
✅ Single Session Login: An employee can only log in on one browser at a time. If they log in from another browser, their first session is automatically logged out.

Technologies Used
Backend: C# (ASP.NET MVC), SQL Server

Frontend: Razor View, HTML, CSS, JavaScript, jQuery, AJAX

Development Tools: Visual Studio
