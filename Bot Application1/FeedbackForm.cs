using Microsoft.Bot.Builder.FormFlow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Bot_Application1
{
    public class FeedbackForm
    {
        [Prompt(new string[] { "What is your first name?" })]
        public string Name { get; set; }

        //[Prompt("How can Ankit contact you? You can enter either your email id or twitter handle (@something)")]
        //public string Contact { get; set; }

        //[Prompt("What's your feedback?")]
        //public string Feedback { get; set; }

        public static IForm<FeedbackForm> BuildForm()
        {
            return new FormBuilder<FeedbackForm>()
                //.Field(nameof(Contact), validate: ValidateContactInformation)
                //.Field(nameof(Feedback), active: FeedbackEnabled)
                .AddRemainingFields()
                .Build();
        }
        private static Task<ValidateResult> ValidateContactInformation(FeedbackForm state, object response)
        {
            var result = new ValidateResult();
            string contactInfo = string.Empty;
            //if (GetTwitterHandle((string)response, out contactInfo) || GetEmailAddress((string)response, out contactInfo))
            //{
            //    result.IsValid = true;
            //    result.Value = contactInfo;
            //}
            //else
            //{
            //    result.IsValid = false;
            //    result.Feedback = "You did not enter valid email address or twitter handle. Make sure twitter handle starts with @.";
            //}
            return Task.FromResult(result);
        }
        private static bool FeedbackEnabled(FeedbackForm state) =>
    !string.IsNullOrWhiteSpace(state.Contact) && !string.IsNullOrWhiteSpace(state.Name);
    }
}