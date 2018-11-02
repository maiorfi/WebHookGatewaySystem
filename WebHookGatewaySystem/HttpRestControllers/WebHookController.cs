using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MlkPwgen;
using Newtonsoft.Json;
using WebHookGateway.Services;
using WebHookGatewaySystemContracts;

namespace WebHookGateway.HttpRestControllers
{
    [Route("api/[controller]")]
    [ApiController]
    [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
    [Produces("application/json")]
    public class WebHookController : ControllerBase
    {
        private readonly ILogger Logger;
        private readonly GatewayService GatewayService;

        public WebHookController(ILogger<WebHookController> logger, GatewayService gatewayService)
        {
            Logger = logger;
            GatewayService = gatewayService;
        }

        [HttpPost]
        [Route("PushPayload")]
        public async Task<ActionResult<string>> PushPayload([FromBody] WebHookPayload payload)
        {
            using (Logger.BeginScope($"WHG-{PronounceableGenerator.Generate()}"))
            {
                if (payload == null)
                {
                    Logger.LogError("Deserializzazione parametro fallita");
                    throw new ArgumentNullException(nameof(payload));
                }

                Logger.LogTrace("PushPayload chiamato con payload : {PAYLOAD}", JsonConvert.SerializeObject(payload, Formatting.None));

                await GatewayService.PushPayload(payload);

                return Ok(new { });
            }
        }
    }
}