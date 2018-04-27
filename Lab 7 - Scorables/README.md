#Scorables
Throughout the labs, we've learned that bot applications can be made of multiple **Dialogs**, and all user responses are passed to the top (or active) **Dialog** in the **DialogStack**.  But what if you want to globally handle incoming messages from users, regardless of the current active **Dialog**.  For example, if the user types *'help'*, we should bring up help options, regardless of the current **Dialog**.  Another example would be allowing the user to *'cancel'* a conversation.

In this short lab, we'll learn how this can be done using Bot Framework's **Scorables**.

