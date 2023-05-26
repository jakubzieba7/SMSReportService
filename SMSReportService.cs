using SMSReportService.Repositories;
using System;
using System.Linq;
using System.ServiceProcess;
using System.Timers;

namespace SMSReportService
{
    public partial class SMSReportService : ServiceBase
    {
        private const int IntervalInMinutes = 30;
        private Timer _timer = new Timer(IntervalInMinutes * 1000);
        private ErrorRepository _errorRepository = new ErrorRepository();
        private static readonly NLog.Logger Logger = NLog.LogManager.GetCurrentClassLogger();

        public SMSReportService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _timer.Elapsed += DoWork;
            _timer.Start();
            Logger.Info("Service started...");
        }

        private void SendError()
        {
            var errors = _errorRepository.GetLastErrors(IntervalInMinutes);

            if (errors == null || !errors.Any())
                return;


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
