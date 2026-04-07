namespace UzWorks.Core.Entities.SMS;

public class SmsToken : BaseEntity
{
    public SmsToken(string smsCode, string phoneNumber)
    {
        SmsCode = smsCode;
        PhoneNumber = phoneNumber;
        CreateDate = DateTime.Now;
        UpdateDate = DateTime.Now;
    }

    public string SmsCode { get; set; } = string.Empty;
    public string PhoneNumber { get; set; } = string.Empty;
}
