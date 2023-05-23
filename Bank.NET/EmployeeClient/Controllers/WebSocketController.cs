using CoreClient;
using EmployeeClient.ViewModels;
using Grpc.Core;
using Grpc.Net.Client;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

namespace EmployeeClient.Controllers
{
    public class WebSocketController : Controller
    {
        private readonly BankCoreService.BankCoreServiceClient clientForCore;

        public WebSocketController()
        {
            var portCoreService = Environment.GetEnvironmentVariable("PortCoreService");
            var ipCoreService = Environment.GetEnvironmentVariable("IpCoreService");
            var channelForCore = GrpcChannel.ForAddress($"http://{ipCoreService}:{portCoreService}");
            clientForCore = new BankCoreService.BankCoreServiceClient(channelForCore);
        }

        [Route("/wsBill")]
        public async Task Get()
        {
            if (HttpContext.WebSockets.IsWebSocketRequest)
            {
                using (var webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync())
                {
                    var buffer = new byte[1024];
                    var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
                    var message = Encoding.UTF8.GetString(buffer, 0, result.Count);

                    if (!string.IsNullOrEmpty(message))
                    {
                        long billId = Convert.ToInt64(message);

                        using var streamingCall =
                            clientForCore.GetOperations(new GetOperationsRequest { BillId = billId });
                        try
                        {
                            await foreach (var data in streamingCall.ResponseStream.ReadAllAsync())
                            {
                                List<OperationViewModel> operations = new List<OperationViewModel>();
                                foreach (var operation in data.Operations)
                                {
                                    operations.Add(new OperationViewModel
                                    {
                                        Amount = operation.Amount,
                                        Id = operation.Id,
                                        PerfomedAt = operation.PerformedAt,
                                        ToBillId = operation.ToBillId,
                                        FromBillId = operation.FromBillId,
                                        Status = operation.Status
                                    });
                                }

                                var json = JsonSerializer.Serialize(operations);
                                await webSocket.SendAsync(Encoding.ASCII.GetBytes(json), WebSocketMessageType.Text,
                                    true, CancellationToken.None);
                            }
                        }
                        catch (RpcException ex)
                        {
                            Console.WriteLine("Stream cancelled.");
                        }
                    }
                }
            }
            else
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.BadRequest;
            }
        }
    }
}