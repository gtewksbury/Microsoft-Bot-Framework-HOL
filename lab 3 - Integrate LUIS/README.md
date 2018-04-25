# Lab 3 - LuisDialog and State Management
Congratulations on making it this far!  At this point you should have setup your .NET Bot Framework development environment ([Lab 1](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/tree/luis-readme/lab%201%20-%20Setup)) and have created, trained, and published your LUIS application ([Lab 2](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/tree/luis-readme/lab%202%20-%20LUIS)).

> Make sure that didn't forget to **publish** you LUIS app before preceding to this lab, or things won't go quite as expected.


In this lab, we are going to integrate the sample bot we created in [Lab 1](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/tree/luis-readme/lab%201%20-%20Setup) with the LUIS model we trained in [Lab 2](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/tree/luis-readme/lab%202%20-%20LUIS).  Moving forward, we're going to be spending most of our time in Visual Studio, so hopefully you're ready to get your hands dirty with some coding!

I hate to do this to you, but I'm going to ask that you start from the *start* project included in this lab (as opposed to the project we created in Lab 1).  I promise, they are almost identical with a couple of minor exceptions:

* The *starter* project's Bot Builder SDK has been upgraded to *3.15.0* to support some of the newer features we'll need (nothing more than a nuget update)
* It contains a couple of extension methods to help parse dates and integer values from *LuisResults*
* It contains a *Reservation.cs* class file which contains some properties which define a reservation (We won't be using it much in here, but we will in future labs).

## LuisDialog
Alright, let's open the *starter* Visual Studio project in this lab and open the *RootDialog.cs* file.  It should look something like this:

```csharp

    [Serializable]
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
}
```

Go ahead and remove the *StartAsync* and *MessageReceivedAsync* methods.

```csharp

    [Serializable]
    public class RootDialog : IDialog<object>
    {

    }

```

Next, we no longer need to inherit from *IDialog*.  Instead, we're going to inherit from .NET Bot Builder's *LuisDialog* with a *Reservation* type parameter.  You're *RootDialog* should now look like this:

```csharp

    [Serializable]
    public class RootDialog : LuisDialog<Reservation>
    {

    }
```

> Because LUIS is such an integral part of bot development, the Bot Builder SDK was friendly enough to create this base dialog which integrates directly with your LUIS app!  If you're curious about how the LuisDialog works, feel free to take a look on [GitHub](https://github.com/Microsoft/BotBuilder/blob/master/CSharp/Library/Microsoft.Bot.Builder/Dialogs/LuisDialog.cs).  To summarize, it handles the *StartAsync* and implements it's own *MessageReceived* handler.  The *MessageReceived* handler in turn passes the incoming user message to the LUIS service endpoint you published in [Lab 2](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/tree/luis-readme/lab%202%20-%20LUIS) and passes the result to methods that you configure to handle specific **intents**.  We'll take a look at how to wireup the dialog to our LUIS app and **Iintents** now!

First, *LuisDialog* needs to know how to reach our published LUIS app.  Luckily, this can be done by simply decorating our class with the *LuisModelAttribute*, giving it the Model ID and Subscription Key for our LUIS endpoint.  

```csharp

    [Serializable]
    [LuisModel("<Your Luis Model Id>", "<Your LUIS Subscription Key>")]
    public class RootDialog : LuisDialog<Reservation>
    {

    }

```


> If you don't remember these values, you can retrieve them from the *publish* page for your app in https://www.luis.ai.  


![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%203%20-%20Integrate%20LUIS/images/luis-publish.png)

Mine looks something like this:

*westus.api.cognitive.microsoft.com/luis/v2.0/apps/ ``a2b4583a-539b-4fa8-8062-c3f0648b5400`` ?subscription-key= ``<subscription key>`` &verbose=true&timezoneOffset=0&q=*

> The first highlighted section is your LUIS Model Id, which uniquely identifies the LUIS application you just created.  The second highlighted section (obfuscated from prying eyes), is my Starter_Key.  Without this key, you'll receive a 401-Unauthorized response when calling the REST API.


Now we have to tell the *LuisDialog* which methods to call when it predicts specific **intents**.  This done by decorating methods with the *LuisIntentAttribute*  Go ahead and add the following 2 methods to *RootDialog*:


```csharp

    public class RootDialog : LuisDialog<Reservation>
    {

        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
			// wait for the next user request
            context.Wait(MessageReceived);
        }

        [LuisIntent("Create Reservation")]
        public async Task CreateReservation(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {


            // wait for the next user request
            context.Wait(MessageReceived);
        }
    }

```

> You can see that the method accept not only the *IDialogContext* and *IMessageActivity* that were passed to LUIS's *MessageReceived* method, but also the *LuisResult* which includes the recommended intent as well as any **entities** parsed from the request.  Additionally, you'll notice that we decorated the *CreateReservation* method with the name of our *'Create Reservation'* intent.  We also decorated the *None* method with the *'None'* **intent** as well as ''.  '' tells LuisDialog to call this method when it predicted an intent, but no other methods in are decorated to handle the intent.

At this point, let's run our bot and see how smart it is.  Go ahead and place breakpoints in the *None* and *CreateReservation* methods and run Visual Studio in *Debug* mode.

![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%203%20-%20Integrate%20LUIS/images/vis2017-breakpoints.png)

Fire up the bot emulator and type in a message similar to the following:

``
Make me a reservation in Pittsburgh tomorrow at 7:30 pm
``

Hopefully the *CreateReservation* method hit!  Take a moment to inspect the *LuisResult* that was passed to the method.  You should hopefully see the *RestaurantReservation.Address* and the *builtin.datetimeV2.datetime* entity.

> If you don't get the expected results, it might mean that you needs to be further trained and re-published, or your current model was never published.

Assuming the last test was successful, let's type another message into the Bot Emulator.  Something that has nothing to do with restaurant reservations:

``
What's the weather like in Pittsburgh
``

What happened?  Hopefully your *None* method was hit!

## State Management
As your bot becomes more complicated, you'll likely need to save state as you move from **Dialog** to **Dialog** throughout a **conversation**.  In this example, we're going to retrieve the **entities** within the *LuisResult* and store them in state.

Let's start by retrieving the LUIS **entities** from the request.  Back in your *CreateReservation* method, let's add the following code:

```csharp
        [LuisIntent("Create Reservation")]
        public async Task CreateReservation(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {

            if (result.TryFindEntity("RestaurantReservation.Address", out var locationRecommendation))
            {
                context.PrivateConversationData.SetValue("LOCATION", locationRecommendation.Entity);
            }

            // wait for the next user request
            context.Wait(MessageReceived);
        }
```

*LuisResult* contains a *TryFindEntity* extension method which will attempt to retrieve a value based on the given **entity** name (if you remember from the last lab, we associated a *'RestaurantReservation.Address'* entity with our LUIS application).  This will set the passed *EntityRecommenation* instance if the **entity** was found in the request.

In the example above, if we find a *RestaurantReservation.Address* entity, we add the value to our *PrivateConverationState* through *IDialogContext*.  While all are simple key / value pairs, there are 3 types of state that you can store:

* User State - State tied to the user across all conversations
* Conversation State - State tied to a conversation (which could include group conversations)
* Private Conversation State - State tied to the current user within a given conversation

For the sake of these tutorials, we are using the built-in memory state provider, however, for production bots, you would want to deploy state to a persistent store to accomodate for auto-scaling and potential hardware failures.  To learn more about storing state in Cosmos DB or Azure Table Storage, please refer to the [Manage Stating](https://docs.microsoft.com/en-us/azure/bot-service/dotnet/bot-builder-dotnet-state-azure-cosmosdb) documentation provided by Microsoft (it's easy to setup, it just requires a little dependency injection).


> While *string* values are simple to parse through the *LuisResult.TryFindEntity(...)* method, grabbing the converted values for Dates and Integers requires a little more work.  Therefore, I've added a custom *LuisExtensions* class to help make this a bit easier.

Let's continue to build out our *CreateReservation* method.  Here's the final result:

```csharp

        [LuisIntent("Create Reservation")]
        public async Task CreateReservation(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            await context.PostAsync("Looks like your attempting to create a reservation.  Let's see what information we were able to pull");

            // attempt to parse the location from the request if provided by the user
            if (result.TryFindEntity("RestaurantReservation.Address", out var locationRecommendation))
            {
                context.PrivateConversationData.SetValue("LOCATION", locationRecommendation.Entity);
            }

            // attempt to parse a cuisine from the luis result if provided by the user
            if (result.TryFindEntity("RestaurantReservation.Cuisine", out var cuisineRecommendation))
            {
                context.PrivateConversationData.SetValue("CUISINE", cuisineRecommendation.Entity);
            }

            // attempt to parse the date time if provided by the user
            if (result.TryFindDateTime("builtin.datetimeV2.datetime", out var when))
            {
                context.PrivateConversationData.SetValue("WHEN", when.Value.ToString());
            }

            // attempt to parse the number of people if provided by the user
            if (result.TryFindInteger("builtin.number", out var partySize))
            {
                context.PrivateConversationData.SetValue("PARTY_SIZE", partySize.Value);
            }
            


			
            // reply with the parsed location if we saved in state
            if (context.PrivateConversationData.ContainsKey("LOCATION"))
            {
                await context.PostAsync($"Location Preference:  {context.PrivateConversationData.GetValueOrDefault<string>("LOCATION")}");
            }

            // reply with the parsed cuisine if we saved in state
            if (context.PrivateConversationData.ContainsKey("CUISINE"))
            {
                await context.PostAsync($"Cuisine Preference:  {context.PrivateConversationData.GetValueOrDefault<string>("CUISINE")}");
            }

            // reply with the parsed date / time if we saved in state
            if (context.PrivateConversationData.ContainsKey("WHEN"))
            {
                await context.PostAsync($"Date Preference:  {context.PrivateConversationData.GetValueOrDefault<DateTime>("WHEN")}");
            }

            // reply with the parsed number of people if saved in state
            if (context.PrivateConversationData.ContainsKey("PARTY_SIZE"))
            {
                await context.PostAsync($"Party Size Preference:  {context.PrivateConversationData.GetValueOrDefault<int>("PARTY_SIZE")}");
            }

            // wait for the next user request
            context.Wait(MessageReceived);
        }

```

Here we are simply parsing and storing the remaining **entities** (if provided), reply with the **entity** values we successfully parsed, and wait for the next message.  Let's also make the *None* method a little more informative:

```csharp
        [LuisIntent("")]
        [LuisIntent("None")]
        public async Task None(IDialogContext context, IAwaitable<IMessageActivity> activity, LuisResult result)
        {
            // send a message to the user indicating the request was not recognized
            await context.PostAsync("Sorry, I don't understand.  I only know how to make restaurant reservations.");

            // wait for the next user request
            context.Wait(MessageReceived);
        }
```

Here we just post back to the user that we don't understand the request and wait for the next message.

Let's run the application again and type the following into the Bot Emulator (note, at this point you can remove your breakpoints):

``
Make me a reservation in Pittsburgh tomorrow at 12:30 pm
``

You should see a response similar to the following in your emulator.

![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%203%20-%20Integrate%20LUIS/images/bot-emulator-intial-state.png)

Let's send another message.  This time only send the locatoin in the request:

``
Make me a reservation in Cleveland
``


![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%203%20-%20Integrate%20LUIS/images/bot-emulator-second-state.png)

Wait a second, I only provided the city this time.  Why did it also return the date from the previous request.  If you remember, we replied with any values that we stored in state.  The state persists throughout the conversation.  If you left and came back to your bot 8 hours from now, the state from the previous response would still be stored.  I wonder what happens to the state if you end the conversation.  Let's go back into Visual Studio in the *CreateReservation* method and make a small change.  

Let's replace this:

```csharp

context.Wait(MessageReceived);

```

With this:

```csharp
context.EndConversation(EndOfConversationCodes.CompletedSuccessfully);
```

Go ahead an run Visual Studio and the Bot Eumulator.  Within the emulator, type the following message:

``
Make me a reservation in Pittsburgh tomorrow at 12:30 pm
``

The bot should respond with the entered city and date.  Now go ahead and type the following:

``
Make me a reservation in Pittsburgh
``

This time we notice that the date is not returned.  This is because your message didn't provide a date and the previous state was wiped clean when you ended the conversation.  In fact, when you send a message after ending the conversation, an entirely new **Dialog Stack** was created.  