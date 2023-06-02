using CypherNew;
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
    public static class GenerateSMSContent
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static string _twilioMessageResource = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "TwilioMessageResourcePath.txt");
        private static string _fromPhoneNumber;
        private static string _toPhoneNumber;
        private static string _accountSID;
        private static string _authToken;

        public static void SendSMS(List<Error> errors)
        {

            try
            {
                var _intervalInMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalInMinutes"]);
                _fromPhoneNumber = DecryptCredentials.DecryptFromPhoneNumber();
                _toPhoneNumber = DecryptCredentials.DecryptToPhoneNumber();
                _accountSID = DecryptCredentials.DecryptAccountSID();
                _authToken = DecryptCredentials.DecryptAuthToken();

                TwilioClient.Init(_accountSID, _authToken);

                var numbersToMessage = new List<string>
                {
                    _toPhoneNumber,
                    //"+14158141829",
                    //"+15017122661"
                };

                foreach (var number in numbersToMessage)
                {
                    var message = MessageResource.Create(
                        body: GenerateErrors(errors, _intervalInMinutes),
                        from: new Twilio.Types.PhoneNumber(_fromPhoneNumber),
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

        public static string GenerateErrors(List<Error> errors, int interval)
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
