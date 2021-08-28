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

    public void unpackPlayerActionOptionsInRoom() {
        //Debug.Log("unpacking rooms..");

        for (int i = 0; i < currentRoom.playerActionOptions.Length; i++)
        {
            controller.interactionDescriptionsInRoom.Add(currentRoom.playerActionOptions[i].actionTakenText);
            controller.buttonChoicesTexts.Add(currentRoom.playerActionOptions[i].buttonText);
        }

    }

    public void changeRoom(int buttonNumber){
        
        Debug.Log("changing rooms..");
        // check for event
        if(currentRoom.playerActionOptions[buttonNumber].eventData.eventToTrigger != null) {
            //Debug.Log("changing rooms / activating event..");
            string eventText = currentRoom.playerActionOptions[buttonNumber].eventData.eventToTrigger.activateEvent(controller, currentRoom.playerActionOptions[buttonNumber].eventData.parameters); 
            //Debug.Log(eventText);
            controller.LogStringWithReturn(eventText);
        }

        currentRoom = currentRoom.playerActionOptions[buttonNumber].valueRoom;
        controller.DisplayRoomText();

    }


}
