using System;
using System.Collections.Generic;
using System.IO;
using System.Diagnostics;
using Binance.Net;
using Binance.Net.Objects;
using CryptoExchange.Net.Authentication;
using CryptoExchange.Net.Logging;
using BitMEX;
using IO.Swagger.Client;
using IO.Swagger.Api;
using IO.Swagger.Model;

namespace Test
{
    internal class Program
    {
        //private static string bitmexKey = "yZbzmBFF2l57u-glfYoF1iPj";
        //private static string bitmexSecret = "ypUIm4YJo7L619xdovE5jeO04AYpFd6LSD91GRCbqnfRV5_m";
        private static string bitmexKey = "ULpOOUDzDYJQOnE6GIalmLM3";
        private static string bitmexSecret = "I-d_k3pb37yQNbCNFdTggs7fWXw_1mq1yt1Ju92BrIWn_xqD";

        private static string BINANCE_APIKEY = "7ZKTD77opUdDvxVFIGxivHwbHFe1fCEH2mHWRywRafhYXyx19r0uaWJJhP6qQpnP";
        private static string BINANCE_APISECRET = "jwoEtuKVUXFcT3JBtMT7PT8qAkWZyiRBCJ9k1HMIW5SvOdx15IrHN48kC038P506";

        private static void Main(string[] args)
        {
            Program p = new Program();
            p.Run(args);

            //BinanceTest();
        }

        private void Run(string[] args)
        {
            BitMEXApi bitmex = new BitMEXApi(bitmexKey, bitmexSecret);
            //var orderBook = bitmex.GetOrderBook("XBTUSD", 3);
            var orders = bitmex.GetOrders();
            //var orders = bitmex.DeleteOrders();
            Console.WriteLine(orders);
        }

        private void SwaggerTest()
        {
            // Configure API key authorization: apiKey
            Configuration.Default.ApiKey.Add("api-key", "BITMEX_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.ApiKeyPrefix.Add("api-key", "Bearer");
            // Configure API key authorization: apiNonce
            Configuration.Default.ApiKey.Add("api-nonce", "BITMEX_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.ApiKeyPrefix.Add("api-nonce", "Bearer");
            // Configure API key authorization: apiSignature
            Configuration.Default.ApiKey.Add("api-signature", "BITMEX_API_KEY");
            // Uncomment below to setup prefix (e.g. Bearer) for API key, if needed
            // Configuration.Default.ApiKeyPrefix.Add("api-signature", "Bearer");

            //Configuration.Default.AccessToken

            var apiInstance = new APIKeyApi();
            var apiKeyID = "BITMEX_API_KEY";  // string | API Key ID (public component).

            try
            {
                // Disable an API Key.
                APIKey result = apiInstance.APIKeyDisable(apiKeyID);
                Debug.WriteLine(result);
            }
            catch (Exception e)
            {
                Debug.Print("Exception when calling APIKeyApi.APIKeyDisable: " + e.Message);
            }
        }

        private static void BinanceTest()
        {
            BinanceClient.SetDefaultOptions(new BinanceClientOptions()
            {
                ApiCredentials = new ApiCredentials(BINANCE_APIKEY, BINANCE_APISECRET),
                LogVerbosity = LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { Console.Out }
            });
            BinanceSocketClient.SetDefaultOptions(new BinanceSocketClientOptions()
            {
                ApiCredentials = new ApiCredentials(BINANCE_APIKEY, BINANCE_APISECRET),
                LogVerbosity = LogVerbosity.Debug,
                LogWriters = new List<TextWriter> { Console.Out }
            });

            using (var client = new BinanceClient())
            {
                // Public
                var ping = client.Ping();
                var exchangeInfo = client.GetExchangeInfo();
                var serverTime = client.GetServerTime();
                var orderBook = client.GetOrderBook("BNBBTC", 10);
                var aggTrades = client.GetAggregatedTrades("BNBBTC", startTime: DateTime.UtcNow.AddMinutes(-2), endTime: DateTime.UtcNow, limit: 10);
                var klines = client.GetKlines("BNBBTC", KlineInterval.OneHour, startTime: DateTime.UtcNow.AddHours(-10), endTime: DateTime.UtcNow, limit: 10);
                var price = client.GetPrice("BNBBTC");
                var prices24h = client.Get24HPrice("BNBBTC");
                var allPrices = client.GetAllPrices();
                var allBookPrices = client.GetAllBookPrices();
                var historicalTrades = client.GetHistoricalTrades("BNBBTC");

                // Private
                var openOrders = client.GetOpenOrders("BNBBTC");
                var allOrders = client.GetAllOrders("BNBBTC");
                var testOrderResult = client.PlaceTestOrder("BNBBTC", OrderSide.Buy, OrderType.Limit, 1, price: 1, timeInForce: TimeInForce.GoodTillCancel);
                var queryOrder = client.QueryOrder("BNBBTC", allOrders.Data[0].OrderId);
                var orderResult = client.PlaceOrder("BNBBTC", OrderSide.Sell, OrderType.Limit, 10, price: 0.0002m, timeInForce: TimeInForce.GoodTillCancel);
                var cancelResult = client.CancelOrder("BNBBTC", orderResult.Data.OrderId);
                var accountInfo = client.GetAccountInfo();
                var myTrades = client.GetMyTrades("BNBBTC");

                // Withdrawal/deposit
                var withdrawalHistory = client.GetWithdrawHistory();
                var depositHistory = client.GetDepositHistory();
                var withdraw = client.Withdraw("ASSET", "ADDRESS", 0);
            }

            var socketClient = new BinanceSocketClient();
            // Streams
            var successDepth = socketClient.SubscribeToDepthStream("bnbbtc", (data) =>
            {
                // handle data
            });
            var successTrades = socketClient.SubscribeToTradesStream("bnbbtc", (data) =>
            {
                // handle data
            });
            var successKline = socketClient.SubscribeToKlineStream("bnbbtc", KlineInterval.OneMinute, (data) =>
            {
                // handle data
            });
            var successTicker = socketClient.SubscribeToAllSymbolTicker((data) =>
            {
                // handle data
            });
            var successSingleTicker = socketClient.SubscribeToSymbolTicker("bnbbtc", (data) =>
            {
                // handle data
            });

            string listenKey;
            using (var client = new BinanceClient())
                listenKey = client.StartUserStream().Data;

            var successAccount = socketClient.SubscribeToUserStream(listenKey, data =>
            {
                // Handle account info data
            },
                data =>
                {
                    // Handle order update info data
                });
            socketClient.UnsubscribeAll();

            Console.ReadLine();
        }
    }
}
