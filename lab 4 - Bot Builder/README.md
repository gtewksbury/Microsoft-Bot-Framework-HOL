# .NET Bot Builder
We're going to put Visual Studio on hold for a bit so we can focus on learning some of the core concepts and capabilities within the .NET Bot Builder SDK (don't worry, we'll get back to coding again in the next lab).

## Dialogs
Much like traditional web and mobile applications are made up of screens and UIs, bot applications are made up of **Dialogs**.  While at this point our reservation bot only has one **Dialog** (which we named *RootDialog*), bot applications can (and many times will) have numerous **Dialogs**, all encapsulating different aspects of a **conversation** (the same way traditional web and mobile applications have numerous screens to encapsulate different scenarios).

We can invoke a new **Dialog** a couple of different ways through *IDialogContext* depending on our needs.  Let's take a look at some examples:

> You'll begin to discover as you go through the labs that *IDialogContext* is the primary component you'll work with when orchestrating your **conversations**.

### IDialogContext.Call
One way to invoke a new **Dialog** is by calling *IDialogContext.Call*, passing in the **Dialog** to be invoked.  Here's an example:

```csharp

    [Serializable]
    public class ParentDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
	    // 1.  Parent Calls Child Dialog
            context.Call(new ChildDialog(), DoneAsync);

            return Task.CompletedTask;
        }

        private Task DoneAsync(IDialogContext context, IAwaitable<object> result)
        {
            return Task.CompletedTask;
        }
    }

    [Serializable]
    public class ChildDialog : IDialog<Object>
    {
		
	// 2. Called immediately upon invoking the dialog
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        public Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {
            return Task.CompletedTask;
        }
    }
```

> In the *ParentDialog's* *MessageReceivedAsync*, we are *Calling* a new **Dialog** named *ChildDialog*.  This will in turn invoke the *ChildDialog's* *StartAsync* method.  Additionally, we've passed in an optional *ResumeHandler* named *DoneAsync*.  When provided, this method will be invoked when the child **Dialog's** *Done(...)* method is called.

### IDialogContext.Forward
*IDialogContext* also provides a *Forward* method.  The difference is that *Forward* provides a *message* parameter which will be passed to the child **Dialog**.

```csharp

    [Serializable]
    public class ParentDialog : IDialog<object>
    {
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> result)
        {
            var message = await result;

	    // 1. Parent Forwards to Child Dialog
            await context.Forward(new ChildDialog(), DoneAsync, message);

        }

        private Task DoneAsync(IDialogContext context, IAwaitable<object> result)
        {
            return Task.CompletedTask;
        }
    }

    [Serializable]
    public class ChildDialog : IDialog<Object>
    {
		
	// 2. Called immediately upon invoking the dialog
        public Task StartAsync(IDialogContext context)
        {
            context.Wait(MessageReceivedAsync);

            return Task.CompletedTask;
        }

        public Task MessageReceivedAsync(IDialogContext context, IAwaitable<IMessageActivity> activity)
        {
	    // 3.  Called with the message provided to the Forward(...) method
            return Task.CompletedTask;
        }
    }
```

> Here we call *IDialogContext.Forward(...)* passing the received message to the child **Dialog**.  When calling *Forward*, the Bot Framework will first call the child **Dialog's** *StartAsync* method (*StartAsync* is always called when a **Dialog** is first invoked) and then pass the message to the child **Dialog's** *MessageReceivedAsync* handler)

At this point, it's probably important to understand how Bot Builder manages **Dialogs**.  The main thing to grasp is the concept of the **DialogStack**.

