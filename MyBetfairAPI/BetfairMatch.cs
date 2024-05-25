using Betfair_API_NG.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ConstrainedExecution;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace MyBetfairAPI
{
    public class BetfairMatch 
    {
        private static BetfairApiClient betfairApiClient = new BetfairApiClient("fdRyqNg9U2HvwJlk", "C:\\Users\\Renen\\OneDrive\\Dokument\\certifikatbetfair\\client-2048.crt", "nana951dah");
        private static string _sesstionToken;
        public string Id { get; set; }
        public string Name { get; set; }
        public string Sport { get; set; }
        public string Country { get; set; }
        public string Competition { get; set; }
        public DateTime StartTime { get; set; }
        public string HomeTeam { get; set; }
        public string AwayTeam { get; set; }
        public List<MarketOdds> MarketOdds { get; set; }
        public List<List<MarketOdds>> MarketOddsHistory { get; set; } = new List<List<MarketOdds>>();   
        public List<CurrentOrderSummary> Orders { get; set; }
        public List<MarketCatalogue> Markets { get; private set; }
                
        public Dictionary<string, List<CurrentOrderSummary>> OrdersByMarket { get; set; }
        private Timer timer;
        public BetfairMatch(string homeTeam, string awayteam, string sport)
        {
            if (_sesstionToken == null || _sesstionToken == "")
                LoginToBetfair();

            this.HomeTeam = homeTeam;
            this.AwayTeam = awayteam;
            this.Sport = sport;
            this.Name = homeTeam + " v " + awayteam;
            this.Markets = betfairApiClient.getMarketCatalogueAsync(HomeTeam, AwayTeam, Sport).Result;
            this.MarketOdds = getMarketOdds();

            MarketCatalogue market = Markets.FirstOrDefault();
            this.Country = BetfairHelper.GetCountryName(market.Event.CountryCode);
         
            this.Competition = market.Competition.Name;

            var eventTimeZone = TimeZoneInfo.FindSystemTimeZoneById("South Africa Standard Time");
     
            this.StartTime = TimeZoneInfo.ConvertTimeFromUtc(market.MarketStartTime, eventTimeZone);


            timer = new Timer(state =>
            {
                UpdateMarketOdds();
            }, null, TimeSpan.Zero, TimeSpan.FromSeconds(30));

        }
        public void UpdateMarketOdds()
        {
            MarketOddsHistory.Add(MarketOdds);
            MarketOdds = getMarketOdds();
        }
        private List<MarketOdds> getMarketOdds()
        {
            List<MarketOdds> matchOdds = new List<MarketOdds>(); 
            // getAllMarketPrice(homeName, awayName, marketName, eventTypeName)
            //GetMarketPriceAndAvailabilityAsync("Newcastle", "Tottenham", "Match Odds", "Tottenham", "BACK");
            var markets = betfairApiClient.getMarketCatalogueAsync(HomeTeam, AwayTeam, Sport).Result;
            foreach(var market in markets)
            {
                MarketOdds marketOdds = new MarketOdds();
                var marketBooks = betfairApiClient.ListMarketBookAsync(new List<string> { market.MarketId }).Result;
               foreach(var marketBook in marketBooks)
                {
                  
                    marketOdds.Market = market.MarketName;
                    foreach(var runner in marketBook.Runners)
                    {
                        BetfairOdds betfairOdds = new BetfairOdds();
                        try
                        {
                            betfairOdds.RunnerName = market.Runners.SingleOrDefault(r => r.SelectionId == runner.SelectionId).RunnerName;
                        }catch(System.Exception e)
                        {
                            continue;
                        }
                 
                        foreach(var priceSize in runner.ExchangePrices.AvailableToBack)
                        {
                            RunnerOdds runnerOdds = new RunnerOdds();
                            runnerOdds.Odds = priceSize.Price;
                            runnerOdds.Size = priceSize.Size;
                            betfairOdds.RunnerBackOdds.Add(runnerOdds);
                        }
                        foreach (var priceSize in runner.ExchangePrices.AvailableToLay)
                        {
                            RunnerOdds runnerOdds = new RunnerOdds();
                            runnerOdds.Odds = priceSize.Price;
                            runnerOdds.Size = priceSize.Size;
                            betfairOdds.RunnerLayOdds.Add(runnerOdds);
                        }
                        marketOdds.BetfairOdds.Add(betfairOdds);
                    }
                    marketOdds.BetfairOddsTime = DateTime.Now;
                }
                matchOdds.Add(marketOdds);
            }

            return matchOdds;
  
        }

        private static void LoginToBetfair()
        {
            _sesstionToken = betfairApiClient.LoginWithCertAsync().Result;
        }

        public void UpdateMarkets()
        {
            this.Markets = betfairApiClient.getMarketCatalogueAsync(HomeTeam, AwayTeam, Sport).Result;
        }
    }
}
