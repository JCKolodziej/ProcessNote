using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProcessNote
{
    public class ProcessDetailRow
    {
        public string Detail { get; set; }
        public string Amount { get; set; }

        public ProcessDetailRow(string detail, string amount)
        {
            Detail = detail;
            Amount = amount;
        }
    }
}
