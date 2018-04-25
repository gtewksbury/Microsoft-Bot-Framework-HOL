# Bot Framework Setup

In this lab, we'll setup our Visual Studio .NET Bot Framework development environment.  This includes the following:

1.	Installing the Bot Emulator (used for testing our bot locally)
2.	Adding the Visual Studio 2017 Bot Framework project template
3.	Create and running our first *Hello World* bot project
4.	A quick review of the Bot Builder SDK for .NET

> Note, the following labs will require Visual Studio 2017 (preferably with the latest updates)


## Installing the Bot Emulator
In a production world, user's interface with your Bots through one or many configured **Channels**.  These channels represent the different *interfaces* that end-users can use to interact with your bot application.  One of the power features of Bot Framework is the ability to code once for multiple channels.  Currently, the supported channels include the following:

*	Cortana
*	Bing
*	Skype
*	Microsoft Teams
*	Web Chat
*	Email
*	GroupMe
*	Facebook
*	Kik
*	Slack
*	Telegram
*	SMS (Twilio)

However, none of these **channels** support a local debugging experience.  Luckily for us, Microsoft has created a Bot Emulator desktop client that can serve as a testing and debugging **channel** as you develop your bot application!


![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%201%20-%20Setup/images/bot-emulator.png)


## Installing Visual Studio Bot Framework Project Templates

Before you build your first .NET Bot Application, you'll want to install the Visual Studio Bot Framework Project Template.  This will give you a nice starting point with a working sample you can run immediately.

> Note, the following labs have been developed using Visual Studio 2017.  Additionally, the Bot Builder SDK for .NET requires .NET Framework 4.6 or higher.


To install the Visual Studio Bot Framework Project Template, download the [Bot Application](http://aka.ms/bf-bc-vstemplate) zip file and save it to your Visual Studio project templates directory

> For typical Visual Studio installations, the *project templates* directory is located at ` %USERPROFILE%\Documents\Visual Studio 2017\Templates\ProjectTemplates\Visual C#\ `
 
Next, download [Bot Controller](http://aka.ms/bf-bc-vscontrollertemplate) and [Bot Dialog](http://aka.ms/bf-bc-vsdialogtemplate) zip files and save them to your Visual Studio item templates directory

> For typical Visual Studio installations, the *item templates* directory is located at ` %USERPROFILE%\Documents\Visual Studio 2017\Templates\ItemTemplates\Visual C#\ `

More information on installing .NET Bot Framework Visual Studio templates can be found [here](https://docs.microsoft.com/en-us/azure/bot-service/dotnet/bot-builder-dotnet-quickstart).

Once the templates has been added successfully, open Visual Studio 2017 (if Visual Studio was previously open, you might have to close it and re-open it for Visual Studio to find your templates).

Navigate to *File* > *New Project* and select *Visual C#* in the *New Project* dialog, and select *Bot Application* (if you don't see this option, you might have to check the template download locations in the previous step).


![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%201%20-%20Setup/images/vs2017-project.png)

At this point you should be all set to start developing your bot application.  Before we run it though, let's take a moment to review the project structure.

> If you're impatient like me, you might try to immediately run the project.  Just make sure you're connected to the internet as project template has a number of *nuget* packages that must be downloaded, including the *Bot Builder SDK*.

## Basic Project Structure

We're almost ready to run our bot and start and interact with it via the Bot Emulator, but before we do, let's take a brief moment to review the Visual Studio project that was created.


![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%201%20-%20Setup/images/vs2017-explorer.png)

If you've done any web development with Visual Studio, you'll probably notice Visual Studio created a new ASP.NET Web API project.  **Channels** invoke our bot application via RESTful Post calls to our *Messages* endpoint, passing an *Activity* in the body of the request.  Using open HTTP standards allows Bot Framework to be integrated with nearly any platform!  Let's open *MessagesController.cs* and take a look.

```csharp

        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new Dialogs.RootDialog());
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
``` 

  We can also see that the controller invokes a new *RootDialog*.  Let take a quick look at *RootDialog* to see what's going on there.

```csharp
    public class RootDialog : IDialog<object>
    {
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
            await context.PostAsync($"You sent {activity.Text} which was {length} characters");

            context.Wait(MessageReceivedAsync);
        }
    }
``` 

Immediately we see a few things that are probably new to us.  Our class implements  something called *IDialog* and we have a couple methods that seem to be getting passed an *IDialogContext*.  These are some of the fundamental building blocks available within the .NEt Bot Builder SDK.  No better way to see how things work than to step through the code, so let's run the project and see it in action!

## Testing your Bot Application

Go ahead an run your Visual Studio project in *Debug* mode.  You should notice a new window open in your defualt browser.  Make note of the URL and port in your browser.

> If you receive any build errors, make sure you are connected to the internet and Visual Studio is able to download nuget packages

Let's open our Bot Emulator.  Copy the url from the browser into the Bot Emulator and append */api/messages* (the url should be http://localhost:3979/api/messages) and click *Connect*.

> You'll notice inputs for *Microsoft App ID* and *Microsoft App Password*.  You can leave those blank for now as they are not required for local debugging.


![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%201%20-%20Setup/images/bot-emulator-address.png)

On the Emulator's right pane, you notice some information was output to the *Log* section.  The log will provide you with the call stack and any potential exceptions that might arise within your code.  Go ahead an click on the *POST* link of one of the entries.  Above the *Logs*, you can see detailed information about the request (or exception if one occurs).

> Note, if you see error messages similar to below, you probably forgot to run your Visual Studio project, or entered the wrong address into the Emulator

![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%201%20-%20Setup/images/bot-emulator-error.png)

Go back to the *RootDialog* in Visual Studio and put a breakpoint on the *StartAsync* method and *MessageReceived* method.

![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%201%20-%20Setup/images/bot-visual-studio-breakpoints.png)

Now let's go back to the Emulator and type a message to our Bot and click *Enter*.


![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%201%20-%20Setup/images/bot-emulator-type-message.png)

You should notice that our *StartAsync* method breakpoint hit.  When you start a new conversation, Bot Framework will first call the *StartAsync* method of your *RootDialog* invoked via the *MessagesController*.  Notce in this example, that the *StartAsync* method immediately calls *IDialogContext.Wait(MessageReceived)*.  This tells Bot Framework that the provided message (or in our case, *Activity*) should be passed to the *MessageReceived* handler.  Let's hit F5 and see what happens next.

As you might have expected, our MessageReceived handler was invoked.  You can also see that after computing the message size, the code passes a message to *IDialogContext.PostAsync(...)*.  The *PostAsync* method is how the Bot Framework sends messages back to the user.  Finally, *MessageReceived* calls *IDialogContext.Wait(MessageReceived)*.  This instructs the Bot Framework to wait indefinitely for response from the user and to invoke the MessageReceived handle when the message is received.  At this point, the state of the dialog is serialized until the user sends another message.

Let's F5 one more time and take a look at our Emulator.

![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%201%20-%20Setup/images/bot-emulator-response.png)

Awesome!  We can see the bot returned a message to us (via the *PostAsync* call).  Keep your breakpoints in place, and type another message into the Emulator.

Hmm...this time we went straight to the *MessageReceived* without calling *StartAsync*.  This is because *StartAsync* is only called the first time the dialog is invoked for a given conversation (remember, we previosly told Bot Framework to *Wait* for incoming messages and invoke *MessageReceived* when they arrive.
