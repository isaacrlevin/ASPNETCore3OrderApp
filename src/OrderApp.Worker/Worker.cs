using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Grpc.Net.Client;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using OrderApp.App.Services;
using OrdersHandler;

namespace OrdersHandler.Worker
{
    public class Worker : BackgroundService
    {
        private readonly ILogger<Worker> _logger;
        private DatabaseService _databaseService;

        public Worker(ILogger<Worker> logger)
        {
            _logger = logger;
            _databaseService = new DatabaseService();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Checking orders at {DateTimeOffset.Now}");
                try
                {
                    //var httpClient = new HttpClient();
                    // The port number(5001) must match the port of the gRPC server.
                    //httpClient.BaseAddress = new Uri("https://localhost:5001");
                    var channel = GrpcChannel.ForAddress("https://localhost:5001");
                    var client = new OrdersHandler.OrdersManager.OrdersManagerClient(channel);
                    OrderRequest request = new OrderRequest();
                    OrderReply result = await client.GetNewOrderAsync(request);
                    if (result.OrderId != 0)
                    {
                        _databaseService.UpdateOrder(result.OrderId);
                        _logger.LogInformation($"Order with id {result.OrderId} has been processed");
                    }
                    else
                    {
                        _logger.LogInformation($"No pending orders at {DateTimeOffset.Now}");
                    }
                }
                catch (Exception exc)
                {
                    _logger.LogError(exc.Message + exc.StackTrace);
                }

                await Task.Delay(3000, stoppingToken);
            }
        }
    }
}
