using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBetfairAPI.Entities
{
    public class CancelInstructiona
    {
        public string BetId { get; set; }
        public double? SizeReduction { get; set; }
    }

}
