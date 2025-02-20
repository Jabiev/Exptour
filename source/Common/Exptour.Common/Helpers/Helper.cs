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
}
