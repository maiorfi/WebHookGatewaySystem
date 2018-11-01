using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace WebHookGateway.SignalrHubs
{
    public class PayloadPusherHub : Hub
    {
        private readonly IConfiguration Configuration;
        private readonly IHttpContextAccessor HttpContextAccessor;
        private readonly ILogger Logger;

        public static class CallbackNames
        {
            public static string PayloadPushed => "PayloadPushed";
        }

        public PayloadPusherHub(IConfiguration configuration, IHttpContextAccessor httpContextAccessor, ILogger<PayloadPusherHub> logger)
        {
            Configuration = configuration;
            HttpContextAccessor = httpContextAccessor;
            Logger = logger;
        }

        public async Task SubscribeToGroup(string groupName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
        }

        public async Task UnsubscribeFromGRoup(string groupName)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, groupName);
        }
    }
}