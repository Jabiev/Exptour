namespace Exptour.Application.Settings;

public class OTP
{
    public int OTPRangeFrom { get; set; }
    public int OTPRangeTo { get; set; }
    public int ExpireMinute { get; set; }
    public int ResendMinute { get; set; }
    public int AttemptCount { get; set; }
    public EmailSettings Email { get; set; }

    public class EmailSettings
    {
        public string EmailSubjectEn { get; set; }
        public string EmailSubjectAr { get; set; }
        public string EmailBodyEn { get; set; }
        public string EmailBodyAr { get; set; }
    }
}
