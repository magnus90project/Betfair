using System;
using System.Threading.Tasks;
using MyBetfairAPI;
using FootyStatsAPI;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace FootyStatsBetfairAPI
{
    class Program
    {
        static async Task Main(string[] args)
        {
            // Betfair API-inloggningsuppgifter
            string appKey = "fdRyqNg9U2HvwJlk";
            string username = "fotboll53";
            string password = "nana951dah";
            string certPath = "C:\\Users\\Renen\\OneDrive\\Dokument\\certifikatbetfair\\client-2048.crt";

            // FootyStats API-nyckel
            string footyStatsApiKey = "DIN_FOOTYSTATS_API_NYCKEL";

            // Skapa en Betfair API-klient och logga in
            var betfairApiClient = new BetfairApiClient(appKey, certPath, "nana951dah");
            //    var sessionToken = await betfairApiClient.LoginAsync();
            var f = await betfairApiClient.LoginWithCertAsync();
            //  var eventTypes = await betfairApiClient.ListEventTypesAsync();
            //   List<EventResponse> events = await betfairApiClient.ListEventsAsync("1", new DateTime(2023, 4, 19), new DateTime(2024, 4, 19));
            //  List<MarketCatalogueResponse> marketCatalogue = await betfairApiClient.ListMarketCatalogueAsync(events.FirstOrDefault().Event.Id);
            //  var competitions = await betfairApiClient.ListCompetitionsAsync("1");
            //   var markBook = await betfairApiClient.ListMarketBookAsync(new List<string> { marketCatalogue.FirstOrDefault().MarketId });
            //   var result = await betfairApiClient.GetMarketPriceAndAvailabilityAsync("Newcastle", "Tottenham", "Match Odds", "Tottenham", "BACK");
            //var betId2 = await betfairApiClient.FindAndPlaceBetAsync("Liverpool", "Fulham", "Match Odds", "Liverpool", "BACK", 1.59, 37);
            //    var d = await betfairApiClient.ReplaceOrdersAsync("Liverpool", "Fulham", "Match Odds", "Liverpool", Side.BACK,1.3);

            //  var eventtype = await betfairApiClient.ListAllEventTypesAsync();
            //      var d = await betfairApiClient.getMarketCatalogueAsync("Bournemouth", "Chelsea", "Match Odds", "Soccer");
            //Match match = new Match("Liverpool", "Brentford", "Soccer");
            //   var markets = match.Markets;
            Oddsportal.OddsportalScraper oddsportalScraper = new Oddsportal.OddsportalScraper();
            var oddsportalMatches = oddsportalScraper.GetMatchesAsync("/football/sweden/allsvenskan/").Result;
            Console.WriteLine("Hämtat matcher");
     

            //        BetfairScraper betfairScraper = new BetfairScraper();
            //       var bet =  betfairScraper.getMatches("Allsvenskan");

            //     var d = await betfairApiClient.CancelBetAsync("Liverpool", "Fulham", "Match Odds");

            //  var d = await betfairApiClient.GetMarketProfitAndLossAsync("Liverpool", "Tottenham", "Match Odds");
            //   var b = await betfairApiClient.GetPlacedOrdersAsync("Liverpool", "Tottenham", "Match Odds");
            //  var currentOrdes = await betfairApiClient.GetPlacedOrdersAsync();
            //  var a = await betfairApiClient.UpdateBetAsync("305815591864", "1.213420259");
            //   var betSize = await betfairApiClient.CalculateEqualProfitLossBet("Liverpool", "Tottenham", "Match Odds");
            //  var d = await betfairApiClient.CashOutAsync("Brighton", "Wolves", "Match Odds", 111);
            // Skapa en FootyStats API-klient och hämta Premium Predictions-data
            //  var footyStatsApiClient = new FootyStatsApiClient(footyStatsApiKey);
            // var premiumPredictions = await footyStatsApiClient.GetPremiumPredictionsAsync();
            /*
            // Analysera data från FootyStats API och placera spel på Betfair API
            foreach (var prediction in premiumPredictions)
            {
                // Analysera data från FootyStats API för att identifiera vadslagningsmöjligheter
                // ...

                // Placera spel på Betfair API med hjälp av Betfair API-klienten
                string marketId = "MARKNADS_ID";
                long selectionId = 123456;
                double stake = 10.0;
                double price = 2.0;
                var betId = await betfairApiClient.PlaceBetAsync(marketId, selectionId, stake, price);

                Console.WriteLine($"Placed bet {betId} on market {marketId} for {stake} at {price}");

                Console.ReadLine();
            }*/
        }
    }
}
