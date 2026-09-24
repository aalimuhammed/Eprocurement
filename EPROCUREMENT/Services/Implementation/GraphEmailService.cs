using Azure.Identity;
using EPROCUREMENT.Services.Interfaces;
using EPROCUREMENT.Settings;
using Microsoft.Extensions.Options;
using Microsoft.Graph;
using Microsoft.Graph.Models;
using Microsoft.Graph.Users.Item.SendMail;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EPROCUREMENT.Services.Implementation
{
    public class GraphEmailService : IGraphEmailService
    {
        private readonly GraphSettings _graphSettings;
        private readonly ClientSecretCredential _credential;
        private readonly GraphServiceClient _graphClient;
        public GraphEmailService(IOptions<GraphSettings> graphOptions)
        {
            _graphSettings = graphOptions.Value;

            _credential = new ClientSecretCredential(
                "d9f3a701-c6ac-487c-8640-b6305c099d66",
                "2e223149-dc05-4625-93e4-5e9ff1bcfc94",
                "D9H8Q~Ob1XPGD13SW14~.x4PKEnAtVnLbBcRjaKN");

            var scopes = new[] { "https://graph.microsoft.com/.default" };

            _graphClient = new GraphServiceClient(_credential, scopes);
        }
        public async Task<bool> SendEmailAsync(
            string recipientEmail,
            string subject,
            string body)
        {
            try
            {
                var message = new Message
                {
                    Subject = subject,

                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = body
                    },

                    ToRecipients = new List<Recipient>
                    {
                        new Recipient
                        {
                            EmailAddress = new EmailAddress
                            {
                                Address = recipientEmail
                            }
                        }
                    }
                };

                var requestBody = new SendMailPostRequestBody
                {
                    Message = message,
                    SaveToSentItems = true
                };

                await _graphClient
                    .Users["it-solutions@siac-construction.com"]
                    .SendMail
                    .PostAsync(requestBody);

                return true;
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
