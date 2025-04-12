using Exptour.Common.Constants;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;
using System.Text.RegularExpressions;

namespace Exptour.Common.Helpers;

public static class Helper
{
    public static DateTime ToUAE(this DateTime obj)
        => TimeZoneInfo.ConvertTime(obj, TimeZoneInfo.FindSystemTimeZoneById("Arabian Standard Time"));

    public static DateTime ToUTC(this DateTime obj)
    {
        if (obj.Kind == DateTimeKind.Utc)
            return obj;

        return TimeZoneInfo.ConvertTime(obj, TimeZoneInfo.Utc);
    }

    public static string ToSentenceCase(this string inputString)
    {
        if (string.IsNullOrEmpty(inputString))
            return string.Empty;

        inputString = inputString.ToLower();

        return char.ToUpper(inputString[0]) + inputString.Substring(1);
    }

    public static string ToProperFullName(this string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return string.Empty;

        return string.Join(" ", fullName
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(word => char.ToUpper(word[0]) + word.Substring(1).ToLower()));
    }

    public static string ToProperFirstName(this string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return string.Empty;

        var firstName = fullName
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .FirstOrDefault();

        if (string.IsNullOrEmpty(firstName))
            return string.Empty;

        return char.ToUpper(firstName[0]) + firstName.Substring(1).ToLower();
    }

    public static string ToProperLastName(this string fullName)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            return string.Empty;

        var lastName = fullName
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .LastOrDefault();

        if (string.IsNullOrEmpty(lastName))
            return string.Empty;

        return char.ToUpper(lastName[0]) + lastName.Substring(1).ToLower();
    }

    public static string GetNameFromEmail(this string email)
    {
        if (string.IsNullOrWhiteSpace(email))
            return string.Empty;

        var atIndex = email.IndexOf('@');
        return atIndex > 0 ? email.Substring(0, atIndex).ToSentenceCase() : string.Empty;
    }

    public static string? GetNationalityEn(this string nationality)
    {
        if (string.IsNullOrWhiteSpace(nationality))
            return null;

        try
        {
            if (Dictionaries.NationalitiesEN.TryGetValue(nationality, out var value))
                return value;
        }
        catch
        {
            return null;
        }

        return null;
    }

    public static string? GetNationalityAr(this string nationality)
    {
        if (string.IsNullOrWhiteSpace(nationality))
            return null;

        try
        {
            if (Dictionaries.NationalitiesAR.TryGetValue(nationality, out var value))
                return value;
        }
        catch
        {
            return null;
        }

        return null;
    }

    public static string GetByLanguage(string? En, string? Ar, Language language)
        => language.Equals(Language.English) ? En : Ar;

    public static string GetEmailUsername(this string email)
        => email.Split('@')[0];

    public static string GetMonthName(this int month)
    {
        var culture = new System.Globalization.CultureInfo("en-US");
        return culture.DateTimeFormat.GetMonthName(month);
    }

    public static bool IsValidEmail(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return false;

        string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
        return Regex.IsMatch(value, pattern, RegexOptions.IgnoreCase);
    }

    public static bool IsValidGuid(this string value)
        => Guid.TryParse(value, out _);
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
