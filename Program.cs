using NLog;
using NLog.Internal;
using System;
using System.Collections.Generic;
using System.Configuration.Install;
using System.Linq;
using System.Reflection;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace SMSReportService
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        /// 
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        static void Main(string[] args)
        {
            if (Environment.UserInteractive)
            {
                var parameter = string.Concat(args);
                switch (parameter)
                {
                    case "--install":
                        ManagedInstallerClass.InstallHelper(new[] { Assembly.GetExecutingAssembly().Location });
                        break;
                    case "--uninstall":
                        ManagedInstallerClass.InstallHelper(new[] { "/u", Assembly.GetExecutingAssembly().Location });
                        break;
                }
            }
            else
            {
                ServiceBase[] ServicesToRun;
                ServicesToRun = new ServiceBase[]
                {
                new SMSReportService()
                };
                ServiceBase.Run(ServicesToRun);
            }

            // Find your Account SID and Auth Token at twilio.com/console
            // and set the environment variables. See http://twil.io/secure
            string accountSid = Environment.GetEnvironmentVariable(System.Configuration.ConfigurationManager.AppSettings["AccountSID"]); 
            string authToken = Environment.GetEnvironmentVariable(System.Configuration.ConfigurationManager.AppSettings["AuthToken"]);

            TwilioClient.Init(accountSid, authToken);

            try
            {
                var numbersToMessage = new List<string>
                {
                    System.Configuration.ConfigurationManager.AppSettings["ToPhoneNumber"],
                    //"+14158141829",
                    //"+15017122661"
                };

                foreach (var number in numbersToMessage)
                {
                    var message = MessageResource.Create(
                        body: "Hello from my Twilio number!",
                        from: new Twilio.Types.PhoneNumber(System.Configuration.ConfigurationManager.AppSettings["FromPhoneNumber"]),
                        to: new Twilio.Types.PhoneNumber(number)
                    );

                }
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }

            
        }
    }
}
