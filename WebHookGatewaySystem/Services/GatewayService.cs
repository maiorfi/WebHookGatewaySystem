using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WebHookGateway.SignalrHubs;
using WebHookGatewaySystemContracts;

namespace WebHookGateway.Services
{
    public class GatewayService
    {
        private readonly ILogger Logger;
        private readonly IHubContext<PayloadPusherHub> PayloadPusherHubContext;

        public GatewayService(ILogger<GatewayService> logger, IHubContext<PayloadPusherHub> payloadPusherHubContext)
        {
            Logger = logger;
            PayloadPusherHubContext = payloadPusherHubContext;
        }

        public void Initialize()
        {
            Logger.LogTrace($"GatewayService Initialized");
        }

        internal async Task PushPayload(WebHookPayload payload)
        {
            Logger.LogTrace("PushPayloadToAll chiamato con payload : {PAYLOAD}", JsonConvert.SerializeObject(payload, Formatting.None));

            if (string.IsNullOrEmpty(payload.Groups))
            {
                await PayloadPusherHubContext.Clients.All.SendAsync(PayloadPusherHub.CallbackNames.PayloadPushed, payload);
            }
            else
            {
                await PayloadPusherHubContext.Clients.Groups(payload.Groups.Split(new []{','},StringSplitOptions.RemoveEmptyEntries).Select(g=>g.Trim()).ToArray()).SendAsync(PayloadPusherHub.CallbackNames.PayloadPushed, payload);
            }
        }
    }
}
