using SMSReportService.Models.Domains;
using System;
using System.Collections.Generic;

namespace SMSReportService.Repositories
{
    public class ErrorRepository
    {
        public List<Error> GetLastErrors()
        {
            return new List<Error>
            {
                new Error{Message="Błąd testowy 1",Date=DateTime.Now },
                new Error{Message="Błąd testowy 2",Date=DateTime.Now }
            };
        }
    }
}
