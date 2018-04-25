# Lab 2 - Language Understanding Intelligence Service (LUIS)

If you're following the labs in order, at this point we have a pretty basic 'Hello World' bot that that simply listens to the incoming message from the user and returns the number of characters in the message (not very helpful).  Users typically interact with bots through natural language.  It's the bot's job to understand these natural language commands and act accordingly.  This is where LUIS comes in!

LUIS is a natural language processing machine-learning service in Microsoft's Cognitive Service's suite of APIs that can be easily trained to understand the content of natural language content.  Best of all, LUIS provides a FREE tier that we can start using immediately without any upfront cost!

To see where LUIS really shines, let's consider the bot that we will be building out during the remainder of the Labs.  Our bot will help users find restaurants and book reservations.  Think about all the different ways someone might request this if speaking with another person:

* *"I'd like to make a reservation for 6 people in Pittsburgh tomorrow night at 8:30 at a nice italian restaurant"*

* *"Reserve me a table next Thursday at 8:30 PM at an Italian restaurant near Pittsburgh area"*

* *Six of us are in the mood for a good Italian restaurant tonight at 8:30 here in Pittsburgh"

While we can easily interpret and parse the relevant information from there requests, doing so programmatically represents a significant challenge.  Luckily, in less than 30 minutes we can train LUIS to understand both the users' **Intent** to *Create a Reservation* as well as the relevant information (or **Entities** in LUIS-speak) included in the above statements:

1. Where:  **Pittsburgh**
2. When:   **tomorrow night at 8:30 pm**
3. How many people:  **6**
4. What type of food:  **Italian**

Don't believe me?  Let's try it for ourselves!

## Getting Started
Log into the [Luis Website](https://www.luis.ai) to begin building and training your model(s).  

![LUIS Homepage](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-homepage.png)

*Note, to sign into the site, you'll need a valid Microsoft Account.  If you don't already have one, you can create one by clicking the **Sign-Up** button on the home screen.  Additionally, if this is the first time you've logged into the site, you'll likely be taken to a welcome page and asked to fill in a couple of configuration settings.*

### Create a LUIS App
From the following screen, click the **Create New App** button

![Create LUIS App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-blank-app.png)

Give your app a *Name* and *Description* (I named mine *GoodEats*, but you can name yours whatever you want).

![Create LUIS App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-create-new-app.png)

Your app is now ready to be built and trained!

*Notice the newly-created app already contains a default **None** intent.  A single LUIS app can (and likely will) have multiple intents to signify the different requets a user can make (for example, **Create Reservation** vs **Cancel Reservation** are two different intents).  It is the job of LUIS to predict and interpret a user's intent.  LUIS will select **None** when no other intents match the user's request (for example, if someone asked our reservation app 'What's the weather like in Chicago')*

![Create LUIS App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-new-app.png)

### Add Create Reservation Intent
The next step is to add the **Intents** that our app will be trained to interpret.  For the purposes of this lab, we will be creating a single *Create Reservation* **Intent**.

Click the *Create New Intent* button and set the **intent** name to *Create Reservation*


![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-create-intent.png)

The following screen is where we train our **intent** to understand different variations of how users might ask for a reservation.  We'll come back to this in a moment.

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-new-intent.png)


### Add Entities

Before we start training our *Create Reservation* **Intent**, let's define the data (or **Entities**) associated with our app.  **Entities** represent data that LUIS will be trained to identify within a user's request.  On the left menu, click *Entities*.  You should be taken to the following screen:

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-blank-entities.png)

Notice you have a few options.  You can *Create new entity* from scratch, or you can use *prebuilt entities*.  For this lab, we are going to include a number of prebuilt entities.  Click the *Manage prebuilt entities* button and add the following:

* **number**
* **datetimeV2**

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-add-prebuilt-entities.png)

Next, click the *Add prebuilt domain entities* and add the following:

* **RestaurantReservation.Address**
* **RestaurantReservation.Cuisine**
* **RestaurantReservation.PlaceName**

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-add-domain-entities.png)

We've now identified the different information that LUIS should attempt to identify within a given request.  At this point, you're screen should look similar to the following:


![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-entities-added.png)

### Add Utterances

We're now ready to start training our app!  Let's navigate back to our *Create Reservation* **intent**.  We are going to train our intent by giving it different examples of the ways people might ask for a reservation.  These examples are what LUIS refers to as **Utterances**.  Note that the more **utterances** we provide for training, the more accurate LUIS will become!

Enter the following **utterance** into your *Create Reservation* **intent** and click *Enter*

```

Make me a reservation in Pitsburgh tomorrow night at 7:30 at a thai restaurant

```

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-utterance-1.png)

In a moment, you should see your text added to the **utterance** list.  Notice that it has identified *tomorrow night at 7:30* as our *datetimeV2* entity.  I'm always impressed by the number of variations in which LUIS can interpret dates and times!

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-utterance-1-added.png)


We need to give LUIS a little help to understand the other pertinent information in our **utterance**.  Hover your mouse over *Pittsburgh*.  Notice it places brackets around the text.  


![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-highlight-city-entity.png)

Click the highlighted text and select *RestaurantReservation.Address* from the menu.  We've just told LUIS to interpret this as the restaurant's address.  Note, in training LUIS this way, we aren't just training LUIS to recognize the term *Pittsburgh*, but rather are training LUIS to understand how to recognize *RestaurantReservation.Addresss* based on natural language heuristics!

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-map-city-entity.png)

Now let's start to train the model to recognize the requested cuisine.  Click *thai* and select *RestaurantReservation.Cuisine*.

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-map-cuisine.png)

