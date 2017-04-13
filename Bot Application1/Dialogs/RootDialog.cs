using System;
using System.Threading.Tasks;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Connector;

namespace HealthBot.Dialogs
{
    [Serializable]
    public class RootDialog : IDialog<object>
    {
        int prev = 0;
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<object> result)
        {
            var activity = await result as Activity;

            // calculate something for us to return
            int length = (activity.Text ?? string.Empty).Length;

            // return our reply to the user
            string reply = $"You sent {activity.Text} which was {length} characters. ";
            reply += prev == 0 ? "" : $"You previously sent {prev} characters";
            prev = length;
            await context.PostAsync(reply);

            context.Wait(MessageReceivedAsync);
        }

        public enum cars
        {
            Ford,
            Mercedes_Benz,
            Toyota,
            Honda,
            Mazda,
            Lexus
        }
    }
}