using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "CYOA/page")]
public class Room : ScriptableObject
{
    [Tooltip("Name for this page.")]
    public string roomName;
    [Tooltip("If set, image will display before text on this page.")]
    public Sprite pageImage;
    [TextArea]
    [Tooltip("Text to display on the main content area of this page if no conditional is met.")]
    public string description;
    [Tooltip("Conditional text that will replace default description.  First matched will replace.")]
    public conditionalPageText[] conditionalPageTexts;
    [Tooltip("Conditional thoughts to add to page content area.")]
    public conditionalThought[] pageThoughts;
    [Tooltip("List of events to trigger after this page has loaded.")]
    public actionEvent[] pageEventsToTrigger;
    [Tooltip("List of action options the player can take on this page, regardless of conditional pages.")]
    public actionOption[] playerActionOptions;
    
    
}
