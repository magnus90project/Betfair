using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
//using Betfair_API_NG.TO;

using Exception = System.Exception;
using MyBetfairAPI.Entities;
using Betfair_API_NG.TO;
using Betfair_API_NG.Json;


namespace MyBetfairAPI
{
    public class BetfairApiClient
    {

        private readonly string _appKey;
        private readonly string _username = "fotboll53";
        private readonly string _password = "nana951dah";
        private string _sessionToken;
        private readonly X509Certificate2 _certificate;
        private const string _baseUrl = "https://api.betfair.com/exchange/betting/rest/v1.0/";


        public BetfairApiClient(string appKey, string certificatePath, string certificatePassword)
        {
            _appKey = appKey;
            _certificate = new X509Certificate2(certificatePath, certificatePassword);
        }

        public async Task<string> LoginWithCertAsync()
        {
            var url = "https://identitysso-cert.betfair.se/api/certlogin";
            var clientHandler = new HttpClientHandler();
            clientHandler.ClientCertificates.Add(_certificate);
            var client = new HttpClient(clientHandler);
            client.DefaultRequestHeaders.Add("X-Application", _appKey);

            var content = new FormUrlEncodedContent(new[]
            {
        new KeyValuePair<string, string>("username", _username),
        new KeyValuePair<string, string>("password", _password)
    });


            var response = await client.PostAsync(url, content);

            if (!response.IsSuccessStatusCode)
            {
                throw new Exception("Failed to log in to Betfair API with certificate: " + response.ReasonPhrase);
            }

            var result = await response.Content.ReadAsStringAsync();
            dynamic jsonResult = Newtonsoft.Json.JsonConvert.DeserializeObject(result);

            if (jsonResult.loginStatus != "SUCCESS")
            {
                throw new Exception("Failed to log in to Betfair API with certificate: " + jsonResult.loginStatus);
            }

            _sessionToken = jsonResult.sessionToken;
            return _sessionToken;
        }

