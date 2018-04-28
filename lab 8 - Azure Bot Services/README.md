# Azure Bot Services
Up until this point, we've been interacting with our bot through our local emulator, but how do you actually host your bot so it can be consumed by external **channels**?  In this lab, we'll learn how to deploy our bot application to an Azure Bot Service via Visual Studio

## Create Azure Web App Bot
Log into the [Azure Portal](https://portal.azure.com), and click the *Create a resource* button on the top left of your screen.  On the following screen, select *AI + Cognitive Services* and click *Web App Bot*

![Create Azure Web Bot App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%208%20-%20Azure%20Bot%20Services/images/azure-create-resource.png)

Enter the required settings and click the *Create* button

![Create Azure Web Bot App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%208%20-%20Azure%20Bot%20Services/images/azure-settings.png)

* **Bot name** must be globally unique.  This will be included in the URL to your Bot API
* **Pricing Tier** *F0* is free of charge
* **Bot Template** should be *Basic (C#)*

> The remaining settings are outside the scope of this tutorial, but the defaults should be fine for our purposes

Once created, you can navigate to your new *Web App Bot* within Azure.  It should look something like this:

![Create Azure Web Bot App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%208%20-%20Azure%20Bot%20Services/images/azure-overview.png)

## Publish
The following section walks through publishing your bot through Visual Studio.

> While Visual Studio deployments are fine for our purposes, it's highly recommended to publish production bots through automated CI / CD pipelines.

Open you bot solution within Visual Studio, right click you bot project, and select *Publish*

![Create Azure Web Bot App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%208%20-%20Azure%20Bot%20Services/images/vs-publish.png)

On the following screen, select *Microsoft Azure App Service*, and chose *Select Existing*

![Create Azure Web Bot App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%208%20-%20Azure%20Bot%20Services/images/vs-publish-step1.png)

Chose your Azure subscription in the dropdown, select the app you just created, and click *OK*

![Create Azure Web Bot App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%208%20-%20Azure%20Bot%20Services/images/vs-select-app.png)


> Visual Studio will immediately start deploying your application to Azure.  However, once finished we'll need to make a quick change and redeploy.  We'll talk about why in a minute.  

Once Visual Studio is done deploying, click the *Settings* link as shown below:

![Create Azure Web Bot App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%208%20-%20Azure%20Bot%20Services/images/vs-settings.png)

In the *Publish* dialog, select *Settings*, and check *Remove additional files at destination* under the *File Publish Options*

![Create Azure Web Bot App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%208%20-%20Azure%20Bot%20Services/images/vs-remove.png)

> This tells *WebDeploy* to remove any existing files in the deployment destination prior to deploying your code.  This is important because the *Web Bot App* we created in Azure already has a sample application deployed.  When Visual Studio immediately deployed in the previous step, it did NOT remove these files.  Therefore, with this value set we'll need to deploy again.

Click the *Publish* button as shown below:

![Create Azure Web Bot App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%208%20-%20Azure%20Bot%20Services/images/vs-final-publish.png)

Congratulations!  You're bot should now be successfully hosted within a durable Azure App Services environment!

## Test Web Chat
Let's go back to our app in Azure and explore a bit.  You should notice a *Test in Web Chat* option in the side menu.  Go ahead and click it.

This is a test area for your deployed bot.  Go ahead and type a message to see if it's working!

![Create Azure Web Bot App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%208%20-%20Azure%20Bot%20Services/images/azure-test.png)

By default, *Web App Bots* have enabled the *Web Chat* **channel** (we'll see where this is done in a moment).  This same web chat experience can easily be added to your own web applications via [Microsoft Bot Frameworks Embedded Web Chat Control](https://github.com/Microsoft/BotFramework-WebChat).

## Channels

Let's click the *Channels* link in the Azure Portal.  This is where you configure additional **channels** for your application.  You can see I've enabled the *Slack* **channel** as well.

![Create Azure Web Bot App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%208%20-%20Azure%20Bot%20Services/images/azure-web-chat.png)

And here's what our bot looks like in *Slack*

![Create Azure Web Bot App](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/master/lab%208%20-%20Azure%20Bot%20Services/images/bot-slack.png)

Thankfully, Microsoft provides step-by-step instructions for configuring each of the supported channels!

## Quick Recap
In this lab, we learned how to create a durable bot hosting environment in Azure and how to deploy our bot through Visual Studio.