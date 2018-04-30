# Lab 2 - Language Understanding Intelligence Service (LUIS)

If you're following the labs in order, at this point we have a pretty basic 'Hello World' bot that simply listens to incoming message from the user and returns the number of characters in the message (not very useful).  More sophisticated bots interact with users through *conversational interfaces*.  It's the bot's job to interpret the **intent** of a user's request and respond accordingly.  This is where LUIS comes in!

LUIS is a natural language processing machine-learning service in Microsoft's Cognitive Services suite of APIs.  Best of all, LUIS provides a free tier we can start using immediately!

To see where LUIS really shines, let's consider the bot that we will be building out during the remainder of the Labs.  Our bot will help users find restaurants and book reservations.  Think about all the different ways someone might request to make a reservation:

* *"I'd like to make a reservation for 6 people in Pittsburgh tomorrow night at 8:30 at a nice italian restaurant"*

* *"Reserve me a table next Thursday at 8:30 PM at an Italian restaurant near Pittsburgh area"*

* *Six of us are in the mood for a good Italian restaurant tonight at 8:30 here in Pittsburgh"

While we as humans can easily interpret and parse the relevant information from such requests, doing so programmatically represents a significant challenge.  Luckily, in less than 30 minutes we can train LUIS to understand not only a user's' **Intent** to *Create a Reservation* but also the relevant information (or **Entities** in LUIS-speak) included in the request.  For example:

1. Where:  **Pittsburgh**
2. When:   **tomorrow night at 8:30 pm**
3. How many people:  **6**
4. What type of food:  **Italian**

Don't believe me?  Let's try it for ourselves!

## Getting Started
Log into the [Luis Website](https://www.luis.ai) to begin building and training your model(s).  

![LUIS Homepage](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-homepage.png)

> To sign into the site, you'll need a valid Microsoft Account.  Preferably, the Microsoft account you select would be associated with an Azure subscription as well, so you can optionally associate your LUIS app with a Standard pricing tier (you *might* start received 403 throttling errors in [Lab 6](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/tree/luis-readme/lab%206%20-%20Luis%20all%20the%20way%20down) on the free tier), but this is NOT required for these labs.  If you don't already have one, you can create one by clicking the **Sign-Up** button on the home screen.  Additionally, if this is the first time you've logged into the site, you'll likely be taken to a welcome page and asked to fill in a couple of configuration settings.

### Create a LUIS App
From the following screen, click the **Create New App** button

![Create LUIS App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-blank-app.png)

Give your app a *Name* and *Description* (I named mine *GoodEats*, but you can name yours whatever you want).

![Create LUIS App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-create-new-app.png)

Your app is now ready to be built and trained!

> Notice the newly-created app already contains a default **None** intent.  A single LUIS app can (and likely will) have multiple intents to signify the different requets a user can make (for example, **Create Reservation** vs **Cancel Reservation**).  It is the job of LUIS to predict and interpret a user's **intent**.  LUIS will select **None** when no other intents match the user's request (for example, if someone asked our reservation app 'What's the weather like in Chicago')

![Create LUIS App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-new-app.png)

### Add Create Reservation Intent
The next step is to add the **Intents** that our app will be trained to interpret.  For the purposes of this lab, we will be creating a single *Create Reservation* **Intent**.

Click the *Create New Intent* button and set the **intent** name to *Create Reservation*


![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-create-intent.png)

The following screen is where we train our **intent** to understand different variations of how users might ask for a reservation.  We'll come back to this in a moment.

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-new-intent.png)


### Add Entities

Before we start training our *Create Reservation* **Intent**, let's define the data (or **Entities**) associated with our app.  **Entities** represent data that LUIS will be trained to identify within a user's request.  On the left menu, click *Entities*.  You should be taken to the following screen:

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-blank-entities.png)

Notice you have a few options.  You can *Create new entity* from scratch, or you can use *prebuilt entities*.  For this lab, we are going to include a number of prebuilt entities.  Click the *Manage prebuilt entities* button and add the following:

* **number**
* **datetimeV2**

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-add-prebuilt-entities.png)

Next, click the *Add prebuilt domain entities* and add the following:

* **RestaurantReservation.Address**
* **RestaurantReservation.Cuisine**

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-add-domain-entities.png)

