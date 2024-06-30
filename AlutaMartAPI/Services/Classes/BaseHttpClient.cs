using System.Text;
using AlutaMartAPI.Utilities;

namespace AlutaMartAPI.Services;

public class BaseHttpClient(IHttpClientFactory _httpClientFactory) : IBaseHttpClient
{
    public async Task<string> PostFormEncodedAsync(string baseUrl, Dictionary<string, string> postData, Dictionary<string, string> headers, string endPointUrl = null)
	{
		using var client = _httpClientFactory.CreateClient();
		
		if(headers is not null) foreach(var header in headers) client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);

		client.DefaultRequestHeaders.TryAddWithoutValidation("Content-Type", "application/x-www-form-urlencoded");

		endPointUrl = endPointUrl is not null ? baseUrl+endPointUrl : baseUrl;
		using var request = new HttpRequestMessage(HttpMethod.Post, endPointUrl);
		request.Content = new FormUrlEncodedContent(postData);

		using var response = await client.SendAsync(request);
		return await response.Content.ReadAsStringAsync();
	}

	public async Task<T> PostAsync<T>(string baseUrl, object postData, Dictionary<string, string> headers, string endPointUrl = null)
	{
		using var client = _httpClientFactory.CreateClient();

		if(headers is not null) foreach(var header in headers) client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);

		endPointUrl = endPointUrl is not null ? baseUrl+endPointUrl : baseUrl;
		using var request = new HttpRequestMessage(HttpMethod.Post, endPointUrl);
		request.Content = new StringContent(postData.ToJson(), Encoding.UTF8, "application/json");

		using var httpResponse = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
		return await AppUtilities.DeserializeRequestAsync<T>(httpResponse);
	}

	public async Task<string> PostAsync(string baseUrl, object postData, Dictionary<string, string> headers, string endPointUrl = null)
	{
		using var client = _httpClientFactory.CreateClient();

		if(headers is not null) foreach(var header in headers) client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);

		endPointUrl = endPointUrl is not null ? baseUrl+endPointUrl : baseUrl;
		using var request = new HttpRequestMessage(HttpMethod.Post, endPointUrl);
		request.Content = new StringContent(postData.ToJson(), Encoding.UTF8, "application/json");

		using var httpResponse = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
		return await httpResponse.Content.ReadAsStringAsync();
	}

	public async Task<T> GetAsync<T>(string baseUrl, Dictionary<string, string> headers, string endPointUrl = null)
    {
        using var client = _httpClientFactory.CreateClient();
        
		if(headers is not null) foreach(var header in headers) client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);

        endPointUrl = endPointUrl is not null ? baseUrl+endPointUrl : baseUrl;
        using var request = new HttpRequestMessage(HttpMethod.Get, endPointUrl);

        using var httpResponse = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        return await AppUtilities.DeserializeRequestAsync<T>(httpResponse);
    }

	// public async Task<string> GetStreamAsync(string endPointUrl)
    // {
    //     using var client = _httpClientFactory.CreateClient();
        
    //     using var request = new HttpRequestMessage(HttpMethod.Get, endPointUrl);

    //     using var httpResponse = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
        
	// 	using var pdfStream = await httpResponse.Content.ReadAsStreamAsync();
    //     using var pdfReader = new PdfReader(pdfStream);
    //     using var pdfDocument = new PdfDocument(pdfReader);
    //     var text = new StringBuilder();

    //     for (int i = 1; i <= pdfDocument.GetNumberOfPages(); i++)
    //     {
    //         text.Append(PdfTextExtractor.GetTextFromPage(pdfDocument.GetPage(i)));
    //     }
    // }
}
