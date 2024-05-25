using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyBetfairAPI
{
    public interface Ibetfairhandler
    {
 
        List<IMatch> GetMatches { get; set; }


    }
}