We've now identified the different information that LUIS should attempt to identify within a given request.  At this point, you're screen should look similar to the following:


![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-entities-added.png)

### Add Utterances

We're now ready to start training our app!  Let's navigate back to our *Create Reservation* **intent**.  We are going to train our **intent** by giving it different examples of the ways people might ask for a reservation.  These examples are what LUIS refers to as **Utterances**.  Note that the more **utterances** we provide for training, the more accurate LUIS will become!

Enter the following **utterance** into your *Create Reservation* **intent** and click *Enter*


> Make me a reservation in Pitsburgh tomorrow night at 7:30 at a thai restaurant


![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-utterance-1.png)

In a moment, you should see your text added to the **utterance** list.  Notice that it has identified *tomorrow night at 7:30* as our *datetimeV2* entity.  I'm always impressed by the number of variations in which LUIS can interpret dates and times!

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-utterance-1-added.png)


We need to give LUIS a little help to understand the other pertinent information in our **utterance**.  Hover your mouse over *Pittsburgh*.  Notice it places brackets around the text.  


![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-highlight-city-entity.png)

Click the highlighted text and select *RestaurantReservation.Address* from the menu.  We've just told LUIS to interpret this as the restaurant's address.  Note, in training LUIS this way, we aren't just training LUIS to recognize the term *Pittsburgh*, but rather we're training LUIS to understand how to recognize *RestaurantReservation.Addresss* based on natural language heuristics!

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-map-city-entity.png)

Now let's start to train the model to recognize the requested cuisine.  Click *thai* and select *RestaurantReservation.Cuisine*.

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-map-cuisine.png)

Let's add another **utterance**.  Type the following statement into the *utterance* textbox and click *Enter*:


> I'd like to reserve a table for 6 people next Thursday at 8 pm in new orleans


![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-utterance-2-added.png)

Interesting...LUIS was able to identify the number of people and date.  Note in this case our location is actually two words *New* and *Orleans* separately.  To select this for training, simply click both *New* and *Orleans*.  You should notice the brackets now enclose both.  Click *New Orleans* and select *RestaurantReservation.Address*

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-map-utterance-2.png)

As we said before, you model is only as good as your training.  Add some more reservation **utterances**, providing different cuisine types, locations, etc.  Make sure to vary the **utterances** based on the different ways people might make this request.  Here are some examples to get you started (make sure to map the **entities** in each):

> Make me a reservation in Pittsburgh

> I'd like to make a reservation

> I'd like to reserve a table in Cleveland

> I'd like to make a reservation in Miami FL for two of us

> Can you make me a reservation next Thursday morning at 8 at a good Mexican restaurant in Los Angeles for 2 people

> 3 of us would like a eat chinese food tomorrow at 2pm in Washington PA

> I need a reservation for 6 people in Chicago at an italian restaurant

> make me a reservation at a good middle eastern restaurant in cleveland

> Make me a reservation in Boston

> I'd like a table tomorrow night at 8:30 pm

> Can you make me a reservation in Nashville

> Reserve me a table at a local Mediterranean restaurant

> Can you make me a reservation at a pizza restaurant

> Make me a reservation

Remember as you go through the labs that the more training you provide, the more accurate LUIS will become.  This is the power of machine learning!  We can make our app smarter through training without having to *code* around individual scenarios!

### Re-associated Utterances

Navigate back to your **Intents** list,  and select the *None* **intent**.  You should notice a number of **utterances** have already been added.  LUIS was kind enough to include these **utterances** when we added the *RestaurantReservation.Address* and *RestaurantReservation.Cuisne* *prebuilt domain entities* to our app.  However, in reviewing these, many of them should be associated with our *Create Reservation* **intent**.

Luckily, it's easy to re-associate an **utterance** to a different **intent** when LUIS makes a mistake!  Review each **utterance** that seems to be associated with restaurant reservations and select *Create Reservation* in it's dropdown.  NOTE, leave any **utterances** NOT related to making a reservation with the *None* **intent**.  

![Retrain Utterances](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-none-utterances.png)

Also, it's a good idea to train your *None* **intent** as well.  Within the *None* **intent**, add a few **utterances** that have nothing to do with making a restaurant reservation.  Here are some examples:

