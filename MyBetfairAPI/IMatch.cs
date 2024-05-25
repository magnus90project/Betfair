using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBetfairAPI
{
    public interface IMatch
    {   
        string Name { get; set; }
        string Sport { get; set; }
        string Country { get; set; }
        string Competition { get; set; }
        DateTime StartTime { get; set; }
        string HomeTeam { get; set; }
        string AwayTeam { get; set; }
        BetfairMatch BetfairMatch { get; set; }
    }
}
