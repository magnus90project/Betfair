using System;
using System.Collections.Generic;
using System.Text;

namespace Oddsportal
{
    public class MatchOdds 
    {
        public string Market { get; set; }
        public List<BookMakerOdds> CurrentBookMakerOdds { get; set; }
        public Odds AverageOdds { get; set; }
        public Odds HighestOdds { get; set; }
    }
    public class Odds
    {
        public decimal Home { get; set; }
        public decimal Draw { get; set; }
        public decimal Away { get; set; }
        public decimal Payout { get; set; }
    }

}
