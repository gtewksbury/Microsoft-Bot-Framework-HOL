
*    **RootDialog**
    *   Gather the state provided by the user's initial request and *Call* the *LocationDialog*
*    **LocationDialog**     
    *    *StartAsync*
        *    IF the location was NOT retrieved from the original request, ask the user for their preferred location and *Wait* for their response
        *    OTHERWISE, *Call* the *CuisineDialog*
    *   *MessageReceivedAsync*
        *   IF the user-provided location is valid, save to state and *Call* the *CuisineDialog*
        *   OTHERWISE, notify the user that the location was not found and *Wait* for their response
*    **CuisineDialog**     
    *    *StartAsync*
         *    IF the cuisine was NOT retrieved from the original request, ask the user for their preferred cuisine and *Wait* for their response
        *    OTHERWISE, *Call* the *RestaurantDialog*
    *   *MessageReceivedAsync*
         *   IF the user-provided cusine is valid, save to state and *Call* the *RestaurantDialog*
         *   OTHERWISE, notify the user that the cuisine was not found and *Wait* for their response  
*    **RestaurantDialog**     
       * *StartAsync*
          *    Ask the user for their preferred restaurant and *Wait* for their response
       *   MessageReceivedAsync
          *   IF the user-provided restaurant is valid, save to state and *Call* the *WhenDialog*
          *   OTHERWISE, notify the user that the restaurant was not found and *Wait* for their response            
*    **WhenDialog**     
       *    *StartAsync*
        *    IF the reservation date / time was NOT retrieved from the original request, ask the user for their preferred time and *Wait* for their response
        *    OTHERWISE, *Call* the *PartySizeDialog*
    *   *MessageReceivedAsync*
           *   IF the user-provided date / time is valid, save to state and *Call* the *PartySizeDialog*
           *   OTHERWISE, notify the user that the provided date / time is invalid and *Wait* for their response  
*    **PartySizeDialog**     
       *    *StartAsync*
           *    IF the party size was NOT retrieved from the original request, ask the user for their preferred number of people and *Wait* for their response
           *    OTHERWISE, *Call* the *ConfirmReservationDialog* and register a *Done* handler
       *   *MessageReceivedAsync*
           *   IF the user-provided party size is valid, save to state and *Call* the *ConfirmReservationDialog* and register a *Done* handler
           *   OTHERWISE, notify the user that the provided party size is invalid and *Wait* for their response  
       *   *ConfirmationDoneHandler*
          * Notify the user that their reservation has been booked and end the conversation   
*    **ConfirmReservationDialog**     
       *    *StartAsync*
           *    Ask the user to confirm the reservation and *Wait* for their response
       *   *MessageReceivedAsync*
           *   IF the user-provided a valid confirmation, save to state and call *Done*
           *   OTHERWISE, ask the user to confirm their reservation and *Wait* for their response  
