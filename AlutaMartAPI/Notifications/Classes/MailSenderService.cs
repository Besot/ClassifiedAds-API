using AlutaMartAPI.DTOs;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;

public class MailSenderService(IBaseHttpClient httpClient, ILoggerFactory logger) : IMailSenderService
{
    private readonly IBaseHttpClient _httpClient = httpClient;
    private readonly ILogger _logger = logger.CreateLogger("Email-Service");

    public async Task SendByPostMarkAppAsync(string message, string to, string subject)
    {
        var model = new PostMarkAppSenderDTO
        {
            From = "notifications@alutamart.co",
            To = to,
            MessageStream = "outbound",
            HtmlBody = message,
            Subject = subject
        };

        _logger.LogInformation("sent to postmark {data}", model.ToJson());
        var headers = new Dictionary<string, string>{ { "X-Postmark-Server-Token", Constants.PostMarkToken } };

        var response = await _httpClient.PostAsync<PostMarkAppResponseDTO>(Constants.PostMarkBaseURL, model, headers);
        if(response.ErrorCode == 0) _logger.LogInformation(message: "email with subject: {subject} sent successfully to {email}", subject, to);
        else _logger.LogInformation(message: "failed to send email with subject: {subject} to {email}: error {error}", subject, to, response.ToJson());
    }
}