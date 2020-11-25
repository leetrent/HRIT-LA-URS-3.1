namespace lmsextreg.Services
{
    public interface IReCaptchaService
    {
        bool ReCaptchaPassed(string gRecaptchaResponse, string secret);
    }
}