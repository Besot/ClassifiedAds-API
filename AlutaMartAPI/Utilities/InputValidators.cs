using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace AlutaMartAPI.Utilities;

sealed public partial class AlphaNumericAttribute : ValidationAttribute
{
    public override bool IsValid(object value) => value is string input && IsAlphaNumeric(input);

    internal static bool IsAlphaNumeric(string srt)
    {
        if (string.IsNullOrEmpty(srt)) return false;

        srt = srt.TrimAllSpace();
        Regex regex = AlphaNumericRegex();
        return regex.IsMatch(srt);
    }

    [GeneratedRegex("^[a-zA-Z0-9]*$")]
    private static partial Regex AlphaNumericRegex();
}

sealed public partial class PhoneNumberValidatorAttribute : ValidationAttribute
{
    public override bool IsValid(object value) => value is string input && IsDigitsOnly(input) && IsValidPhoneNumber(input);
    
    internal static bool IsValidPhoneNumber(string str)
    {
        if (string.IsNullOrEmpty(str)) return false;
        str = str.TrimAllSpace();
        
        if (!str.StartsWith("0")) return false;
        str = string.Concat("+234", str.AsSpan(1));
        
        var validator = PhoneNumbers.PhoneNumberUtil.GetInstance();
        var phoneNumber = validator.Parse(str, "NG");
        return validator.IsValidNumber(phoneNumber);
    }

    static bool IsDigitsOnly(string str) => str.All(char.IsDigit);
}