> Book me a flight from Pittsburgh to Cleveland

> How will the Pittsburgh Steelers by next year

> How tall is the Eiffel Tower in Paris

> What time is it in Chicago

It's important to note that at any point in time, you can modify the associated intent of an **utterance**.  In fact, once you've published your model (we'll discuss *publishing* in the following section), you can view (and modify) LUIS's categorization of individual user **utterances** by navigating to the *Review endpoint utterances* link in the left menu!

### Training and Testing

At this point your *Create Reservation* **intent** should have around 20-30 **utterances** (give or take).  Let's make it official and *train* our app based on all the information we've given it so far!  You've probably noticed a *Train* button in the top right (next to the *Test* button).  Go ahead an click it!

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-train.png)

After a few moments, you're model has now been trained with your latest **utterances** (it's important to note that you can retrain at any point in time).  Let's go back to our *Create Reservation* intent and see how smart LUIS has become.  Go ahead and add a new **utterance**, for example `Can you make me a reservation in Boston at a good seafood restaurant for eight people tomorrow at 5:30 pm`.

What happened?  For me, LUIS was able to identify all the pertinent information in the request with a very high confidence score!  Nice work LUIS, pat on the back to ya!

> Predictions are scored on a scale between 0 and 1 (1 being the highest).  Look at the score for you're request.  Depending on the results, your app may need more training.

### Publishing

Now that we've trained our model, it's time to publish them!  Once published, LUIS exposes your app publicly via a REST API (protected by a secret subscription key that only you and your team should know).  To make our most recent training publicly available to external consumers, navigate to the *Publish* menu on the top of your screen.

> In future labs, our bot will connect to this API to parse incoming user messages.

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-publish.png)

> You'll notice you have the option to publish to either Production and Staging endpoints.  This allows you to retrain and test within a Staging environment before publishing to Production.  **Important**: Every time you re-train your app, you'll have to re-publish before those changes impact your external-facing API.

Make sure *Production* is selected and click the *Publish to production slot* button.  In a few moments, your newly trained model can be called externally.  In fact, you can try it yourself!  

#### Calling LUIS Externally

Notice the URL associated with your Starter_Key at the bottom of your screen (mine looks something like this):

*westus.api.cognitive.microsoft.com/luis/v2.0/apps/ ``a2b4583a-539b-4fa8-8062-c3f0648b5400`` ?subscription-key= ``<subscription key>`` &verbose=true&timezoneOffset=0&q=*

> The first highlighted section is your LUIS Model Id, which uniquely identifies the LUIS application you just created.  The second highlighted section (obfuscated from prying eyes), is my Starter_Key.  Without this key, you'll receive a 401-Unauthorized response when calling the REST API.

Copy the url into your favorite REST client and add a sample **utterance** to the *q=* querystring parameter (I prefer Postman, but feel free to use whatever makes you happy).

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%202%20-%20LUIS/images/luis-rest-api.png)

You'll see that the response not only identified the *Create Reservation* **intent** but also provides a list of **entity** values it parsed from the request.  Notice also that *'tomorrow night at 7'* is further converted to a date/time format *(2018-04-26 19:00:00)*.

#### Keys

Make note of the *Keys* button back on the *Publish* screen.  For now, we're working off the **Free** Starter_Key provided by LUIS.  This gives us 10,000 requests per month!  However, for production workloads, you can associate a Standard Tier Cognitive Service's subscription key created through your Azure subscription for higher throughput and performance.

## Quick Recap

Here's what we accomplished:

1.	We created a new LUIS app through the [LUIS website](https://www.luis.ai)
2.	We added **entities** that define the data we want to extract from a user's **utterance**
3.	We trained our app to recognize the appropriate **intent** and **entities** within sample **utterances**
4.	We published our LUIS app so it can be consumed externally via highly-available REST APIs

Remember, the more training you provide, the more accurate **intent** and **entity** predictions will become.  A strong model will have hundreds of trained and validated **utterances**.

## Next Steps

Now that we've trained and published our LUIS app, it's time to integrate it with our chat bot in [Lab 3](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/tree/master/lab%203%20-%20Integrate%20LUIS)!