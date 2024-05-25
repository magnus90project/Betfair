using System;
using System.Collections.Generic;
using System.Text;

namespace FootyStatsAPI
{
    public class PremiumPredictionsResponse
    {
        public bool success { get; set; }
        public PremiumPrediction[] data { get; set; }
    }
}
