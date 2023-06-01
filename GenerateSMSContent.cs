using NLog;
using SMSReportService.Models.Domains;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace SMSReportService
{
    public class GenerateSMSContent
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private string _twilioMessageResource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TwilioMessageResourcePath.txt");

        public void SendSMS(List<Error> errors)
        {
            // Find your Account SID and Auth Token at twilio.com/console
            // and set the environment variables. See http://twil.io/secure
            string accountSid = Environment.GetEnvironmentVariable(ConfigurationManager.AppSettings["AccountSID"]);
            string authToken = Environment.GetEnvironmentVariable(ConfigurationManager.AppSettings["AuthToken"]);

            TwilioClient.Init(accountSid, authToken);

            try
            {
                var _intervalInMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalInMinutes"]);

                var numbersToMessage = new List<string>
                {
                    ConfigurationManager.AppSettings["ToPhoneNumber"],
                    //"+14158141829",
                    //"+15017122661"
                };

                foreach (var number in numbersToMessage)
                {
                    var message = MessageResource.Create(
                        body: GenerateErrors(errors, _intervalInMinutes),
                        from: new Twilio.Types.PhoneNumber(ConfigurationManager.AppSettings["FromPhoneNumber"]),
                        to: new Twilio.Types.PhoneNumber(number)
                    );

                    File.WriteAllText(_twilioMessageResource, message.Sid);
                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }

        }

        public string GenerateErrors(List<Error> errors, int interval)
        {
            if (errors == null)
                throw new ArgumentNullException(nameof(errors));

            if (!errors.Any())
                return string.Empty;

            var smsContent = $"Błędy z ostatnich {interval} minut.";

            foreach (var error in errors)
            {
                smsContent += $@"{error.Message}{Environment.NewLine}{error.Date.ToString("dd-MM-yyyy HH:mm")}{Environment.NewLine}";
            }

            return smsContent;
        }
    }
}
