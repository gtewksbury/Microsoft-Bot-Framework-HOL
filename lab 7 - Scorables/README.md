# Lab 7 - Scorables
Throughout the labs, we've learned that bot applications can be made of multiple **Dialogs**, and all user responses are passed to the top (or active) **Dialog** in the **DialogStack**.  But what if you want to globally handle incoming messages from users, regardless of the current active **Dialog**?  For example, if the user types *'help'*, we should provide help options regardless of the current **Dialog**.  Another example would be allowing the user to *'cancel'* any point in a conversation.

In this short lab, we'll learn how we can use **Scorables** to introduce global message handlers throughout our bot application.

## Scorables
**Scorables** provide a way of intercepting incoming messages prior to being passed to the active **Dialog**.  When a **Scorable** handles a request, the request will NOT be forwarded to the active **Dialog**.  We're going to create a *CancelScorable* which will end the conversation when the user types *Cancel*, *Nevermind*, or *Forget it*.  

> Your application can have many **Scorables** to handle different situations.  The Bot Framework will execute the **Scorable** with the highest *score* for the given response.  If none of the **Scorables** are equipped to handle a response, it will be passed to the active **Dialog**.

Let's open the starter solution in Visual Studio (or you can use you completed solution from the last lab).  Add a *Scorables* directory to your project, add a new *CancelScorable.cs* file to the new directory, and replace its contents with the following code:

```csharp

using Autofac;
using GoodEats.Dialogs;
using GoodEats.Models;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Builder.Scorables.Internals;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace GoodEats.Scorables
{
    public class CancelScorable : ScorableBase<IActivity, string, double>
    {
        private readonly IDialogTask DialogTask;

        public CancelScorable(IDialogTask dialogTask)
        {
            SetField.NotNull(out this.DialogTask, nameof(dialogTask), dialogTask);
        }

        protected override async Task<string> PrepareAsync(IActivity activity, CancellationToken token)
        {
            // this is the first method to execute in the scorable workflow
            // this method simply returns the received message if it matches any of the
            // following keywords
            var message = activity as IMessageActivity;
            var values = new string[] { "cancel", "nevermind", "never mind", "forget it","forgetit" };

            if (message != null && !string.IsNullOrWhiteSpace(message.Text))
            {
                if (values.Contains(message.Text, StringComparer.InvariantCultureIgnoreCase))
                {
                    // if the user typed any of the above commands, this scorable
                    // will be scored and potentially executed prior to any dialogs
                    return message.Text;
                }
            }

            return null;
        }

        protected override bool HasScore(IActivity item, string state)
        {
            // each registered scorable (through the ScoreableModule in this solution) will be
            // executed and scored (between 0 and 1).  The scorable with the highest score will be executed
            return state != null;
        }

        protected override double GetScore(IActivity item, string state)
        {
            // if we matched the cancel keywords exectly, give this the top score of 1
            return 1.0;
        }

        protected override async Task PostAsync(IActivity item, string state, CancellationToken token)
        {
          // if this scorable wins and executes, simply call the EndOfConversation dialog
          // which in turn will end the conversation.  I couldn't find a way of successfully
          // ending the conversation directly from here, so I push it to the dialog
          await this.DialogTask.Forward(new EndConversationDialog(), null, item as IMessageActivity, token);
        }


        protected override Task DoneAsync(IActivity item, string state, CancellationToken token)
        {
            return Task.CompletedTask;
        }
    }
}
```

Let's take a look at what this code is doing.

#### PrepareAsync
First,the *PrepareAsync* method receives a message and checks if the message content equals different variations of *Cancel*, *Nevermind*, or *Forget It*.  If the message content equals any of these values, it returns the the message text, otherwise, it returns null (this is setting the *state* value that get's passed to the other methods).

#### HasScore
This is the next method to be called in the pipeline, and returns true if the provided *state* from the PrepareAsync method is NOT Null (for us, that means the user typed *Cancel*, *Nevermind*,  or *Forget It*).

#### GetScore
Next, if *HasScore* equals *true*, the Bot Framework will call *GetScore* which should return a value between 0 and 1.  In our case, we return the highest score possible of 1 if the user typed *Cancel*, *Nevermind*, or *Forget It*.  The reason you can return a range between 0 and 1 is because your bot application can have many **Scorables**.  After it determines all **Scorables** that have a score for the current request, and executes the **Scorable** with the highest score.  If none of your **Scorables** have a score for the current request, the message is passed to the active **Dialog**.

#### PostAsync
Once the Bot Framework finds the highest-scoring **Scorable**, it calls it's *PostAsync* method.  This is where your code should actually *do something* with the message.  In our case, we're *Forwarding* to a new *EndConverationDialog*.  

### EndConverationDialog
Let's go ahead and add the EndConversationDialog to our solution.  Open the *Dialogs* directory, add a new code or class file named *EndConversationDialog.cs*, and replace the contents with the following code:

```csharp

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
```

Since we *Forwarded* to this **Dialog** (instead of *Calling*), the MessageReceivedAsync method will be called immediately, which in turn post a friending message to the user and ends the conversation.

### Dependency Injection
The .NET Bot Builder uses AutoFac internally to register and resolve dependencies throughout the framework.  While outside the scope of this tutorial, if you haven't worked with *AutoFac* or Dependency Injection and IoC containers before, you can learn more about it on [AutoFac's Website](https://autofac.org/).

We need to register our *CancelScorable* with .NET Bot Builder's IoC container so that it can discover and execute our **Scorable**.

#### ScorableModule
Create a new class or code file named *ScorableModule* in the *Modules* directory and replace the contents with the following code:

```csharp

using Autofac;
using GoodEats.Scorables;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Scorables;
using Microsoft.Bot.Connector;

namespace GoodEats.Modules
{
    public class ScorableModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            // register the scorable module with the autofac container
            builder
            .Register(c => new CancelScorable(c.Resolve<IDialogTask>()))
            .As<IScorable<IActivity, double>>()
            .InstancePerLifetimeScope();
        }
    }
}
```

#### Register ScorableModule
Next we need to register the new *ScorableModule* with .NET Bot Builder's IoC container.  To do so, open your *Global.ascx.cs* file, and replace the contents with the following code:

```csharp

using Autofac;
using Autofac.Integration.WebApi;
using GoodEats.Modules;
using Microsoft.Bot.Builder.Azure;
using Microsoft.Bot.Builder.Dialogs;
using Microsoft.Bot.Builder.Dialogs.Internals;
using Microsoft.Bot.Builder.Internals.Fibers;
using Microsoft.Bot.Connector;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Http;
using System.Web.Routing;

namespace GoodEats
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            Conversation.UpdateContainer(
                builder =>
                {
                    builder.RegisterModule(new AzureModule(Assembly.GetExecutingAssembly()));
                    builder.RegisterModule<ScorableModule>();
                });
            GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}
```

Let's give it a try.  Go ahead and run your solution along with the emulator and type `I'd like to reserve a table in Pittsburgh`.  When the bot prompts you for a preferred cuisine, type `nevermind`.

You're bot should have sent you a friendly *Good-Bye* message before it ended the conversation.  This should work regardless of your current position in the **conversation**.  This time, enter information up until the point when the bot asks you to confirm your reservation.  At that point, type `cancel`.  You should see the same result!

## Quick Recap
In this short lab, we learned how to create and register **Scorables** to handle messages globally throughout our bot application.

## Next Steps
In [Lab 8](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/tree/master/lab%208%20-%20Azure%20Bot%20Services), we'll learn how to deploy and host our bot within Microsoft Azure.


