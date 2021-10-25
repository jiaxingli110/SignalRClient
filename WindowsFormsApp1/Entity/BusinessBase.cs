using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SignalRClient.Entity
{
    public class BusinessBase
    {
        public int Id;
        public string OperatorName;
        public string ProcessId;
        public string BusinessName;
        public string StepName;
        public DateTime DeadLine;
        public DateTime CompletedTime;
        public DateTime ReceiveTime;
        public DateTime OperateTime;
        public OriCusOps Ops;
    }
}
