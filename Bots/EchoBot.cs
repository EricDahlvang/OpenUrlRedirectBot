// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License.
//
// Generated with Bot Builder V4 SDK Template for Visual Studio EchoBot v4.9.2

using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Schema;
using Newtonsoft.Json;

namespace OpenUrlRedirectBot.Bots
{
    public class EchoBot : ActivityHandler
    {
        HttpContext _currentContext;

        public EchoBot(IHttpContextAccessor context)
        {
            _currentContext = context.HttpContext;
        }

        protected override async Task OnMessageActivityAsync(ITurnContext<IMessageActivity> turnContext, CancellationToken cancellationToken)
        {
            var replyText = $"Echo: {turnContext.Activity.Text}";
            await turnContext.SendActivityAsync(MessageFactory.Text(replyText, replyText), cancellationToken);
        }

        protected override async Task OnMembersAddedAsync(IList<ChannelAccount> membersAdded, ITurnContext<IConversationUpdateActivity> turnContext, CancellationToken cancellationToken)
        {
            foreach (var member in membersAdded)
            {
                if (member.Id != turnContext.Activity.Recipient.Id)
                {
                    var card = CreateAdaptiveCardAttachment();
                    var response = MessageFactory.Attachment(card);
                    await turnContext.SendActivityAsync(response, cancellationToken);
                }
            }
        }

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
    }
}
