# Lab 2 - Language Understanding Intelligence Service (LUIS)

If you're following the labs in order, at this point we have a pretty basic 'Hello World' bot that that simply listens to the incoming message from the user and returns the number of characters in the message (not very helpful).  Users typically interact with bots through natural language.  It's the bot's job to understand these natural language commands and act accordingly.  This is where LUIS comes in!

LUIS is a natural language processing machine-learning service in Microsoft's Cognitive Service's suite of APIs that can be easily trained to understand the content of natural language content.  Best of all, LUIS provides a FREE tier that we can start using immediately without any upfront cost!

To see where LUIS really shines, let's consider the bot that we will be building out during the remainder of the Labs.  Our bot will help users find restaurants and book reservations.  Think about all the different ways someone might request this if speaking with another person:

* *"I'd like to make a reservation for 6 people in Pittsburgh tomorrow night at 8:30 at a nice italian restaurant"*

* *"Reserve me a table next Thursday at 8:30 PM at an Italian restaurant near Pittsburgh area"*

* *Six of us are in the mood for a good Italian restaurant tonight at 8:30 here in Pittsburgh"

While we can easily interpret and parse the relevant information from there requests, doing so programmatically represents a significant challenge.  Luckily, in a matter of minutes, LUIS can be trained to understand both the users' **Intent** to *Create a Reservation* as well as the relevant information (or **Entities** in LUIS-speak) included in the above statements:

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


We're now ready to start training our app!  Let's navigate back to our *Create Reservation* **intent**.  We are going to train our intent by giving it different examples of the ways people might ask for a reservation.  These examples are what LUIS refers to as **Utterances**.  Note that the more **utterances** we provide for training, the more accurate LUIS will become!

Enter the following **utterance** into your *Create Reservation* **intent** and click *Enter*

```

Make me a reservation in Pitsburgh tomorrow night at 7:30 at a thai restaurant

```

![Create LUIS Intent](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-utterance-1.png)