Let's add another **utterance** and see what LUIS has learned!  Type the following statement into the *utterance* textbox and click enter:

```

I'd like to reserve a table for 6 people next Thursday at 8 pm in new orleans

```

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-utterance-2-added.png)

Hmm...although LUIS was able to identify the number of people and date, it seems that it is yet to master the art interpreting restaurant location.  In the LUIS world, that can only make one thing...more training!  The difference here is that our location is actually two words *New* and *Orleans*.  To select this for training, simply click both *New* and *Orleans*.  You should notice the brackets now enclose both.  Click *New Orleans* and select *RestaurantReservation.Address*

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-map-utterance-2.png)

As we said before, you model is only as good as your training.  Add and train 5 or 6 more examples of reservation **utterances** providing different cuisine types and locations.  Make sure to vary the **utterances** based on the different ways people might make this request.  Here are a few more examples to get you started:

```
I'd like to make a reservation in Miami FL for two of us
Can you make me a reservation next Thursday morning at 8 at a good Mexican restaurant in Los Angeles for 2 people
3 of us would like a eat chinese food tomorrow at 2pm in Washington PA
I need a reservation for 6 people in Chicago at an italian restaurant

```

### Re-associated Utterances

If you navigate back to your **Intents** list,  you should see the count of **utternances** you added to your *Create Reservation* **intent**.  Oddly enough though, you'll notice that we also have a number of **utterances** in our *None* intent.  That seems strange since we didn't add any **utterances** to *None*.  Let's click on the *None* intent and see what's there.

LUIS was kind enough to include these **utterances** when we added the *RestaurantReservation.Address*, *RestaurantReservation.PlaceName*, and *RestaurantReservation.Cuisne* *prebuilt domain entities* to our app.  However, in reviewing these, many of them should be associated with our *Create Reservation* intent.

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-none-utterances.png)

Luckily, it's easy to re-associate an **utterance** to a different **intent** when LUIS makes a mistake!  Review each **utterance** that seems to be associated with restaurant reservations and select *Create Reservation* in it's dropdown.  NOTE, leave any **utterances** NOT related to making a reservation with the *None* **intent**.

It's important to note that at any point in time, you can modify the associated intent of an **utterance**.  In fact, once you've published your model (we'll discuss *publishing* in the following section), you view see (and modify) LUIS's categorization of individual **utterances** from your users by navigating to the *Review endpoint utterances* link in the left menu!

### Training and Testing

Let's make it official and train our app based on all the information we've given it!  You've probably noticed a *Train* button in the top right (next to the *Test* button).  Go ahead an click it!

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-train.png)

After a few moments, you're model has now been trained with your latest **utterances**.  You can retrain at any point in time.  Let's go ahead and test your model!  Click the *Test* button and enter a new reservation request.

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-test.png)

Hopefully it correct identified your intents!  It's important to note predictions are scored on a scale between 0 and 1.  Look at the score for you're request.  Depending on the results, you might want...Wait for it...MORE TRAINING!

### Publishing

Now that we've trained our model, it's time to publish our changes so that we can consume our model externally.  LUIS exposes published models via a publicly accessible REST API (protected by a secret subscription key that only you and your team should know).  To make our most recent training publicly available, navigate to the *Publish* menu on the top of your screen.

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-publish.png)

*You'll notice you have the option to publish to either Production and Staging endpoints.  This allows you to retrain your model and publish to a Staging environment to verify you changes before pushing to Production.*

Make sure *Production* is selected and click the *Publish to production slot* button.  In a few moments, your newly trained model can be called externally.  In fact, you can try it yourself!  

```

Note, if you re-train your model you'll have to re-publish for those changes to impact the LUIS API.

```


#### Calling LUIS Externally

Notice the URL associated with your Starter_Key.  Mine looks something like this:

*https://westus.api.cognitive.microsoft.com/luis/v2.0/apps/ ``a2b4583a-539b-4fa8-8062-c3f0648b5400`` ?subscription-key= ``<subscription key>`` &verbose=true&timezoneOffset=0&q=*

The first highlighted section is your LUIS Model Id, which identifies the LUIS application we just created.  The second highlighted section (obfuscated from prying eyes), is my Starter_Key.  Without this key, you'll receive a 401-Unauthorized response when calling the REST API.

Copy the url into your favorite REST client and an example your utterance to the *q=* querystring parameter (I prefer Postman, but feel free to use whatever makes you happy).

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-rest-api.png)

You'll see that the response not only identified the *Create Reservation* **intent** but also provides me a list of **entity** values it parsed from the request.  Notice also that the *tomorrow night at 7* is further provided as a date/time value of *2018-04-26 19:00:00*.

#### Keys

Make not of the *Keys* button back on the *Publish* screen.  For now, we've been working off the **Free** started key provided by LUIS.  This gives us 10,000 requests per month!  However, for production workloads, you can associate a Standard Tier Cognitive Service's subscription key created through your Azure subscription for higher throughput and performance.

## Quick Recap

Here's what we accomplished:

1.	We created a new LUIS app through the [LUIS website](https://www.luis.ai)
2.	We added **entities** that define the data we want to extract from a user's **uttereance**
3.	We trained our app to recognize the appropriate intent data **entities** within sample **utterances**
4.	We published our LUIS app so it can be consumed externally via REST APIs

Remember, the more training you provide your model, the more accurate **intent** predictions and **entity** identification will become.  A strong model will have hundreds of trained and validated **utterances**.

## Next Steps

Now that we've trained and published our LUIS app, it's time to integrate it with our chat bot!