using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Net.Mail;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace AlutaMartAPI.Utilities;

public static class AppUtilities
{
	private static readonly JsonSerializerOptions jsonOptions = new()
	{
		PropertyNameCaseInsensitive = true,
		PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
		IgnoreReadOnlyProperties = false,
		DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
	};

	public static List<T> ShuffleList<T>(this List<T> inputList)
	{
		T[] array = [.. inputList];
        Random.Shared.Shuffle(array.AsSpan());
        return [.. array];
	}

	public static bool IsValidEmail(this string email)
	{
		try
		{
			if (email.Contains('#') || email.Contains('^') || email.Contains('&')
				|| email.Contains('%') || email.Contains('$') || email.Contains('*'))
			{
				return false;
			}

			if (new MailAddress(email).Address != email) return false;

			var indexOfAt = email.Split('@')[1];
			if (!indexOfAt.Contains('.')) return false;
			var indexOfDot = indexOfAt.Split('.')[1];
			return !string.IsNullOrEmpty(indexOfDot);
		}
		catch
		{
			return false;
		}
	}

	public static bool IsValidImage(this string imageType)
    {
        var acceptedImageType = new List<string>
        {
            "image/jpg",
            "image/jpeg",
            "image/png"
        };
        return acceptedImageType.Any(x => x == imageType);
    }

	public static bool IsValidVideoType(this string videoType)
    {
        var acceptedImageType = new List<string>
        {
            "video/mp4",
            "video/mov",
            "video/m4v",
			"video/mkv"
        };
        return acceptedImageType.Any(x => x == videoType);
    }

	public static bool IsValidPDF(this string fileName)
	{
		return fileName.Equals(".pdf", StringComparison.OrdinalIgnoreCase);
	}

	public static long RandomLong(int requiredLenght = 10, bool appendTime = false, bool appendDate = false)
	{
		requiredLenght = requiredLenght > 19 ? 19 : requiredLenght;
		var outputString = "1";
		outputString = appendTime ? $"1{DateTime.Now:hhmmssyyMMdd}" : outputString;
		outputString = appendDate && !appendTime ? $"1{DateTime.Now:yyMMdd}" : outputString;
		while(outputString.Length < requiredLenght) outputString = $"{outputString}{new Random().Next(1, int.MaxValue)}";
		if(outputString.Length > requiredLenght) outputString = outputString.Remove(requiredLenght);
		return Convert.ToInt64(outputString);
	}

	public static int RandomInt(int requiredLenght = 10, bool appendTime = false)
	{
		requiredLenght = requiredLenght > 10 ? 10 : requiredLenght;
		var outputString = "1";
		outputString = appendTime ? $"1{DateTime.Now:hhmmss}" : outputString;
		while(outputString.Length < requiredLenght) outputString = $"{outputString}{new Random().Next(1, int.MaxValue)}";
		if(outputString.Length > requiredLenght) outputString = outputString.Remove(requiredLenght);
		return Convert.ToInt32(outputString);
	}

	public static int LongCount(this long input)
    {
        if (input >= 0)
        {
            if (input < 10L) return 1;
            if (input < 100L) return 2;
            if (input < 1000L) return 3;
            if (input < 10000L) return 4;
            if (input < 100000L) return 5;
            if (input < 1000000L) return 6;
            if (input < 10000000L) return 7;
            if (input < 100000000L) return 8;
            if (input < 1000000000L) return 9;
            if (input < 10000000000L) return 10;
            if (input < 100000000000L) return 11;
            if (input < 1000000000000L) return 12;
            if (input < 10000000000000L) return 13;
            if (input < 100000000000000L) return 14;
            if (input < 1000000000000000L) return 15;
            if (input < 10000000000000000L) return 16;
            if (input < 100000000000000000L) return 17;
            if (input < 1000000000000000000L) return 18;
            return 19;
        }
        return 0;
    }

	public static int IntCount(this int input)
    {
        if (input >= 0)
        {
            if (input < 10L) return 1;
            if (input < 100L) return 2;
            if (input < 1000L) return 3;
            if (input < 10000L) return 4;
            if (input < 100000L) return 5;
            if (input < 1000000L) return 6;
            if (input < 10000000L) return 7;
            if (input < 100000000L) return 8;
            if (input < 1000000000L) return 9;
            return 10;
        }
        return 0;
    }

 	public static bool IsAllDigits(this string s) => decimal.TryParse(s, out decimal number) && number > 0;

	public static async Task<T> DeserializeRequestAsync<T>(HttpResponseMessage response)
	{
		using var contentStream = await response.Content.ReadAsStreamAsync();
		return await JsonSerializer.DeserializeAsync<T>(contentStream, jsonOptions);
	}

    public static string ToJson(this object input) => JsonSerializer.Serialize(input, jsonOptions);
    public static T FromJson<T>(this string input) => JsonSerializer.Deserialize<T>(input, jsonOptions);


	#pragma warning disable SYSLIB1045 // Template should be a static expression
	public static string TrimAllSpace(this string input) => Regex.Replace(input, @"\s+", "");

	public static void Forget(this Task task)
	{
		if (!task.IsCompleted || task.IsFaulted) _ = ForgetAwaited(task);
		async static Task ForgetAwaited(Task task) => await task;
	}

	public static string Name(this Enum e)
	{
		try
		{
			var attributes = (DisplayAttribute[])e.GetType().GetField(e.ToString()).GetCustomAttributes(typeof(DisplayAttribute), false);
			return attributes.Length > 0 ? attributes[0].Name : e.ToString();
		}
		catch
		{
			return "N/A";
		}
	}

	public static string ToTitleCase(this string input)
	{
		return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(input);
	}

	public static string GetDesignation(Roles role)
    {
        return role switch
        {
            Roles.SuperAdmin => "IT",
            Roles.PlatformManager => "Support",
            Roles.BusinessManager => "Management",
            Roles.AdminUser => "Admin User",
            _ => "Unknown"
        };
    }
}
