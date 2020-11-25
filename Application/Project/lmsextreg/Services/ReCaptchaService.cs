using System;
using System.Net;
using System.Net.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using lmsextreg.Constants;

namespace lmsextreg.Services
{
    public class ReCaptchaService : IReCaptchaService
    {
        private readonly ILogger<ReCaptchaService> _logger;
        private readonly IConfiguration _config;

        public ReCaptchaService(ILogger<ReCaptchaService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;
        }

        public bool ReCaptchaPassed(string gRecaptchaResponse, string secret)
        {
            WebProxy webProxy = new WebProxy(_config[MiscConstants.WEB_PROXY_URL], true);

            HttpClientHandler handler = new HttpClientHandler()
            {
                Proxy = webProxy
            };

            HttpClient httpClient = new HttpClient(handler);

            HttpResponseMessage res = null;
            try
            {
                res = httpClient.GetAsync($"https://www.google.com/recaptcha/api/siteverify?secret={secret}&response={gRecaptchaResponse}").Result;
            }
            catch(Exception exc)
            {
                _logger.LogError("\n[ReCaptchaService][ReCaptchaPassed][Exception]=>\n");
                _logger.LogError(exc.Message);
                _logger.LogError("\n<=[ReCaptchaService][ReCaptchaPassed][Exception]");
                _logger.LogError("\n[ReCaptchaService][ReCaptchaPassed][Exception]=>\n");
                _logger.LogError(exc.StackTrace);
                _logger.LogError("<=[ReCaptchaService][ReCaptchaPassed][Exception]");                
            }

            if (res.StatusCode != HttpStatusCode.OK)
            {
                _logger.LogError("\n[ReCaptchaService][ReCaptchaPassed] => Error while sending request to ReCaptcha");
                return false;
            }            
           
            string JSONres = res.Content.ReadAsStringAsync().Result;
            dynamic JSONdata = JObject.Parse(JSONres);

            if (JSONdata.success != "true")
            {
                return false;
            }

            return true;              
        }
    }
}