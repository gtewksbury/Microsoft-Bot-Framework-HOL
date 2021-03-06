# Lab 1 - Bot Framework Setup

In this lab, we'll setup our Visual Studio .NET Bot Framework development environment.  This includes the following:

1.	Installing the Bot Emulator (used to test and debug our bot locally)
2.	Adding the Visual Studio 2017 Bot Framework project templates
3.	Creating and running our first *Hello World* bot project
4.	A quick review of the Bot Builder SDK for .NET

> Note, the following labs will require Visual Studio 2017 (preferably with the latest updates)


## Installing the Bot Emulator
In a production world, user's interface with your bots through one or more configured **channels**.  These channels represent the different *interfaces* that end-users can use to interact with your application.  One of the more power features of Bot Framework and Bot Services is the ability to code once for multiple **channels**.  The following **channels** are currently supported:

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

Unfortunately, none of these **channels** support a local debugging experience.  Luckily for us, Microsoft has created the Bot Emulator desktop client that can serve as a testing and debugging **channel** as you develop your bot application!


![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%201%20-%20Setup/images/bot-emulator.png)

Download and run the Bot Emulator installer from [here](https://github.com/Microsoft/BotFramework-Emulator/releases/download/v3.5.35/botframework-emulator-Setup-3.5.35.exe). 


## Installing Visual Studio Bot Framework Project Templates

Before you build your first .NET Bot Application, you'll want to install the Visual Studio Bot Framework Project Template.  This will give you a nice starting point with a working sample you can run immediately.

> Note, the following labs have been developed using Visual Studio 2017.  Additionally, the Bot Builder SDK for .NET requires .NET Framework 4.6 or higher.


To install the Visual Studio Bot Framework project template, download the [Bot Application](http://aka.ms/bf-bc-vstemplate) zip file and save it to your Visual Studio project templates directory (Don't unzip it).

> For typical Visual Studio installations, the *project templates* directory is located under ` %USERPROFILE%\Documents\Visual Studio 2017\Templates\ProjectTemplates\Visual C#\ `
 
Next, download the [Bot Controller](http://aka.ms/bf-bc-vscontrollertemplate) and [Bot Dialog](http://aka.ms/bf-bc-vsdialogtemplate) zip files and save them to your Visual Studio item templates directory (Again, no need to unzip these files).

> For typical Visual Studio installations, the *item templates* directory is located under ` %USERPROFILE%\Documents\Visual Studio 2017\Templates\ItemTemplates\Visual C#\ `

Once the templates have been added, go ahead and open Visual Studio 2017.

> If Visual Studio was previously open, you might need to close it and re-open it for Visual Studio to find your templates.

Navigate to *File* > *New Project*, select *Visual C#* in the *New Project* dialog, and select *Bot Application* (if you don't see this option, the templates might not have been installed correctly in the previous steps).  Since throughout the labs we'll be creating a restaurant reservation bot, I named mine *GoodEats*, but you can name yours whatever you want.


![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%201%20-%20Setup/images/vs2017-project.png)

## Basic Project Structure

At this point you should be all set to start developing your bot application.  Before we run it though, let's take a brief moment to examine the project structure.

> If you're impatient like me, you might try to immediately run the project.  Just make sure you're connected to the internet as the project template has a number of *nuget* packages that must be downloaded, including the *Bot Builder SDK*.


![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%201%20-%20Setup/images/vs2017-explorer.png)

If you've done any .NET web development, you'll probably notice Visual Studio created a new ASP.NET Web API project.  **Channels** invoke our bot application by Posting requests to our Web API, passing an *Activity* object in the body of the request.  Using open HTTP standards allows Bot Framework to be integrated with nearly any platform!  Let's open *MessagesController.cs* and take a look.

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

  We can also see that the controller seems to invoke a new *RootDialog*.  Let take a quick look at *RootDialog* to see what's going on there.

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

Immediately we see a few things that are probably new to us.  Our *RootDialog* class implements  something called *IDialog* and we have a couple methods that seem to be getting passed an *IDialogContext*.  These are some of the fundamental building blocks available within the .NET Bot Builder SDK (don't worry, we'll cover these in detail in future labs).  No better way to see how things work than to step through the code, so let's run the project and see it in action!

## Testing your Bot Application

Go ahead an run your Visual Studio project in *Debug* mode.  You should notice a new window open in your default browser.  Make note of the URL and port in your browser.

> If you receive any build errors, make sure you are connected to the internet and Visual Studio is able to download nuget packages

Let's open our Bot Emulator.  Copy the url from the browser into the Bot Emulator and append */api/messages* (the url should be http://localhost:3979/api/messages) and click *Connect*.

> You'll notice inputs for *Microsoft App ID* and *Microsoft App Password*.  You can leave those blank for now as they are not required for local debugging.


![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%201%20-%20Setup/images/bot-emulator-address.png)

On the Emulator's right pane, you'll notice some information was output to the *Log* section.  This section will provide you with the call stack and any potential exceptions that might arise within your code.  Go ahead an click on the *POST* link of one of the entries.  Above the *Logs*, you can see detailed information about the request (or exception if one occurred).

![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%201%20-%20Setup/images/bot-emulator-connected.png)


> Note, if you see error messages similar to below, you probably forgot to run your Visual Studio project, or entered the wrong address into the Emulator.


![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%201%20-%20Setup/images/bot-emulator-error.png)

Go back to the *RootDialog.cs* class in Visual Studio and put a breakpoint on the *StartAsync* method and *MessageReceivedAsync* method.

![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%201%20-%20Setup/images/bot-visual-studio-breakpoints.png)

Now let's go back to the Emulator and type a message to our bot and see what happens.


![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%201%20-%20Setup/images/bot-emulator-type-message.png)

You should notice that our *StartAsync* method breakpoint hit.  

> When you start a new **conversation**, Bot Framework will first call the *StartAsync* method of your *RootDialog* invoked via the *MessagesController*.  Notice in this example that the *StartAsync* method immediately calls *IDialogContext.Wait(MessageReceivedAsync)*.  This tells Bot Framework that the dialog should wait to receive a message from the user.  When it arrives, this message should in turn be passed to the *MessageReceivedAsync* handler. 

Let's hit F5 and see what happens next.  As you might have expected, our *MessageReceivedAsync* handler was invoked.

> You can also see that after computing the message size, the code passes a message to *IDialogContext.PostAsync(...)*.  The *PostAsync* method is how the Bot Framework sends messages back to the user.  Finally, *MessageReceivedAsync* calls *IDialogContext.Wait(MessageReceivedAsync)*.  This instructs the Bot Framework to wait indefinitely for a response from the user and to invoke the *MessageReceivedAsync* handler when a message arrives.  At this point, the state of the dialog is serialized until it receives another message.

Let's F5 one more time and take a look at our Emulator.

![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%201%20-%20Setup/images/bot-emulator-response.png)

Nice!  We can see the bot returned a message to us (thanks to the *PostAsync* call).  Keep your breakpoints in place, and type another message into the Emulator.

Hmm...this time we went straight to the *MessageReceivedAsync* without calling *StartAsync*.  
> This is because *StartAsync* is only called the first time the dialog is invoked for a given conversation (remember, we instructed Bot Framework to *Wait* for incoming messages and invoke *MessageReceivedAsync* when they arrive).  Apparent, we are still in the same conversation.

You can always *End Conversations*, essentially wiping out the state of a given **conversation**.  Upon doing so, the next message sent by the user will initiate a new **conversation**.  While you can do this programmatically, you can also do so directly through the Bot Emulator one of two ways as shown below:


![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%201%20-%20Setup/images/bot-emulator-end-conversation.png)

After ending the conversation, go ahead and type another message.  You should notice that the *StartAsync* was again called.  

> Because this was a new conversation, we didn't have an existing dialog waiting for a response, so Bot Framework *Started* a new dialog.

## Quick Recap

Congratulations, you now have a complete Visual Studio development environment capable of debugging custom bot applications!  Alright, I admit it, the sample bot we created isn't very exciting.  But hey, you're now setup and ready to create exciting new user experiences!  

Throughout the remainder of the labs, we'll be building out a bot that helps users make restaurant reservations.  Users will be able to ask our bot things like:

*	Make me a reservation at a good Indian restaurant in Pittsburgh
*	Can you book me a table tomorrow night at 7:30 for Mexican?

But wait a minute, we're simply sending text-based messages to our bot.  How can we possibly parse and interpret all the variations of how users might ask for a reservation?  That my friends is where Natural Language processing and Machine Learning comes in.

## Next Steps
In [Lab 2](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/tree/master/lab%202%20-%20LUIS) we'll build a machine learning model using Microsoft's Language Understanding Intelligence Service (known as LUIS) to give our bot some smarts.
