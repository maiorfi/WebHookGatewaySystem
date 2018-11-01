namespace WebHookGatewaySystemContracts
{
    public class WebHookPayload
    {
        public string Groups { get; set; }
        public string ContentType { get; set; }
        public string Content { get; set; }
        public string Arguments { get; set; }
        public string WorkingDirectory { get; set; }
    }
}