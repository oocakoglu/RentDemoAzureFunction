using System;
using System.Collections.Generic;
using System.Text;

namespace Timeentry.Models
{
    public class OperationResponse
    {
        public string ExistRecords { get; set; }

        public string CreatedRecords { get; set; }

        public string FailedRecords { get; set; }
    }
}
