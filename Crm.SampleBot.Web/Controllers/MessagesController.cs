using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Linq;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.Dialogs;
using Crm.SampleBot.Dialogs;
using System;

namespace Crm.SampleBot.Web
{
    [Route("api/messages")]
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        private IDialogFactory dialogFactory;

        public MessagesController(IDialogFactory dialogFactory)
        {
            this.dialogFactory = dialogFactory;
        }

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                var dialog = dialogFactory.Create<RootDialog>();
                await Conversation.SendAsync(activity, () => dialog);
            }
            else
            {
                await this.HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private async Task<Activity> HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
                await this.WelcomeNewUserAsync(message);



            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }

        private async Task WelcomeNewUserAsync(Activity activity)
        {
            if (activity.MembersAdded.Any(m => m.Id == activity.Recipient.Id))
            {
                var connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                var response = activity.CreateReply();
                response.Text = "Hello! I am the Ecolab CRM bot. I can help you do things like find an order status, or find a service representative. Where should we start?";
                await connector.Conversations.ReplyToActivityAsync(response);
            }
        }
    }
}