
using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using BetfairAPI;
using BetfairAPI.BFExchange;

//using BetfairApi.Responses;
//using BetfairApi.Requests;/
//using BetfairApi.Requests.Json;


namespace Betfair
{
    public class BetfairRequest
    {
        private string _appKey;
        private string _username;
        private string _password;
        private string _certPath;
        private string _certPassword;
        private string _accountId;
        private Session _session;
        
        public BetfairRequest(string appKey, string username, string password, string certPath, string certPassword, string accountId)
        {
            _appKey = appKey;
            _username = username;
            _password = password;
            _certPath = certPath;
            _certPassword = certPassword;
            _accountId = accountId;

            // Skapa en session med Betfair API.
            X509Certificate2 cert = new X509Certificate2(_certPath, _certPassword);
            _session = new Session(_appKey, cert, _username, _password);
        }

        public void PlaceOrder(string selectionId, double price, double size, string marketId)
        {
            // Skapa en platsorderbegäran.
            PlaceOrdersRequest placeOrdersRequest = new PlaceOrdersRequest()
            {
                MarketId = marketId,
                Instructions = new PlaceInstruction[]
                {
                    new PlaceInstruction()
                    {
                        SelectionId = selectionId,
                        Side = Side.BACK,
                        OrderType = OrderType.LIMIT,
                        LimitOrder = new LimitOrder()
                        {
                            Size = size,
                            Price = price,
                            PersistenceType = PersistenceType.PERSIST
                        }
                    }
                },
                CustomerRef = Guid.NewGuid().ToString(),
            };

            // Placera ordern på Betfair API.
            PlaceExecutionReport placeExecutionReport = _session.V3.PlaceOrders(placeOrdersRequest);

            if (placeExecutionReport.Status == ExecutionReportStatus.FAILURE)
            {
                throw new Exception("Fel vid placering av vadslagning på Betfair: " + placeExecutionReport.ErrorCode);
            }
        }
    }
}
