namespace DentalBooking.Infrastructure.Templates;

public static class EmailTemplates
{
    public static string RegistrationConfirmation(string confirmationLink) =>
        $@"
        <h2>Welcome to DentalBooking!</h2>
        <p>Please confirm your registration by clicking the link below:</p>
        <p><a href='{confirmationLink}'>Confirm Email</a></p>
        ";

    public static string BookingConfirmation(string doctor, string procedure, DateTime startUtc) =>
        $@"
        <h2>Your booking is confirmed!</h2>
        <p><strong>Doctor:</strong> {doctor}</p>
        <p><strong>Procedure:</strong> {procedure}</p>
        <p><strong>Date:</strong> {startUtc:dd.MM.yyyy HH:mm}</p>
        <p>Thank you for choosing DentalBooking!</p>
        ";

    public static string Reminder(string doctor, string procedure, DateTime startUtc) =>
        $@"
        <h2>Appointment Reminder</h2>
        <p>This is a friendly reminder for your upcoming appointment:</p>
        <p><strong>Doctor:</strong> {doctor}</p>
        <p><strong>Procedure:</strong> {procedure}</p>
        <p><strong>Date:</strong> {startUtc:dd.MM.yyyy HH:mm}</p>
        <p>Please arrive 10 minutes earlier. Thank you!</p>
        ";

    public static string BookingCancelled(string doctor, string procedure, DateTime startUtc) =>
        $@"
        <h2>Your booking was cancelled</h2>
        <p><strong>Doctor:</strong> {doctor}</p>
        <p><strong>Procedure:</strong> {procedure}</p>
        <p><strong>Date:</strong> {startUtc:dd.MM.yyyy HH:mm}</p>
        <p>If this is a mistake, please create a new booking.</p>
        ";
}
