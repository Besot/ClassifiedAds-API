namespace AlutaMartAPI.Services;

public interface IBaseHttpClient
{
	Task<T> PostAsync<T>(string baseUrl, object postdata, Dictionary<string, string> headers, string endPointUrl = null);
	Task<T> GetAsync<T>(string baseUrl, Dictionary<string, string> headers, string endPointUrl = null);
	Task<string> PostFormEncodedAsync(string baseUrl, Dictionary<string, string> postData, Dictionary<string, string> headers, string endPointUrl = null);
	Task<string> PostAsync(string baseUrl, object postData, Dictionary<string, string> headers, string endPointUrl = null);
}