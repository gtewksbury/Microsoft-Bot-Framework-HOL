# Lab 2 - Language Understanding Intelligence Service (LUIS)

If you're following the labs in order, at this point we have a pretty basic 'Hello World' bot that that simply listens to the incoming message from the user and returns the number of characters in the message (not very helpful).  Users typically interact with bots through natural language.  It's the bot's job to understand these natural language commands and act accordingly.  This is where LUIS comes in!

LUIS is a natural language processing machine-learning service in Microsoft's Cognitive Service's suite of APIs that can be easily trained to understand the content of natural language content.  Best of all, LUIS provides a FREE tier that we can start using immediately without any upfront cost!

To see where LUIS really shines, let's consider the bot that we will be building out during the remainder of the Labs.  Our bot will help users find restaurants and book reservations.  Think about all the different ways someone might request this if speaking with another person:

* *"I'd like to make a reservation for 6 people in Pittsburgh tomorrow night at 8:30 pm at a nice italian restaurant"* OR

* *"Reserve me a table next Thursday at 8:30 PM at an Italian restaurant in the Pittsburgh area"*

* *Six of us are in the mood for a good Italian restaurant tonight at 8:30 here in Pittsburgh"

While we can easily interpret and parse the relevant information from there requests, doing so programmatically represents a significant challenge.  Luckily, in a matter of minutes, LUIS can be trained to understand both the users' **Intent** to *Create a Reservation* as well as the relevant information (or **Entities** in LUIS-speak) included in the above statements:

1. Where:  **Pittsburgh**
2. When:   **tomorrow night at 8:30 pm**
3. How many people:  **6**
4. What type of food:  **Italian**

Don't believe me?  Let's try it ourselves!

## Getting Started
Log into the [Luis Website](https://luis.ai) to begin building and training your model(s).  

![LUIS Homepage](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-homepage.png)

*Note, to sign into the site, you'll need a valid Microsoft Account.  If you don't already have one, you can create one by clicking the **Sign-Up** button on the home screen.  Additionally, if this is the first time you've logged into the site, you'll likely be taken to a welcome page and asked to fill in a couple of configuration settings.*

### Create a LUIS App
From the following screen, click the **Create New App** button
![Create LUIS App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-blank-app.png)

Give your app a *Name* and *Description*

![Create LUIS App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-create-new-app.png)

Your app is now ready to be built and trained!

*Notice that the new app already contains a default **None** intent.  A single app can and likely will have multiple intents to signify the different requets a user can make (for example, **Create Reservation** vs **Cancel Reservation** are two different intents).  It is the job of LUIS to predict and interpret user's intent.  LUIS will select **None** when no other intents are predicted (for example, if someone asked our reservation app 'What's the weather like in Chicago')*

![Create LUIS App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%202%20-%20LUIS/images/luis-new-app.png)

### Add Create Reservation Intent





