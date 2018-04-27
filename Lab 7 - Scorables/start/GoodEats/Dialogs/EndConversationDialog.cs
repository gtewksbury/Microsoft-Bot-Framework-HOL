using Microsoft.Bot.Builder.Dialogs;
using System;
using System.Threading.Tasks;
using Microsoft.Bot.Connector;
using GoodEats.Models;

namespace GoodEats.Dialogs
{
    /// <summary>
    /// Ends the conversation, resetting the dialog stack and clearing the conversation state.
    /// This dialog should always be invoked via a Forward(...) request (NOT a Call(...)
    /// </summary>
    [Serializable]
    public class EndConversationDialog : IDialog<Reservation>
    {
        public Task StartAsync(IDialogContext context)
        {
            // wait for the response (this should be received automatically via the Forward(...) request
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> message)
        {
            // send a message to user confirming their cancel request
            await context.PostAsync(Properties.Resources.CANCEL_CONFIRMATION);

            // end the conversation (clearing the state and dialog stack)
            context.EndConversation(EndOfConversationCodes.UserCancelled);
        }
    }
}