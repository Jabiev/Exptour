using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace Exptour.Common.Helpers;

public static class Helper
{
    public static DateTime ToUAE(this DateTime obj)
        => TimeZoneInfo.ConvertTime(obj, TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time"));

    public static string ToSentenceCase(this string inputString)
    {
        if (string.IsNullOrEmpty(inputString))
            return string.Empty;

        inputString = inputString.ToLower();

        return char.ToUpper(inputString[0]) + inputString.Substring(1);
    }

    public static bool IsValidGuid(this string value)
    {
        return Guid.TryParse(value, out _);
    }
}

public static class CustomCoders
{
    public static string UrlEncode(this string value)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(value);
        return WebEncoders.Base64UrlEncode(bytes);
    }

    public static string UrlDecode(this string value)
    {
        byte[] bytes = WebEncoders.Base64UrlDecode(value);
        return Encoding.UTF8.GetString(bytes);
    }

    public static bool IsBase64UrlValid(this string value)
    {
        if (string.IsNullOrEmpty(value)) return false;

        try
        {
            byte[] checkBytes1 = WebEncoders.Base64UrlDecode(value);
            Encoding.UTF8.GetString(checkBytes1);

            byte[] checkBytes2 = Encoding.UTF8.GetBytes(value);
            WebEncoders.Base64UrlEncode(checkBytes2);

            return true;
        }
        catch
        {
            return false;
        }
    }
}
