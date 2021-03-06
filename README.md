# OpenUrlRedirectBot

Steps to enable redirecting OpenUrl links:

- Add a RedirectController:

```cs
    [Route("redirect")]
    [ApiController]
    public class RedirectController : ControllerBase
    {
        [HttpGet]
        public IActionResult Get(string url)
        {
            // TODO: Log url here
            return Redirect(url);
        }
    }
```

- Update Startup.cs ConfigureServices:
```cs
    services.AddHttpContextAccessor();
```

- Inject IHttpContextAccessor into the class where it will be used:
```cs
    HttpContext _currentContext;

    public EchoBot(IHttpContextAccessor context)
    {
        _currentContext = context.HttpContext;
    }
```

- Templatize the Adaptive Card:
```json
{
  "$schema": "http://adaptivecards.io/schemas/adaptive-card.json",
  "type": "AdaptiveCard",
  "version": "1.2",
  "body": [
    {
      "type": "TextBlock",
      "text": "This card's action will open a URL"
    }
  ],
  "actions": [
    {
      "type": "Action.OpenUrl",
      "title": "Microsoft",
      "url": "[baseurl]?url=https://www.microsoft.com"
    },
    {
      "type": "Action.OpenUrl",
      "title": "Google",
      "url": "[baseurl]?url=https://www.google.com"
    },
    {
      "type": "Action.OpenUrl",
      "title": "Amazon",
      "url": "[baseurl]?url=https://www.amazon.com"
    }
  ]
}
    
```

- Replace the `[baseurl]` with the redirect controller path:
```cs
    private Attachment CreateAdaptiveCardAttachment()
    {
        var cardResourcePath = "OpenUrlRedirectBot.Resources.openUrlCard.json";

        using (var stream = GetType().Assembly.GetManifestResourceStream(cardResourcePath))
        {
            using (var reader = new StreamReader(stream))
            {
                var adaptiveCard = reader.ReadToEnd();

                string baseUrl = $"{_currentContext.Request.Scheme}://{_currentContext.Request.Host.Value}/redirect";

                adaptiveCard = adaptiveCard.Replace("[baseurl]", baseUrl);

                return new Attachment()
                {
                    ContentType = "application/vnd.microsoft.card.adaptive",
                    Content = JsonConvert.DeserializeObject(adaptiveCard),
                };
            }
        }
    }
```
