using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using PuppeteerSharp;

namespace Oddsportal
{
    public class OddsportalScraper
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "https://www.oddsportal.com";

        public OddsportalScraper()
        {
            _httpClient = new HttpClient();
        }

        private static (string, string) ParseTeams(HtmlDocument matchPage)
        {
            var teamDivs = matchPage.DocumentNode.Descendants("div")
                .Where(div => div.Attributes.Contains("class") && div.Attributes["class"].Value.Contains("items-center")).ToList();

            string homeTeam = "";
            string awayTeam = "";

            foreach (var div in teamDivs)
            {
                var image = div.Descendants("img")
                       .Where(img => img.Attributes.Contains("src") && img.Attributes["src"].Value.Contains("team-logo"))
                      .FirstOrDefault();
                if (image != null)
                {
                    var teamName = image.NextSibling.InnerText;

                    if (!string.IsNullOrEmpty(teamName))
                    {
                        if (homeTeam == "")
                        {
                            homeTeam = teamName;
                        }
                        else if (homeTeam != "" && homeTeam != teamName)
                        {
                            awayTeam = teamName;
                            break;
                        }
                    }
                }
            }

            return (homeTeam, awayTeam);
        }

        public DateTime ParseStartTime(HtmlDocument matchPage)
        {
            var startTimeDiv = matchPage.DocumentNode.Descendants("div")
                                       .FirstOrDefault(div => div.Attributes.Contains("class") && div.Attributes["class"].Value.Contains("start-time"));

            if (startTimeDiv != null)
            {
                var startTimeText = startTimeDiv.NextSibling?.InnerHtml;
                if (!string.IsNullOrEmpty(startTimeText))
                {
                    startTimeText = Regex.Replace(startTimeText, @"^\D+", ""); // Ta bort inledande icke-siffror
                    startTimeText = startTimeText.Replace("&nbsp;", " ").Trim(); // Ersätt "&nbsp;" med mellanslag och trimma bort mellanslag i början och slutet

                    DateTime startTime;
                    if (DateTime.TryParse(startTimeText, out startTime))
                    {
                        return startTime;
                    }
                }
            }

            // Returnera DateTime.MinValue om det inte går att parsas eller om elementet inte hittas
            return DateTime.MinValue;
        }

