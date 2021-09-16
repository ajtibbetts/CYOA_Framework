using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class roomNavigation : MonoBehaviour
{
    public Room currentRoom;
    private gameController controller;

    void Awake() {
        controller = GetComponent<gameController>();
    }

    public bool hasPageImage(){
        // activate page image if set
        if(currentRoom.pageImage != null) {
            return true;
        }
        return false;
    }

    public void unpackPlayerActionOptionsInRoom() {
        //Debug.Log("unpacking rooms..");

        // get player action options
        for (int i = 0; i < currentRoom.playerActionOptions.Length; i++)
        {
            controller.interactionDescriptionsInRoom.Add(currentRoom.playerActionOptions[i].actionTakenText);
            controller.buttonChoicesTexts.Add(currentRoom.playerActionOptions[i].buttonText);
        }

    }

    public void triggerPageEvents() {
        // triggers any page events 
        if(currentRoom.pageEventsToTrigger.Length > 0) {
            foreach(actionEvent e in currentRoom.pageEventsToTrigger) {
                if(e.eventToTrigger != null) {
                    controller.LogStringWithReturn(e.eventToTrigger.activateEvent(controller, e.parameters));
                }
            }
        }
    }

    public void changeRoom(int buttonNumber){
        
        Debug.Log("changing rooms..");
        // check for event

        // Debug.Log("Checking for events to fire on this selected option..");
        if(currentRoom.playerActionOptions[buttonNumber].eventsToTrigger.Length > 0) {
            // Debug.Log("Events found. Firing off events");
            foreach(actionEvent e in currentRoom.playerActionOptions[buttonNumber].eventsToTrigger) {
                if(e.eventToTrigger != null) {
                    controller.LogStringWithReturn(e.eventToTrigger.activateEvent(controller, e.parameters));
                }
            }
            
        }

        currentRoom = currentRoom.playerActionOptions[buttonNumber].valueRoom;
        controller.DisplayRoomText();

    }


}
