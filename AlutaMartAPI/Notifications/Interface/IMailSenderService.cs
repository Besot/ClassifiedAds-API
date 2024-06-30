namespace AlutaMartAPI.Services;
public interface IMailSenderService
{
    Task SendByPostMarkAppAsync(string message, string to, string subject);
}