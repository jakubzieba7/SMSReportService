using SMSReportService.Repositories;
using System;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Timers;

namespace SMSReportService
{
    public partial class SMSReportService : ServiceBase
    {

        private Timer _timer;
        private ErrorRepository _errorRepository = new ErrorRepository();
        private NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        private readonly int _intervalInMinutes;
        private readonly int _sendHour;
        private readonly bool _ifSendReport;

        public SMSReportService()
        {
            InitializeComponent();

            try
            {
                _intervalInMinutes = Convert.ToInt32(ConfigurationManager.AppSettings["IntervalInMinutes"]);
                _sendHour = Convert.ToInt32(ConfigurationManager.AppSettings["SendHour"]);
                _ifSendReport = Convert.ToBoolean(ConfigurationManager.AppSettings["IfSendReport"]);
                _timer = new Timer(_intervalInMinutes * 60000);
                
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        protected override void OnStart(string[] args)
        {
            _timer.Elapsed += DoWork;
            _timer.Start();
            Logger.Info("Service started...");
        }

        private void SendError()
        {
            var actualHour = DateTime.Now.Hour;
            if (actualHour < _sendHour)
                return;

            var errors = _errorRepository.GetLastErrors();

            if (errors == null || !errors.Any())
                return;

            GenerateSMSContent.SendSMS(errors);

            Logger.Info("Error sent...");
        }

        private void DoWork(object sender, ElapsedEventArgs e)
        {
            try
            {
                SendError();
            }
            catch (Exception ex)
            {
                Logger.Error(ex, ex.Message);
                throw new Exception(ex.Message);
            }
        }

        protected override void OnStop()
        {
            Logger.Info("Service stopped...");
        }
    }
}
