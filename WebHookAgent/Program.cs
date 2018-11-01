using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.SignalR.Client;
using Newtonsoft.Json;
using WebHookGatewaySystemContracts;
using Formatting = System.Xml.Formatting;

namespace WebHookAgent
{
    class Program
    {
        [Argument(0, Description = "SignalR Hub Url")]
        [Required]
        public string HubEndpoint { get; }

        [Option("-a|--all", Description = "Subscribe to all pushed payloads")]
        public bool ReceiveAllPayloads { get; }

        [Option("-g|--group", CommandOptionType.MultipleValue, Description = "Subscribe to group (multiple values accepted)")]
        public string[] Groups { get; }

        static void Main(string[] args) => CommandLineApplication.Execute<Program>(args);

        private HubConnection _hubConnection;
        private HubConnectionBuilder _hubConnectionBuilder;

        public async Task OnExecute()
        {
            _hubConnectionBuilder = new HubConnectionBuilder();

            _hubConnection = _hubConnectionBuilder.WithUrl(HubEndpoint).Build();

            _hubConnection.Closed += async (ex) =>
            {
                await Task.Run(async () =>
                {
                    await manageReconnection();
                });
            };

            _hubConnection.On<WebHookPayload>("PayloadPushed", processPayload);

            Console.WriteLine($"Connecting to SignalR Hub...");

            while (true)
            {
                try
                {
                    await _hubConnection.StartAsync();

                    Console.WriteLine($"...connection successful!");

                    await subscribeToGroups();

                    break;
                }
                catch
                {
                    Console.WriteLine("...");

                    await Task.Delay(TimeSpan.FromSeconds(1));
                }
            }


            await Task.Delay(-1);
        }

        private async Task subscribeToGroups()
        {
            if (Groups == null) return;

            foreach (var g in Groups)
            {
                await _hubConnection.InvokeAsync("SubscribeToGroup", g);
            }
        }

        private async Task manageReconnection()
        {
            Console.WriteLine($"Connection to SignalR Hub closed, reconnecting...");

            var reconnectionStartTime = DateTime.Now;

            while (true)
            {
                try
                {
                    await _hubConnection.StartAsync();

                    Console.WriteLine($"...reconnection successful!");

                    await subscribeToGroups();

                    break;
                }
                catch
                {
                    await Task.Delay(TimeSpan.FromSeconds(1));

                    var ts = DateTime.Now - reconnectionStartTime;

                    Console.WriteLine($"...retrying since {ts.Hours} hours, {ts.Minutes} minutes and {ts.Seconds} seconds ago ...");
                }
            }
        }

        public void processPayload(WebHookPayload payload)
        {
            Console.WriteLine($"PayloadPushed : {JsonConvert.SerializeObject(payload, Newtonsoft.Json.Formatting.None)}");

            if (string.IsNullOrEmpty(payload.ContentType)) return;

            try
            {
                innerProcessPayload(payload);
            }
            catch (Exception ex)
            {
                var color = Console.ForegroundColor;

                try
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine(ex);
                }
                finally
                {
                    Console.ForegroundColor = color;
                }

            }
        }

        private static void innerProcessPayload(WebHookPayload payload)
        {
            switch (payload.ContentType)
            {
                case "ShellExecute":
                    {
                        var startInfo = new ProcessStartInfo()
                        {
                            FileName = payload.Content,
                            Arguments = payload.Arguments,
                            UseShellExecute = true
                        };

                        if (!string.IsNullOrEmpty(payload.Arguments)) startInfo.Arguments = payload.Arguments;
                        if (!string.IsNullOrEmpty(payload.WorkingDirectory)) startInfo.WorkingDirectory = payload.WorkingDirectory;

                        var process = new Process { StartInfo = startInfo };

                        process.Start();
                    }

                    break;

                case "Run":
                    {
                        var startInfo = new ProcessStartInfo()
                        {
                            FileName = payload.Content,
                            Arguments = payload.Arguments,
                            UseShellExecute = false
                        };

                        if (!string.IsNullOrEmpty(payload.Arguments)) startInfo.Arguments = payload.Arguments;
                        if (!string.IsNullOrEmpty(payload.WorkingDirectory)) startInfo.WorkingDirectory = payload.WorkingDirectory;

                        var process = new Process { StartInfo = startInfo };

                        process.Start();
                    }

                    break;
            }
        }
    }
}