        public string ParseCountryFromUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                var segments = uri.Segments;
                if (segments.Length >= 3)
                {
                    // Landet kommer vara i det tredje segmentet i URL:en
                    var country = segments[2].Trim('/');

                    // Om landet innehåller bindestreck kan du byta ut dem med mellanslag
                    country = country.Replace('-', ' ');
                    country = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(country);

                    return country;
                }
            }
            catch (UriFormatException)
            {
                // URL:en är ogiltig, hantera detta fall efter behov
            }

            // Returnera null eller en standardsträng om landet inte kan parsas
            return null;
        }

        public string ParseSportFromUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                var segments = uri.Segments;

                if (segments.Length >= 2)
                {
                    // Sporten kommer att finnas i det andra segmentet i URL:en
                    var sport = segments[1].Trim('/');
                    sport = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(sport);

                    if (sport.Equals("Football", StringComparison.OrdinalIgnoreCase))
                    {
                        // Om sporten är "football", ändra den till "Soccer"
                        sport = "Soccer";
                    }

                    return sport;
                }
            }
            catch (UriFormatException)
            {
                // URL:en är ogiltig, hantera detta fall efter behov
            }

            // Returnera null eller en standardsträng om sporten inte kan parsas
            return null;
        }
 
        public string ParseCompetitionFromUrl(string url)
        {
            try
            {
                var uri = new Uri(url);
                var segments = uri.Segments;

                if (segments.Length >= 3)
                {
                    // Tävlingen kommer att finnas i det tredje segmentet i URL:en
                    var competition = segments[3].Trim('/');

                    // Konvertera första bokstaven till stor bokstav
                    competition = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(competition);

                    return competition;
                }
            }
            catch (UriFormatException)
            {
                // URL:en är ogiltig, hantera detta fall efter behov
            }

            // Returnera null eller en standardsträng om tävlingen inte kan parsas
            return null;
        }


        // Metod för att extrahera bookmakerns namn från URL
        private string GetBookmakerNameFromUrl(string url)
        {
            var uri = new Uri(url);
            var path = uri.AbsolutePath.TrimStart('/');
            var segments = path.Split('/');
            var bookmakerName = segments[1]; // Bookmakerns namn är den andra delen av URL-segmentet

            return bookmakerName;
        }

        private Odds ParseAverageOdds(HtmlDocument document)
        {
            Odds matchOdds = new Odds();

            var oddsElements = document.DocumentNode.Descendants("div")
                .Where(div => div.Attributes.Contains("class") && div.Attributes["class"].Value.Contains("flex text-xs h-9 border-b border-black-borders border-l border-r bg-gray-light"))
                .ToList();
            if (oddsElements.Count != 2)
                throw new Exception("Kunde inte hämta genomsnittsodds");

            var averageOddsElements = oddsElements.FirstOrDefault();

            var matchOddsParagraph = averageOddsElements.Descendants("p")
                    .Where(div => div.Attributes.Contains("class") && div.Attributes["class"].Value.Contains("height-content"))
                    .ToList();

            var MatchOddsList = new List<Decimal>();
            foreach (var matchoddsElement in matchOddsParagraph)
            {
                var stringOdds = matchoddsElement.InnerText;

                if (stringOdds != null && decimal.TryParse(stringOdds.Replace("%",string.Empty), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal odds))
                {
                    MatchOddsList.Add(odds);
                }
            }

            if (MatchOddsList.Count == 4)
            {
                matchOdds.Home = MatchOddsList.ElementAt(0);
                matchOdds.Draw = MatchOddsList.ElementAt(1);
                matchOdds.Away = MatchOddsList.ElementAt(2);
                matchOdds.Payout = MatchOddsList.ElementAt(3);
            }
            else
                throw new Exception("Kunde inte hämta odds korrekt");
           
            return matchOdds;
        }

        private Odds ParseHighestOdds(HtmlDocument document)
        {
            Odds matchOdds = new Odds();

            var oddsElements = document.DocumentNode.Descendants("div")
                .Where(div => div.Attributes.Contains("class") && div.Attributes["class"].Value.Contains("flex text-xs h-9 border-b border-black-borders border-l border-r bg-gray-light"))
                .ToList();
            if (oddsElements.Count != 2)
                throw new Exception("Kunde inte hämta genomsnittsodds");

            var highestOddsElements = oddsElements.LastOrDefault();

            var matchOddsParagraph = highestOddsElements.Descendants("p")
                    .Where(div => div.Attributes.Contains("class") && (div.Attributes["class"].Value.Contains("height-content") || div.Attributes["class"].Value.Contains("font-bold")))
                    .ToList();

            var MatchOddsList = new List<Decimal>();
            foreach (var matchoddsElement in matchOddsParagraph)
            {
                var stringOdds = matchoddsElement.InnerText;

                if (stringOdds != null && decimal.TryParse(stringOdds.Replace("%", string.Empty), NumberStyles.Float, CultureInfo.InvariantCulture, out decimal odds))
                {
                    MatchOddsList.Add(odds);
                }
            }

            if (MatchOddsList.Count == 4)
            {
                matchOdds.Home = MatchOddsList.ElementAt(0);
                matchOdds.Draw = MatchOddsList.ElementAt(1);
                matchOdds.Away = MatchOddsList.ElementAt(2);
                matchOdds.Payout = MatchOddsList.ElementAt(3);
            }
            else
                throw new Exception("Kunde inte hämta odds korrekt");

            return matchOdds;
        }


        public List<BookMakerOdds> ParseOdds(HtmlDocument document)
        {
            var oddsList = new List<BookMakerOdds>();

            var oddsElements = document.DocumentNode.Descendants("div")
                .Where(div => div.Attributes.Contains("data-v-cb2b6512") && div.Attributes["class"].Value.Contains("flex text-xs border-b h-9 border-l border-r"))
                .ToList();

            foreach (var element in oddsElements)
            {
                var bookMakerOdds = new BookMakerOdds();
                bookMakerOdds.Odds = new Odds();

                var bookMaker = element.Descendants("div")
                    .FirstOrDefault(d => d.Attributes.Contains("data-v-7cea9570") && d.Attributes.Contains("data-v-cb2b6512"));
                string bookMakerUrl = bookMaker.Descendants("a").FirstOrDefault(a => a.Attributes.Contains("href")).Attributes["href"].Value;
                string bookMakerName = GetBookmakerNameFromUrl(bookMakerUrl);

                bookMakerOdds.Bookmaker = bookMakerName;

                var matchOdds = element.Descendants("div")
                     .Where(div => div.Attributes.Contains("data-v-cb2b6512") && div.Attributes["class"].Value.Contains("flex flex-row items-center gap-[3px]"))
                     .ToList();

                var MatchOddsList = new List<Decimal>();
                foreach (var matchoddsElement in matchOdds)
                {
                    var stringOdds = matchoddsElement.Descendants("p")
                        .Where(p => p.Attributes.Contains("data-v-cb2b6512") && (p.Attributes["class"].Value.Contains("height-content") || p.Attributes["class"].Value.Contains("text-black-main")))
                        .FirstOrDefault().InnerHtml;

                    if (stringOdds != null && decimal.TryParse(stringOdds, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal odds))
                    {
                        MatchOddsList.Add(odds);
                    }
                }
                if (MatchOddsList.Count == 3)
                {
                    bookMakerOdds.Odds.Home = MatchOddsList.ElementAt(0);
                    bookMakerOdds.Odds.Draw = MatchOddsList.ElementAt(1);
                    bookMakerOdds.Odds.Away = MatchOddsList.ElementAt(2);
                }
                else
                     throw new Exception("Kunde inte hämta odds korrekt");

                var payoutDiv = element.Descendants("div")
                     .Where(div => div.Attributes.Contains("data-v-cb2b6512") && div.Attributes["class"].Value.Contains("flex-center relative gap-1 border-l border-r border-black-borders min-w-[60px] text-black-main"))
                     .FirstOrDefault();
               var payoutText = payoutDiv.Descendants("span")
                        .Where(p => p.Attributes.Contains("data-v-cb2b6512") && p.Attributes["class"].Value.Contains("height-content text-[10px]"))
                        .FirstOrDefault().InnerHtml?.Replace("%", string.Empty);

                if (payoutText != null && decimal.TryParse(payoutText, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal payout))
                    bookMakerOdds.Odds.Payout = payout;

                oddsList.Add(bookMakerOdds);
            }

            return oddsList;
        }
        public string ConvertMarketToBetfairFormat(string market)
        {
            Dictionary<string, string> marketMapping = new Dictionary<string, string>()
            {
                { "1X2 Fulltime", "Fulltime Matchodds" },
                {" ","Fulltime Matchodds" }
                // Add more mappings as needed
            };

            if (marketMapping.ContainsKey(market))
            {
                return marketMapping[market];
            }

            return market; // Return the same market if no mapping is found
        }


        public string ParseMarketFromUrl(string url)
        {
            Uri uri = new Uri(url);
            string market = uri.Fragment.TrimStart('#');
            string[] parts = market.Split(';');
            string part1 = parts[0];
            string part2 = parts.Length > 1 ? parts[1] : string.Empty;
            if (part1 != "") { 
                switch (part2)
                {
                    case "2":
                        part2 = "Fulltime";
                        break;
                    case "":
                        part2 = "Fulltime";
                        break;
                    case "3":
                        part2 = "First half";
                        break;
                    case "4":
                        part2 = "Second half";
                        break;
                    default:
                        break;
                } 
            }

           return ConvertMarketToBetfairFormat(part1 + " " + part2);
        }


        public async Task<List<OddsportalMatch>> GetMatchesAsync(string leagueUrl)
        {
            var matches = new List<OddsportalMatch>();

            var leaguePage = await GetHtmlDocumentUsingPuppeteerAsync(BaseUrl + leagueUrl);

            var eventRows = leaguePage.DocumentNode.Descendants("div")
                                .Where(div => div.Attributes.Contains("class") && div.Attributes["class"].Value.Contains("eventRow"))
                                .ToList();
          
            List<String> matchLinks = new List<String>();
            foreach (var row in eventRows)
            {
                var link = row.Descendants("a")
        .Where(a => a.Attributes.Contains("class") && a.Attributes["class"].Value.Contains("next-m:flex-row"))
        .Select(a => BaseUrl + a.Attributes["href"].Value)
        .FirstOrDefault();
                 if (link != null)
                 {
                     matchLinks.Add(link);
                 }
            }

            foreach (var matchLink in matchLinks)
            {
                OddsportalMatch oddsportalMatch = new OddsportalMatch();
                var matchPage = await GetHtmlDocumentUsingPuppeteerAsync(matchLink);

                (string homeTeam, string awayTeam) teams = ParseTeams(matchPage);
                oddsportalMatch.HomeTeam = teams.homeTeam;
                oddsportalMatch.AwayTeam = teams.awayTeam;
                 
                oddsportalMatch.StartTime = ParseStartTime(matchPage);

                oddsportalMatch.Country = ParseCountryFromUrl(matchLink);
                oddsportalMatch.Sport = ParseSportFromUrl(matchLink);
                oddsportalMatch.Competition = ParseCompetitionFromUrl(matchLink);

                try
                {
                    var matchOdds = new MatchOdds();
                    matchOdds.CurrentBookMakerOdds = ParseOdds(matchPage);
                    matchOdds.AverageOdds = ParseAverageOdds(matchPage);
                    matchOdds.HighestOdds = ParseHighestOdds(matchPage);
                    matchOdds.Market = ParseMarketFromUrl(matchLink);

                    oddsportalMatch.Odds = matchOdds;

                    oddsportalMatch.BetfairMatch = new MyBetfairAPI.BetfairMatch(oddsportalMatch.HomeTeam, oddsportalMatch.AwayTeam, oddsportalMatch.Sport);
                }catch(Exception e) { }


                matches.Add(oddsportalMatch);
               
            }

            return matches;
        }

    

        private async Task<HtmlDocument> GetHtmlDocumentUsingPuppeteerAsync(string url)
        {
            var browserFetcher = new BrowserFetcher();

            if (await browserFetcher.CanDownloadAsync(BrowserFetcher.DefaultChromiumRevision))
            {
                await browserFetcher.DownloadAsync(BrowserFetcher.DefaultChromiumRevision);
            }
       
            var browser = await Puppeteer.LaunchAsync(new LaunchOptions { Headless = true });
            var page = await browser.NewPageAsync();
            await page.GoToAsync(url);
            var content = await page.GetContentAsync();
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(content);
            await browser.CloseAsync();
            return htmlDocument;
        }

    }
}


/*
            var response = await _httpClient.GetAsync(url);
            var content = await response.Content.ReadAsStringAsync();
            var document = new HtmlDocument();
            document.LoadHtml(content);
            return document;*/









