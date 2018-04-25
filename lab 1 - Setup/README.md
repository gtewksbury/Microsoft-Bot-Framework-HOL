# Bot Framework Setup

In this lab, we will setup our Visual Studio .NET Bot Framework development environment.  This includes the following:

1.	Installing the Bot Emulator (used for testing our bot locally)
2.	Adding the Visual Studio 2017 Bot Framework project template
3.	Create and running our first *Hello World* bot project
4.	A quick review of the Bot Builder SDK for .NET

> Note, the following labs will require Visual Studio 2017 for Windows


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


To install the Visual Studio Bot Framework Project Template, download the [Bot Application] (http://aka.ms/bf-bc-vstemplate) zip file and save it to your Visual Studio project templates directory

> For typical Visual Studio installations, the *project templates* directory is located at ` %USERPROFILE%\Documents\Visual Studio 2017\Templates\ProjectTemplates\Visual C#\ `
 
Next, download [Bot Controller](http://aka.ms/bf-bc-vscontrollertemplate) and [Bot Dialog](http://aka.ms/bf-bc-vsdialogtemplate) zip files and save them to your Visual Studio item templates directory

> For typical Visual Studio installations, the *item templates* directory is located at ` %USERPROFILE%\Documents\Visual Studio 2017\Templates\ItemTemplates\Visual C#\ `

More information on installing .NET Bot Framework Visual Studio templates can be found [here](https://docs.microsoft.com/en-us/azure/bot-service/dotnet/bot-builder-dotnet-quickstart)

Once the templates has been added successfully, open Visual Studio 2017 (if Visual Studio was previously open, you might have to close it and re-open it for Visual Studio to find your templates)

Navigate to *File* > *New Project* and select *Visual C#* in the *New Project* dialog, and select *Bot Application* (if you don't see this option, you might have to check the template download locations in the previous step).


![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%201%20-%20Setup/images/vs2017-project.png)

At this point you should be all set to start with a brand new Visual Studio bot application.  Before we run it, let's take a moment to review the project structure

> If you're impatient like me, you might try to immediately run the project.  Just make sure you're connected to the internet as project template has a number of *nuget* packages that must be downloaded, including the *Bot Builder SDK*.

## Basic Project Structure

We're almost ready to run our bot an start interfacing with it via the Bot Emulator, but before we do, let's take a brief moment to review the Visual Studio project that was created.


![Bot Emulator](https://github.com/gtewksbury/Microsoft-Bot-Framework-HOL/blob/luis-readme/lab%201%20-%20Setup/images/vs2017-explorer.png)