        public async Task<List<EventTypeResult>> ListEventTypesAsync(MarketFilter filter, string locale = null)
        {
            var url = _baseUrl + "listEventTypes/";

            var requestBody = new
            {
                filter = filter,
                locale = locale
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var response = await SendRequest(request, requestBody);

            var eventTypeResults = JsonConvert.DeserializeObject<List<EventTypeResult>>(response);

            return eventTypeResults;
        }

        public async Task<List<EventTypeResult>> ListAllEventTypesAsync()
        {
            MarketFilter marketFilter = new MarketFilter();

            return await ListEventTypesAsync(marketFilter);
        }

        public async Task<List<EventResult>> ListEventsAsync(MarketFilter filter, string locale = null)
        {
            var url = _baseUrl + "listEvents/";

            var requestBody = new
            {
                filter = filter,
                locale = locale
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var response = await SendRequest(request, requestBody);

            var eventResults = JsonConvert.DeserializeObject<List<EventResult>>(response);

            return eventResults;
        }


        public async Task<List<EventResult>> ListEventsAsync(string eventTypeId, DateTime from, DateTime to)
        {
            MarketFilter marketFilter = new MarketFilter() { EventTypeIds = new HashSet<string>() { eventTypeId }, MarketStartTime = new TimeRange(from, to) };

            return await ListEventsAsync(marketFilter);
        }

        public async Task<List<MarketCatalogue>> ListMarketCatalogueAsync(MarketFilter filter, ISet<MarketProjection> marketProjection, MarketSort? sort = null, int maxResults = 100, string locale = null)
        {
            var url = _baseUrl + "listMarketCatalogue/";

            var requestBody = new
            {
                filter = filter,
                marketProjection = marketProjection,
                sort = sort,
                maxResults = maxResults,
                locale = locale
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var response = await SendRequest(request, requestBody);

            var marketCatalogues = JsonConvert.DeserializeObject<List<MarketCatalogue>>(response);

            return marketCatalogues;
        }

        public async Task<List<MarketCatalogue>> ListAllMarketCatalogueByEventIdAsync(string eventId)
        {
            MarketFilter marketFilter = new MarketFilter() { EventIds = new HashSet<string> { eventId } };
            int maxResults = 200;
            HashSet<MarketProjection> marketProjections = new HashSet<MarketProjection>() { MarketProjection.COMPETITION, MarketProjection.EVENT, MarketProjection.EVENT_TYPE, MarketProjection.MARKET_DESCRIPTION, MarketProjection.MARKET_START_TIME, MarketProjection.RUNNER_DESCRIPTION, MarketProjection.RUNNER_METADATA };

            return await ListMarketCatalogueAsync(marketFilter, marketProjections, null, maxResults);
        }

        public async Task<List<CompetitionResult>> ListCompetitionsAsync(MarketFilter filter, string locale = null)
        {
            var url = _baseUrl + "listCompetitions/";

            var requestBody = new
            {
                filter = filter,
                locale = locale
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var response = await SendRequest(request, requestBody);

            var competitionResults = JsonConvert.DeserializeObject<List<CompetitionResult>>(response);

            return competitionResults;
        }

        public async Task<List<CompetitionResult>> ListAllFootballCompetitionsAsync()
        {
            MarketFilter marketFilter = new MarketFilter();
            var eventTypes = await ListAllEventTypesAsync();

            marketFilter.EventTypeIds = new HashSet<string> { eventTypes.Single(e => e.EventType.Name == "Soccer").EventType.Id };

            return await ListCompetitionsAsync(marketFilter);
        }

        public async Task<List<MarketBook>> ListMarketBookAsync(List<string> marketIds, PriceProjection priceProjection, OrderProjection orderProjection = OrderProjection.ALL, MatchProjection matchProjection = MatchProjection.ROLLED_UP_BY_AVG_PRICE, bool includeOverallPosition = true, bool partitionMatchedByStrategyRef = false, HashSet<string> customerStrategyRefs = null, string currencyCode = null, string locale = null, DateTime? matchedSince = null, HashSet<string> betIds = null)
        {
            var url = _baseUrl + "listMarketBook/";

            var requestBody = new
            {
                marketIds = marketIds,
                priceProjection = priceProjection,
                orderProjection = orderProjection,
                matchProjection = matchProjection,
                includeOverallPosition = includeOverallPosition,
                partitionMatchedByStrategyRef = partitionMatchedByStrategyRef,
                customerStrategyRefs = customerStrategyRefs,
                currencyCode = currencyCode,
                locale = locale,
                matchedSince = matchedSince?.ToString("yyyy-MM-ddTHH:mm:ssZ"),
                betIds = betIds
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var response = await SendRequest(request, requestBody);

            var marketBooks = JsonConvert.DeserializeObject<List<MarketBook>>(response);

            return marketBooks;
        }

        public async Task<List<MarketBook>> ListMarketBookAsync(List<string> marketIds)
        {

            var priceProjection = new PriceProjection();
            priceProjection.PriceData = new HashSet<PriceData> { PriceData.EX_TRADED, PriceData.EX_BEST_OFFERS };
            priceProjection.Virtualise = true;

            return await ListMarketBookAsync(marketIds, priceProjection);
        }

        public async Task<UpdateInstructionReport> UpdateBetAsync(string betId, string markedId, PersistenceType persistenceType)
        {
            var url = _baseUrl + "updateOrders/";

            var updateOrder = new UpdateInstruction
            {
                BetId = betId,
                NewPersistenceType = persistenceType
            };

            var requestBody = new
            {
                marketId = markedId,
                instructions = new[] { updateOrder },
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var response = await SendRequest(request, requestBody);

            var updateOrderResponse = JsonConvert.DeserializeObject<UpdateExecutionReport>(response);

            if (updateOrderResponse.Status != ExecutionReportStatus.SUCCESS)
            {
                throw new Exception("Bet update failed: " + updateOrderResponse.ErrorCode);
            }

            if (updateOrderResponse.InstructionReports.Count != 1)
            {
                throw new Exception("Unexpected number of instruction reports: " + updateOrderResponse.InstructionReports.Count);
            }
            return updateOrderResponse.InstructionReports.FirstOrDefault();
        }

        public async Task<PlaceExecutionReport> PlaceOrdersAsync(string marketId, List<PlaceInstruction> instructions, string customerRef = null, string customerStrategyRef = null, bool async = false)
        {
            var url = _baseUrl + "placeOrders/";

            var requestBody = new
            {
                marketId = marketId,
                instructions = instructions,
                customerRef = customerRef,
                customerStrategyRef = customerStrategyRef,
                async = async
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var response = await SendRequest(request, requestBody);

            var placeExecutionReport = JsonConvert.DeserializeObject<PlaceExecutionReport>(response);


            if (placeExecutionReport.Status != ExecutionReportStatus.SUCCESS)
            {
                throw new Exception("Bet placement failed: ");
            }

            return placeExecutionReport;
        }


        public async Task<PlaceExecutionReport> PlaceBetAsync(string marketId, long selectionId, Side side, double priceToTrade, double sizeTotrade, double? handicap = null, OrderType orderType = OrderType.LIMIT)
        {
            PlaceInstruction placeInstruction = new PlaceInstruction();
            placeInstruction.SelectionId = selectionId;
            placeInstruction.Handicap = handicap;
            placeInstruction.Side = side;
            placeInstruction.OrderType = orderType;
            placeInstruction.LimitOrder = new LimitOrder() { Price = priceToTrade, Size = sizeTotrade, PersistenceType = PersistenceType.LAPSE };

            return await PlaceOrdersAsync(marketId, new List<PlaceInstruction>() { placeInstruction });
        }

        public async Task<PlaceExecutionReport> FindAndPlaceBetAsync(string homeName, string awayName, string marketName, string runnerName, string eventTypeName, Side side, double price, double size, double? handicap = null)
        {
            MarketCatalogue marketInformation = await getMarketCatalogueAsync(homeName, awayName, marketName, eventTypeName);
            long selectionId = marketInformation.Runners.SingleOrDefault(r => r.RunnerName == runnerName).SelectionId;

            // Placera spelet
            return await PlaceBetAsync(marketInformation.MarketId, selectionId, side, price, size, handicap);
        }

        public async Task<PlaceExecutionReport> FindAndPlaceBetAsync(string homeName, string awayName, string marketName, long selectionId, string eventTypeName, Side side, double price, double size, double? handicap = null)
        {
            MarketCatalogue marketInformation = await getMarketCatalogueAsync(homeName, awayName, marketName, eventTypeName);

            // Placera spelet
            return await PlaceBetAsync(marketInformation.MarketId, selectionId, side, price, size, handicap);
        }

        public async Task<CancelExecutionReport> CancelOrdersAsync(string marketId, List<CancelInstruction> instructions, string customerRef = null)
        {
            var url = _baseUrl + "cancelOrders/";

            var requestBody = new
            {
                marketId = marketId,
                instructions = instructions,
                customerRef = customerRef
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var response = await SendRequest(request, requestBody);

            var cancelExecutionReport = JsonConvert.DeserializeObject<CancelExecutionReport>(response);

            if (cancelExecutionReport.Status != ExecutionReportStatus.SUCCESS)
            {
                throw new Exception("Bet cancel failed: " + cancelExecutionReport.ErrorCode);
            }

            return cancelExecutionReport;
        }


        public async Task<CancelExecutionReport> CancelOrdersAsync(string betId, string marketId)
        {
            var cancelInstruction = new CancelInstruction
            {
                BetId = betId,
                SizeReduction = null
            };

            return await CancelOrdersAsync(marketId, new List<CancelInstruction>() { cancelInstruction });
        }

        public async Task<List<MarketCatalogue>> getMarketCatalogueAsync(string homeName, string awayName, string eventTypeName)
        {
            string eventName = homeName + " v " + awayName;

            var eventTypeResponse = await ListAllEventTypesAsync();

            var eventType = eventTypeResponse.Single(x => x.EventType.Name == eventTypeName);

            var fromDate = DateTime.UtcNow;
            var toDate = fromDate.AddDays(365);
            var eventResponse = await ListEventsAsync(eventType.EventType.Id, fromDate, toDate);

            var eventToBetOn = eventResponse.Single(x => x.Event.Name.Contains(eventName));

            HashSet<MarketProjection> marketProjections = new HashSet<MarketProjection>() { MarketProjection.COMPETITION, MarketProjection.EVENT, MarketProjection.EVENT_TYPE, MarketProjection.MARKET_DESCRIPTION, MarketProjection.MARKET_START_TIME, MarketProjection.RUNNER_DESCRIPTION, MarketProjection.RUNNER_METADATA };
            return await ListMarketCatalogueAsync(new MarketFilter() { EventIds = new HashSet<string>() { eventToBetOn.Event.Id } }, marketProjections);
        }


        public async Task<MarketCatalogue> getMarketCatalogueAsync(string homeName, string awayName, string marketName, string eventTypeName)
        {
            string eventName = homeName + " v " + awayName;

            var eventTypeResponse = await ListAllEventTypesAsync();

            var eventType = eventTypeResponse.Single(x => x.EventType.Name == eventTypeName);

            var fromDate = DateTime.UtcNow;
            var toDate = fromDate.AddDays(30);
            var eventResponse = await ListEventsAsync(eventType.EventType.Id, fromDate, toDate);

            var eventToBetOn = eventResponse.Single(x => x.Event.Name.Contains(eventName));

            HashSet<MarketProjection> marketProjections = new HashSet<MarketProjection>() { MarketProjection.COMPETITION, MarketProjection.EVENT, MarketProjection.EVENT_TYPE, MarketProjection.MARKET_DESCRIPTION, MarketProjection.MARKET_START_TIME, MarketProjection.RUNNER_DESCRIPTION, MarketProjection.RUNNER_METADATA };
            var marketCatalogueResponseList = await ListMarketCatalogueAsync(new MarketFilter() { EventIds = new HashSet<string>() { eventToBetOn.Event.Id } }, marketProjections);

            var marketToBetOn = marketCatalogueResponseList.SingleOrDefault(x => x.MarketName == marketName);

            return marketToBetOn;
        }

        public async Task<List<PriceSize>> GetMarketPriceAndAvailabilityAsync(string homeName, string awayName, string marketName, string runnerName, string eventTypeName, Side side)
        {
            MarketCatalogue marketInformation = await getMarketCatalogueAsync(homeName, awayName, marketName, eventTypeName);
            long selectionId = marketInformation.Runners.SingleOrDefault(r => r.RunnerName == runnerName).SelectionId;

            // Hämta marknadspriserna
            var marketBooks = await ListMarketBookAsync(new List<string>() { marketInformation.MarketId });

            // Hämta priset för det valda spelet
            var marketBook = marketBooks.Single();

            var runner = marketBook.Runners.Single(r => r.SelectionId == selectionId);
            double? lastPriceTraded = runner.LastPriceTraded;

            var priceSize = side == Side.BACK ? runner.ExchangePrices.AvailableToBack : runner.ExchangePrices.AvailableToLay;

            return priceSize;
        }

        public async Task<MarketBook> getAllMarketPrice(string homeName, string awayName, string marketName,string eventTypeName)
        {
            MarketCatalogue marketInformation = await getMarketCatalogueAsync(homeName, awayName, marketName, eventTypeName);

            // Hämta marknadspriserna
            var marketBooks = await ListMarketBookAsync(new List<string>() { marketInformation.MarketId });

            // Hämta priseer för det valda spelet
            return marketBooks.Single();
        }

        public async Task<List<PriceSize>> GetMarketPriceAndAvailabilityAsync(string homeName, string awayName, string marketName, long selectionId, string eventTypeName, Side side)
        {
            MarketCatalogue marketInformation = await getMarketCatalogueAsync(homeName, awayName, marketName, eventTypeName);

            // Hämta marknadspriserna
            var marketBooks = await ListMarketBookAsync(new List<string>() { marketInformation.MarketId });

            // Hämta priset för det valda spelet
            var marketBook = marketBooks.Single();

            var runner = marketBook.Runners.Single(r => r.SelectionId == selectionId);
            double? lastPriceTraded = runner.LastPriceTraded;

            var priceSize = side == Side.BACK ? runner.ExchangePrices.AvailableToBack : runner.ExchangePrices.AvailableToLay;

            return priceSize;
        }

        public async Task<CancelExecutionReport> CancelOrdersAsync(string homeName, string awayName, string marketName, string eventTypeName)
        {
            MarketCatalogue marketInformation = await getMarketCatalogueAsync(homeName, awayName, marketName, eventTypeName);

            List<CurrentOrderSummary> orders = await GetCurrentOrdersAsync(homeName, awayName, marketName,eventTypeName);

            List<CancelInstruction> cancelInstructions = new List<CancelInstruction>();

            foreach (CurrentOrderSummary orderSummary in orders)
            {
                if (orderSummary.SizeRemaining > 0)
                {
                    cancelInstructions.Add(new CancelInstruction
                    {
                        BetId = orderSummary.BetId,
                        SizeReduction = null
                    });
                }

            }

            return await CancelOrdersAsync(marketInformation.MarketId, cancelInstructions);
        }

        public async Task<CurrentOrderSummaryReport> ListCurrentOrdersAsync(ISet<string> betIds = null, ISet<string> marketIds = null, OrderProjection orderProjection = OrderProjection.ALL, TimeRange placedDateRange = null, TimeRange dateRange = null, OrderBy? orderBy = null, SortDir? sortDir = null, int fromRecord = 0, int recordCount = 1000, bool includeItemDescription = false)
        {
            var url = _baseUrl + "listCurrentOrders/";

            var requestBody = new
            {
                betIds = betIds,
                marketIds = marketIds,
                orderProjection = orderProjection,
                placedDateRange = placedDateRange,
                dateRange = dateRange,
                orderBy = orderBy,
                sortDir = sortDir,
                fromRecord = fromRecord,
                recordCount = recordCount,
                includeItemDescription = includeItemDescription
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var response = await SendRequest(request, requestBody);

            var currentOrderSummaryReport = JsonConvert.DeserializeObject<CurrentOrderSummaryReport>(response);

            return currentOrderSummaryReport;
        }

        public async Task<List<CurrentOrderSummary>> GetCurrentOrdersAsync(string homeName, string awayName, string marketName, string evenetTypeName, string runnerName = null)
        {
            MarketCatalogue marketInformation = await getMarketCatalogueAsync(homeName, awayName, marketName, evenetTypeName);

            var currentOrders = (await ListCurrentOrdersAsync(null, new HashSet<string>() { marketInformation.MarketId })).CurrentOrders;

            if (runnerName != null)
            {
                long selectionId = marketInformation.Runners.SingleOrDefault(r => r.RunnerName == runnerName).SelectionId;
                currentOrders = currentOrders.Where(o => o.SelectionId == selectionId).ToList();
            }

            return currentOrders;
        }

        public async Task<CurrentOrderSummaryReport> GetCurrentOrdersAsync()
        {
            return await ListCurrentOrdersAsync();
        }

        public async Task<List<MarketProfitAndLoss>> ListMarketProfitAndLossAsync(HashSet<string> marketIds, bool includeSettledBets = false, bool includeBspBets = false, bool netOfCommission = false)
        {
            var url = _baseUrl + "listMarketProfitAndLoss/";

            var requestBody = new
            {
                marketIds = marketIds,
                includeSettledBets = includeSettledBets,
                includeBspBets = includeBspBets,
                netOfCommission = netOfCommission
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var response = await SendRequest(request, requestBody);

            var marketProfitAndLoss = JsonConvert.DeserializeObject<List<MarketProfitAndLoss>>(response);

            return marketProfitAndLoss;
        }

        public async Task<List<MarketProfitAndLoss>> GetMarketProfitAndLossAsync(string homeName, string awayName, string marketName, string eventTypeName, bool includeSettledBets = false, bool includeBspBets = false, bool netOfCommission = false)
        {
            MarketCatalogue marketInformation = await getMarketCatalogueAsync(homeName, awayName, marketName, eventTypeName);

            var marketIds = new HashSet<string> { marketInformation.MarketId };

            return await ListMarketProfitAndLossAsync(marketIds, includeSettledBets, includeBspBets, netOfCommission);
        }


        private async Task<string> SendRequest(HttpRequestMessage request, Object requestBody)
        {
            request.Content = new StringContent(JsonConvert.SerializeObject(requestBody));
            request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
            request.Headers.Accept.Clear();
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            request.Headers.Add("X-Application", _appKey);
            request.Headers.Add("X-Authentication", _sessionToken);

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.SendAsync(request);
                return await response.Content.ReadAsStringAsync();
            }
        }


        public async Task<ReplaceExecutionReport> ReplaceOrdersAsync(string marketId, List<ReplaceInstruction> instructions, string customerRef = null, bool async = false)
        {
            var url = _baseUrl + "replaceOrders/";

            var requestBody = new
            {
                marketId,
                instructions,
                customerRef,
                async
            };

            var request = new HttpRequestMessage(HttpMethod.Post, url);
            var response = await SendRequest(request, requestBody);

            var replaceExecutionReport = JsonConvert.DeserializeObject<ReplaceExecutionReport>(response);

            if (replaceExecutionReport.Status != ExecutionReportStatus.SUCCESS)
            {
                throw new Exception("Replace orders failed: " + replaceExecutionReport.ErrorCode);
            }

            return replaceExecutionReport;
        }

        public async Task<ReplaceExecutionReport> ReplaceOrdersAsync(string homeName, string awayName, string marketName,string eventTypeName ,string runnerName, Side side ,double newPrice, string customerRef = null, bool async = false)
        {
            MarketCatalogue marketInformation = await getMarketCatalogueAsync(homeName, awayName, marketName, eventTypeName);
            List<CurrentOrderSummary> currentOrders = await GetCurrentOrdersAsync(homeName, awayName, marketName, eventTypeName, runnerName);
            currentOrders = currentOrders.Where(o => o.Side == side).ToList();

            List<ReplaceInstruction> replaceInstructions = new List<ReplaceInstruction>();
            foreach (CurrentOrderSummary orderSummary in currentOrders)
            {
                if (orderSummary.SizeRemaining > 0)
                {
                    replaceInstructions.Add(new ReplaceInstruction
                    {
                        BetId = orderSummary.BetId,
                        NewPrice = newPrice
                    });
                }
            }
            
            return await ReplaceOrdersAsync(marketInformation.MarketId, replaceInstructions, customerRef, async);
        }



    }
}

