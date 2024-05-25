using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBetfairAPI
{
    public class MarketOdds
    {
        public string Market { get; set; }
        public DateTime BetfairOddsTime { get; set; }
        public List<BetfairOdds> BetfairOdds { get; set; } = new List<BetfairOdds>();

    }
}
