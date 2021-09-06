using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CYOA/room")]
public class Room : ScriptableObject
{
    [TextArea]
    [Tooltip("Text to display on the main content area of this page.")]
    public string description;
    [Tooltip("If set, image will display before text on this page.")]
    public Sprite pageImage;
    [Tooltip("Name for this page.")]
    public string roomName;
    [Tooltip("List of action options the player can take on this page.")]
    public actionOption[] playerActionOptions;
    [Tooltip("List of events to trigger after this page has loaded.")]
    public actionEvent[] pageEventsToTrigger;

    
}
