(Richiede .NET Core 2.1)

### Avviare il client (su windows, mac o linux) con:

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

Esempio #1 (fa aprire il browser sul sito di Repubblica a tutti i client collegati):

```json
{
  "ContentType":"ShellExecute",
  "Content":"http://www.repubblica.it"
}
```

Esempio #2 (lncia Notepad sui client, Windows in questo caso, collegati dei gruppi "Gruppo_1" e "Gruppo_2"):

```json
{
  "ContentType":"ShellExecute",
  "Content":"notepad.exe",
  "Arguments":"c:\\directory-demo\\esempio.txt",
  "Groups":"Gruppo_1,Gruppo_2"
}
```

Esempio #3 (lancia Stickies sui client collegati, MacOS in questo caso, del gruppo "mac"):

```json
{
  "ContentType":"Run",
  "Content":"/Applications/Stickies.app/Contents/MacOS/Stickies",
  "Groups":"mac"
}
```
