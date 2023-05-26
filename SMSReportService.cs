using SMSReportService.Repositories;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace SMSReportService
{
    public partial class SMSReportService : ServiceBase
    {
        private const int IntervalInMinutes = 30;
        private Timer _timer=new Timer(IntervalInMinutes*1000);
        private ErrorRepository _errorRepository = new ErrorRepository();

        public SMSReportService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            _timer.Elapsed += DoWork;
            _timer.Start();
        }

        private void SendError()
        {
            var errors = _errorRepository.GetLastErrors(IntervalInMinutes);

            if (errors == null || !errors.Any())
                return;
        }

        private void DoWork(object sender, ElapsedEventArgs e)
        {
            SendError();
        }


        protected override void OnStop()
        {
        }
    }
}
