using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Microsoft.Bot.Builder.Dialogs;
using System.Threading.Tasks;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Luis.Models;
using Microsoft.Bot.Builder.Luis;
using Microsoft.Bot.Connector;
using Microsoft.Bot.Builder.FormFlow;
using MySql.Data.MySqlClient;
using System.Data;

namespace HealthBot
{
    [Serializable]
    [LuisModel("e2fc87d7-9220-45a0-b9bc-a4c9df522e34", "978284262e484eab862e94e8bf4a3a3b")]
    public class HealthBotLuisDialog : LuisDialog<object>
    {
        #region declarations
        int tries = 0;
        bool ailselected = false, foodselected = false;
        string food = "", ailment = "";
        Random rand = new Random();
        List<string> pleasantries = new List<string>()
        {
            "I'm doing okay, and you?",
            "I'm fine, thank you",
            "All is well",
            "I'm alright, thank you",
            "Well, I'm a bot. I'm always fine. Lol"
        };
        List<string> singlereplies = new List<string>()
        {
            "Hi, how you doing?",
            "Hello, how may I help?",
            "Hey there, can I be of assistance?",
            "How are you? How may I be of service?"
        };
        List<string> grtsuffix = new List<string>()
        {
            " to you too",
            " to you as well",
            ""
        };
        List<string> thanks = new List<string>()
        {
            "You're welcome",
            "Always at your service",
            "It's nothing",
            "It's alright",
            "My pleasure"
        };
        #endregion

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, LuisResult result)
        {
            string reply = $"I'm sorry. I didn't understand you.\n\n" +
             "I am HealthBot, designed to give you nutritional advice based on your ailment.\n\n" +
             "Please feel free to request any nutritional advice";

            datacon dc = new datacon();

            if (tries++ >= 2)
            {
                string[] aillist = dc.ds.Tables["ailments"].AsEnumerable()
                    .Select(s => s.Field<string>("ailment")).Distinct().ToArray();
                var dialog = new PromptDialog.PromptChoice<string>(
                    aillist,
                    "I'm sorry. I didn't understand you.\r\nPlease select which ailment is applicable to you",
                    "Sorry, that option is not valid",
                    3);

                context.Call(dialog, AfterChoiceAsync);

            }
            else
            {
                await context.PostAsync(reply);
                context.Wait(MessageReceived);
            }

            dc.savequery(result.Query.ToString(), result.TopScoringIntent.Intent, reply);
        }

        //public async Task getchoice(IDialogContext context, IAwaitable<bool> argument)
        //{
        //    var choice = await argument;
        //}
        //public async Task AfterResetAsync()
        //{
        //    var confirm = await argument;
        //    if (confirm)
        //    {
        //        name = "empty";
        //        await context.PostAsync("Reset count.");
        //    }
        //    else
        //    {
        //        await context.PostAsync("Did not reset count.");
        //    }
        //    context.Wait(None);
        //}

        [LuisIntent("Introduction")]
        public async Task Introduction(IDialogContext context, LuisResult result)
        {
            string reply = @"I am HealthBot, designed to give you nutritional advice based on your ailment.";
            await context.PostAsync(reply);
            //await context.PostAsync(@"\r\nPlease tell me your first name");
            //var feedbackForm = new FormDialog<FeedbackForm>(new FeedbackForm(), FeedbackForm.BuildForm, FormOptions.PromptInStart);
            context.Wait(MessageReceived);
            datacon dc = new datacon();
            dc.savequery(result.Query.ToString(), result.TopScoringIntent.Intent, reply);
        }

        [LuisIntent("Single Greetings")]
        public async Task Single_Greetings(IDialogContext context, LuisResult result)
        {
            int index = rand.Next(singlereplies.Count());
            string reply = singlereplies[index];
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
            datacon dc = new datacon();
            dc.savequery(result.Query.ToString(), result.TopScoringIntent.Intent, reply);
        }

        [LuisIntent("Thanks")]
        public async Task Thanks(IDialogContext context, LuisResult result)
        {
            int index = rand.Next(thanks.Count());
            string reply = thanks[index];
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
            datacon dc = new datacon();
            dc.savequery(result.Query.ToString(), result.TopScoringIntent.Intent, reply);
        }

        [LuisIntent("Main request")]
        public async Task Main_request(IDialogContext context, LuisResult result)
        {
            //int index = rand.Next(singlereplies.Count());
            var entities = result.Entities;
            var sentence = result.Query.ToString();

            string[] words = sentence.Split(new char[] { ' ' });
            List<string> foods = new List<string>(), ailments = new List<string>();
            //foreach (EntityRecommendation ntt in entities)
            foreach(string word in words)
            {
                foreach (string s in Phrases.lookuplist())
                {
                    //if (ntt.Entity.ToLower().Contains(s.ToLower()))
                    if (word.ToLower().Contains(s.ToLower()))
                    {
                        string type = Phrases.lookup[Phrases.lookuplist().IndexOf(s), 2];
                        string value = Phrases.lookup[Phrases.lookuplist().IndexOf(s), 1];
                        switch (type)
                        {
                            case "food":
                                foods.Add(value);
                                break;
                            case "ailment":
                                ailments.Add(value);
                                break;
                        }
                        break;
                    }
                }
            }
            string reply = "";
            datacon dcon = new datacon();
            foreach(string food in foods)
            {
                foreach(string ailment in ailments)
                {
                    reply = analyze(food, ailment, dcon);
                    await context.PostAsync(reply);
                }
            }
            if (foods.Count == 0 || ailments.Count == 0)
            {
                reply = "Sorry but based on your response, I do not have enough information to advise you";
                await context.PostAsync(reply);
            }
            context.Wait(MessageReceived);
            dcon.savequery(result.Query.ToString(), result.TopScoringIntent.Intent, reply); //Revisit this
        }