## Dialog Stack
Bot Builder maintains a *Stack* of invoked **Dialogs** throughout a **conversation**.  Each time a new **Dialog** is invoked (through either *IDialogContext.Call(...)* or *IDialogContext.Forward(...)*, the invoked **Dialog** is added to the top of the stack.

What's important to note is that top **Dialog** in the stack is in control of the **conversation**.  All messages sent to the **conversation** will be routed to the top **Dialog** until that **Dialog** either closes via *IDialogContext.Done(...)*, invokes another **Dialog**, or ends the conversation via **IDialogContext.EndConversation(...)*.

> If you think about it, traditional web and mobile UIs are similar, in that only the current screen will accept input from the user.

Equally important to note is that the current active **Dialog** in the stack must ALWAYS be configured to wait for incoming messages via *IDialogContext.Wait(...)*.

> If the current active **Dialog** completes execution without calling *IDialogContext.Wait(...)*, you'll receive an error similar to *'IDialog method execution finished with no resume handler specified through IDialogStack'*.  If you think about it, this makes sense since Bot Builder would have nowhere to route incoming messages.

## Root Dialogs
Just a quick note about *Root Dialogs*.  The name of this dialog is insignificant.  Conceptually, the *Root Dialog* is nothing more than the **Dialog** to be called at the start of a **conversation**.  This is typically the **Dialog** called from your application's *MessageController.Post(...)* method.  In the below example, *ParentDialog* is the *Root Dialog*.

```csharp

    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            if (activity.Type == ActivityTypes.Message)
            {
                await Conversation.SendAsync(activity, () => new ParentDialog());
            }
            else
            {
                //HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }
    }
```

## IDialogContext.Done
Once a **Dialog** has done all it was intended to do, you can call *IDialogContext.Done*, essentially passing control back to the previous **Dialog** in the stack.  When *Done* is called, it will execute the *ResumeHandler* provided by the parent **Dialog** if a *ResumeHandler* was provided.

> The *ResumeHandler* is optionally provided when the child **Dialog** is invoked via *IDialogContext.Call(...)* or *IDialogContext.Forward(...)*.  Be aware that if you call *Done* when a *ResumeHandler* was not provided, you'll likely receive errors, as the parent **Dialog** will have no means of registering a wait handler for the next incoming user message.  Typically, if the child **Dialog** calls *Done*, you should always register a *ResumeHandler*.

## Sending Messages to Users
There are a number of different ways and formats in which the Bot Framework can send messages to the user.  Again, this is typically handled through *IDialogContext*.

###IDialogContext.PostAsync(string)
We've actually used this method in our previous lab.  This is probably the simplest way to send a text-based message back to the user.

```csharp

        public async Task StartAsync(IDialogContext context)
        {

            await context.PostAsync("Here is some text that I want to send to the user");
            context.Wait(MessageReceivedAsync);
        }
```

###DialogContext.PostAsync(IMessageActivity)
In some cases, you may want to send the user rich visualizations a long with your message.  Bot Framework supports a number of visualizations, such as *ThumbnailCards*, *HeroCards*, and if you're feeling adventurous, *AdaptiveCards*.  These visualizations are added to messages as [Attachments](https://docs.microsoft.com/en-us/azure/bot-service/dotnet/bot-builder-dotnet-add-rich-card-attachments).  For such cases, you can call *IDialogContext.PostAsync(IMessageActivity)*.

```csharp

        public async Task StartAsync(IDialogContext context)
        {
            // create a hero card
            var card = new HeroCard
            {
                Title = "Dancing Crab Thai Noodle House (5 people)",
                Subtitle = "Friday, April 27, 2018 at 11:30:00 PM",
                Text = "2126 E.Carson St.Pittsburgh PA 15203",
                Images = new List<CardImage> { new CardImage(url: "https://eatstreet.com/images/image1.png") },
                Buttons = new List<CardAction> { new CardAction() { Title = "Reserve", Type = ActionTypes.ImBack, Value = "confirm" } }
            };

            // make a new message object and assign the hero card as an attachment
            var message = context.MakeMessage();
            message.Text = "OK, here’s what I have. Click ‘Reserve’ to confirm your reservation.";
            message.Attachments = new List<Attachment> { card.ToAttachment() };

            // send the message to the user
            await context.PostAsync(message);

            context.Wait(MessageReceivedAsync);
        }
```

And here's what it looks like in the Bot Emulator.

> You'll notice slight variations of how different **channels** render attachments.  You can visit the [Channel Inspector](https://docs.botframework.com/en-us/channel-inspector/channels/Skype/) to view examples of these variants.

![Create LUIS App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%204%20-%20Bot%20Builder/images/bot-eumulator-hero-card.png)

> Always be aware of the different **channels** with which your users might be interfacing.  For example, visualizations wouldn't make sense if your bot is integrated with speech-only **channels**.

###DialogContext.SayAsync
*IDialogContext.PostAsync(string)* works well when your sending messages to text-based **channels**, such as Web Chat, Slack, and Microsoft Teams, but what if your bot supports speech-based channels such as Cortana.  In these cases, you can send messages via *IDialogContext.SayAsync(...)*.  The text you provide can be decorated to control various characteristics of synthesized speech via [Speech Synthesis Markup Language](https://msdn.microsoft.com/en-us/library/hh378377(v=office.14).aspx).

```csharp

        public async Task StartAsync(IDialogContext context)
        {

            await context.SayAsync(text: "Here is some important information",
                                   speak: "Here is some <emphasis level=\"strong\"> important</emphasis> information");
            context.Wait(MessageReceivedAsync);
        }
```

## IDialogContext.Wait
Much of the life of a bot application is waiting for users to post information to them.  Within the Bot Framework, you instruct a **Dialog** to wait for a user response by calling *IDialogContext.Wait(...)*.  It should be noted that the Bot Framework will wait  indefinitely unless the conversation is either ended or reset (meaning, if I come back to the conversation 14 days later, the active **Dialog** in the stack will pick the conversation back up).  When *Wait* is called, the Bot Framework will serialize the state of the **DialogStack** as well as any custom state stored with the application to your configured state provider (this will be in-memory by default, but you can easily configure your bot to store state to Azure Cosmos DB or Table Storage as well, which is recommended for production workloads).  Note, at any given time, a **conversation** only supports ONE *Wait* handler.  If you multiple **Dialogs** configured to *Wait* for a message, you'll receive an exception when the **Dialog** is serialized.

## IDialogContext.EndConversation
At any point, you can end a conversation by calling *IDialogContext.EndConversation(...)*.  Upon doing so, the **DialogStack** and any **conversation** state will be destroyed.  The next message from the user will invoke the *Root Dialog*, kicking off a brand new conversation.

> *IDialogContext* also contains a *Reset* method.  Calling this method will reset the **DialogStack**, but retain any custom **conversation** state.  However, you will receive errors if you attempt to call this from within a **Dialog**.  These are useful when working with **Scorables**, which we'll briefly discuss in the next section.

## Global Message Handlers (Scorables)
There are times when you need to provide the user with  *global* commands regardless of the current **Dialog** (for example, if the user types *'cancel'* or *'nevermind'*, we might want to end the conversation regardless of the current **Dialog**).  That being said, we shouldn't have to handle this in every **Dialog**, and luckily we don't. 

**Scorables** provide a means of monitoring all incoming messages prior to the message being sent to the active **Dialog**, providing a means of intercepting the message and taking action.  

> We'll learn more about **Scorables** in a later lab.

##Quick Recap
Throughout this lab, we learned some basic Bot Framework concepts, including **Dialogs**,  **DialogStacks**, and **IDialogContext**.

## Next Steps
Hopefully you will find this information useful throughout the remainder of the labs and your future bot development.  Armed with this new-found knowledge, we're going to continue to build-out our reservation bot in [Lab 5](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/tree/luis-readme/lab%205%20-%20Dialogs), using many of these concepts.