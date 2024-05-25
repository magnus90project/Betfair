using Betfair_API_NG.TO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MyBetfairAPI
{
    public class BetfairScraper
    {
        private static BetfairApiClient betfairApiClient = new BetfairApiClient("fdRyqNg9U2HvwJlk", "C:\\Users\\Renen\\OneDrive\\Dokument\\certifikatbetfair\\client-2048.crt", "nana951dah");
        private static string _sesstionToken;

        public  BetfairScraper()
        {
            _sesstionToken = betfairApiClient.LoginWithCertAsync().Result;
        }

        public List<BetfairMatch> getMatches(string competition)
        {
            List<BetfairMatch> betfairMatches = new List<BetfairMatch>();

            MarketFilter marketFilter = new MarketFilter();
            var fotballCompetitions = betfairApiClient.ListAllFootballCompetitionsAsync()?.Result;
            var competitionId = fotballCompetitions.Where(c => c.Competition.Name.Contains(competition)).Select(c => c.Competition.Id).ToHashSet();

            marketFilter.CompetitionIds = competitionId;
           var eventResults =  betfairApiClient.ListEventsAsync(marketFilter,"").Result;

            foreach(var eventResult in eventResults)
            {
                string[] teams = eventResult.Event.Name.Split('v');

                string homeTeam = teams[0].Trim();
                string awayTeam = teams[1].Trim();
                 BetfairMatch betfairMatch = new BetfairMatch(homeTeam, awayTeam, "Soccer");
                betfairMatches.Add(betfairMatch);
            }


            return betfairMatches;
        }



    }
}