        [LuisIntent("Pleasantries")]
        public async Task Pleasantries(IDialogContext context, LuisResult result)
        {
            int index = rand.Next(pleasantries.Count());
            string reply = pleasantries[index];
            await context.PostAsync(reply);
            context.Wait(MessageReceived);
            datacon dc = new datacon();
            dc.savequery(result.Query.ToString(), result.TopScoringIntent.Intent, reply);
        }

        [LuisIntent("No reply")]
        public async Task No_reply(IDialogContext context, LuisResult result)
        {
            //await context.PostAsync(dc.nm);

            //context.Wait(MessageReceived);


            //PromptDialog.Confirm(context, AfterConfirming_TurnOffAlarm,
            //    "Are you sure?", promptStyle: PromptStyle.None);


            datacon dc = new datacon();
            dc.savequery(result.Query.ToString(), result.TopScoringIntent.Intent, "");
        }

        private async Task AfterChoiceAsync(IDialogContext context, IAwaitable<string> result)
        {
            try
            {
                string userChoice = await result;
                //await context.PostAsync("You chose " + userChoice);
                if (!ailselected && !foodselected)
                {
                    ailselected = true;
                    ailment = userChoice;
                    datacon dc = new datacon();
                    string[] aillist = dc.ds.Tables["foods"].AsEnumerable()
                            .Select(s => s.Field<string>("food")).Distinct().ToArray();
                    var dialog = new PromptDialog.PromptChoice<string>(
                        aillist,
                        $"You chose {userChoice}. \r\nPlease select which food you intend to enquire about",
                        "Sorry, that option is not valid",
                        3);
                    context.Call(dialog, AfterChoiceAsync);
                }
                else if (ailselected && !foodselected)
                {
                    ailselected = false;
                    food = userChoice;
                    //foodselected = true;
                    await context.PostAsync(analyze(food, ailment, new datacon()));
                    context.Wait(MessageReceived);
                }
            }
            catch (TooManyAttemptsException)
            {
                await context.PostAsync("Sorry we could not interprete your request");
                context.Wait(MessageReceived);
            }
        }

        private string analyze(string food, string ailment, datacon dcon)
        {
            var entry = dcon.ds.Tables["advice"].Select($"food = '{food}' and ailment = '{ailment}'");
            int advicescale = entry.Length > 0 ? int.Parse(entry[0]["advice"].ToString()) : -1;

            string advice;
            switch (advicescale)
            {
                default:
                    advice = "Something terrible happened";
                    break;
                case -1:
                    advice = "Sorry I do not have any advice concerning your choices";
                    break;
                case 0:
                    advice = $"It is well okay to take {food} while you have {ailment}";
                    break;
                case 5:
                    advice = $"Please do not take {food} while you have {ailment}. I strongly advise you against that.";
                    break;
            }
            return advice;
        }

        public async Task AfterConfirming_TurnOffAlarm(IDialogContext context, IAwaitable<bool> confirmation)
        {
            if (await confirmation)
            {
                //this.alarmByWhat.Remove(this.turnOff.What);
                await context.PostAsync($"Ok, alarm disabled.");
            }
            else
            {
                await context.PostAsync("Ok! We haven't modified your alarms!");
            }
            context.Wait(MessageReceived);
        }

        [LuisIntent("Greetings")]
        public async Task Greetings(IDialogContext context, LuisResult result)
        {
            int index = rand.Next(grtsuffix.Count());
            string message = result.Query.ToString() + grtsuffix[index];
            await context.PostAsync(message);
            context.Wait(MessageReceived);
            datacon dc = new datacon();
            dc.savequery(result.Query.ToString(), result.TopScoringIntent.Intent, message);
        }

        #region
        //[LuisIntent("Feedback")]
        //public async Task Feedback(IDialogContext context, LuisResult result)
        //{
        //    try
        //    {
        //        await context.PostAsync("That's great. You will need to provide few details about yourself before giving feedback.");
        //        var feedbackForm = new FormDialog<FeedbackForm>(new FeedbackForm(), FeedbackForm.BuildForm, FormOptions.PromptInStart);
        //        context.Call(feedbackForm, FeedbackFormComplete);
        //    }
        //    catch (Exception)
        //    {
        //        await context.PostAsync("Something really bad happened. You can try again later meanwhile I'll check what went wrong.");
        //        context.Wait(MessageReceived);
        //    }
        //}
        //private async Task FeedbackFormComplete(IDialogContext context, IAwaitable<FeedbackForm> result)
        //{
        //    try
        //    {
        //        var feedback = await result;
        //        //string message = GenerateEmailMessage(feedback);
        //        //var success = await EmailSender.SendEmail(recipientEmail, senderEmail, $"Email from {feedback.Name}", message);
        //        //if (!success)
        //        //    await context.PostAsync("I was not able to send your message. Something went wrong.");
        //        //else
        //        //{
        //        //    await context.PostAsync("Thanks for the feedback.");
        //        //    await context.PostAsync("What else would you like to do?");
        //        //}

        //    }
        //    catch (FormCanceledException)
        //    {
        //        await context.PostAsync("Don't want to send feedback? That's ok. You can drop a comment below.");
        //    }
        //    catch (Exception)
        //    {
        //        await context.PostAsync("Something really bad happened. You can try again later meanwhile I'll check what went wrong.");
        //    }
        //    finally
        //    {
        //        context.Wait(MessageReceived);
        //    }
        //}
        #endregion
    }
}