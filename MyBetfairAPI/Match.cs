using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBetfairAPI
{
    public class Match : IMatch
    {
        public string Name { get; set; }
        public string Sport { get; set; }
        public string Country { get; set; }
        public string Competition { get; set; }
        public DateTime StartTime { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }

        public BetfairMatch BetfairMatch { get; set; }


    }
}
