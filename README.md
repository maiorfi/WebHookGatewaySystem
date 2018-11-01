### Avviare il client (su windws, mac o linux) con:

```dotnet WebHookAgent.dll https://webhookgateway.azurewebsites.net/pusher -a``` (riceve i push destinati a tutti)

oppure con

```dotnet WebHookAgent.dll https://webhookgateway.azurewebsites.net/pusher -g <gruppo1> -g <gruppo2> -g <gruppo3>``` (riceve i push destinati ai gruppi indicati)

### HTTP REST-API

POST https://webhookgateway.azurewebsites.net/api/webhook/pushpayload

Content-Type: application/json

```json
{
  "ContentType":"<ShellExecute/Run>",
  "Content":"<eseguibile, eventualmente con path>",
  "Arguments": "<parametri>",
  "WorkingDirectory":"<directory di lavoro>",
  "Groups":"<se definito e non vuoto, lista separata da virgole dei gruppi destinatari dell'invio push>"
}
```
