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

    public void unpackExitsInRoom() {
        Debug.Log("unpacking rooms..");

        for (int i = 0; i < currentRoom.exits.Length; i++)
        {
            controller.interactionDescriptionsInRoom.Add(currentRoom.exits[i].exitDescription);
            controller.buttonChoicesTexts.Add(currentRoom.exits[i].buttonText);
        }

    }

    public void changeRoom(int buttonNumber){
        
        Debug.Log("changing rooms..");
        // check for event
        if(currentRoom.exits[buttonNumber].eventData.eventToTrigger != null) {
            Debug.Log("changing rooms / activating event..");
            string eventText = currentRoom.exits[buttonNumber].eventData.eventToTrigger.activateEvent(controller, currentRoom.exits[buttonNumber].eventData.parameters); 
            Debug.Log(eventText);
            controller.LogStringWithReturn(eventText);
        }

        currentRoom = currentRoom.exits[buttonNumber].valueRoom;
        controller.DisplayRoomText();

    }


}
