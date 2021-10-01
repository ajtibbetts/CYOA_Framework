using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;

public class NodeSearchWindow : ScriptableObject, ISearchWindowProvider
{
   
   private DialogueGraphView _graphView;
   private EditorWindow _window;
   private Texture2D _indentationIcon;

   public void Init(EditorWindow window, DialogueGraphView graphView)
   {
       _window = window;
       _graphView = graphView;

        // indentation hack for search window as a transparent icon
       _indentationIcon = new Texture2D(1,1);
       _indentationIcon.SetPixel(0,0,new Color(0,0,0,0));  // dont forget to set the alpha to 0 as well
       _indentationIcon.Apply();
   }
   
   
   public List<SearchTreeEntry> CreateSearchTree(SearchWindowContext context)
   {
       var tree = new List<SearchTreeEntry>
       {
            new SearchTreeGroupEntry(new GUIContent("Create Elements"), 0),
            new SearchTreeGroupEntry(new GUIContent("Dialogue"), 1),
            new SearchTreeEntry(new GUIContent("Dialogue Node",_indentationIcon))
            {
               userData = new DialogueNode(), level = 2
            },
            new SearchTreeGroupEntry(new GUIContent("Check Nodes"), 1),
                new SearchTreeEntry(new GUIContent("playerSkill",_indentationIcon))
                {
                    userData = new CheckNode(), level = 2
                },
                new SearchTreeEntry(new GUIContent("playerProperty",_indentationIcon))
                {
                    userData = new CheckNode(), level = 2
                },
                new SearchTreeEntry(new GUIContent("npcProperty",_indentationIcon))
                {
                    userData = new CheckNode(), level = 2
                },
                new SearchTreeEntry(new GUIContent("enemyProperty",_indentationIcon))
                {
                    userData = new CheckNode(), level = 2
                },
                new SearchTreeEntry(new GUIContent("questProperty",_indentationIcon))
                {
                    userData = new CheckNode(), level = 2
                },
                new SearchTreeEntry(new GUIContent("storyProperty",_indentationIcon))
                {
                    userData = new CheckNode(), level = 2
                },
                new SearchTreeEntry(new GUIContent("itemProperty",_indentationIcon))
                {
                    userData = new CheckNode(), level = 2
                },
                new SearchTreeEntry(new GUIContent("worldProperty",_indentationIcon))
                {
                    userData = new CheckNode(), level = 2
                },
            new SearchTreeGroupEntry(new GUIContent("Event Nodes"), 1),
                new SearchTreeEntry(new GUIContent("player",_indentationIcon))
                {
                    userData = new EventNode(), level = 2
                },
                new SearchTreeEntry(new GUIContent("skill",_indentationIcon))
                {
                    userData = new EventNode(), level = 2
                },
                new SearchTreeEntry(new GUIContent("item",_indentationIcon))
                {
                    userData = new EventNode(), level = 2
                },
                new SearchTreeEntry(new GUIContent("npc",_indentationIcon))
                {
                    userData = new EventNode(), level = 2
                },
                new SearchTreeEntry(new GUIContent("enemy",_indentationIcon))
                {
                    userData = new EventNode(), level = 2
                },
                new SearchTreeEntry(new GUIContent("quest",_indentationIcon))
                {
                    userData = new EventNode(), level = 2
                },
                new SearchTreeEntry(new GUIContent("story",_indentationIcon))
                {
                    userData = new EventNode(), level = 2
                },
                new SearchTreeEntry(new GUIContent("world",_indentationIcon))
                {
                    userData = new EventNode(), level = 2
                },
                new SearchTreeEntry(new GUIContent("game",_indentationIcon))
                {
                    userData = new EventNode(), level = 2
                },
            new SearchTreeEntry(new GUIContent("ENDPOINT Node",_indentationIcon))
            {
                userData = new EndpointNode(), level = 1
            }
       };
       return tree;
   }

   public bool OnSelectEntry(SearchTreeEntry SearchTreeEntry, SearchWindowContext context)
   {
        var worldMousePosition = _window.rootVisualElement.ChangeCoordinatesTo(_window.rootVisualElement.parent,
            context.screenMousePosition - _window.position.position);
        var localMousePosition = _graphView.contentViewContainer.WorldToLocal(worldMousePosition);
        var nodeName = SearchTreeEntry.name;
        switch(SearchTreeEntry.userData)
        {
            
            case EndpointNode endpointNode:
                _graphView.CreateNode("ENDPOINT",localMousePosition);
                return true;
            case EventNode eventNode:
                eventType eventType;
                if(Enum.TryParse(nodeName, out eventType))
                {
                    if (Enum.IsDefined(typeof(eventType), eventType) | eventType.ToString().Contains(","))
                    {
                        // Debug.Log($"Converted string {nodeName} to enum conditionType {checkType.ToString()}");
                        string nodeNamePrefix = eventType.ToString();

                        _graphView.CreateNode("Event Node", localMousePosition, nodeNamePrefix, eventType.ToString());
                        Debug.Log("creating check node with name " + nodeNamePrefix + " " + nodeName);
                        return true; 
                    }
                    return false;
                }
                return false;
            case CheckNode checkNode:
                // check for node name against condition enum type
                conditionType checkType;
                if(Enum.TryParse(nodeName, out checkType))
                {
                    if (Enum.IsDefined(typeof(conditionType), checkType) | checkType.ToString().Contains(","))
                    {
                        // Debug.Log($"Converted string {nodeName} to enum conditionType {checkType.ToString()}");
                        string nodeNamePrefix = dataFormatter.getConditionText(checkType);

                        _graphView.CreateNode("Check Node", localMousePosition, nodeNamePrefix, checkType.ToString());
                        Debug.Log("creating check node with name " + nodeNamePrefix + " " + nodeName);
                        return true; 
                    }
                    return false;
                }
                return false;
            case DialogueNode dialogueNode:
                _graphView.CreateNode("Dialogue Node", localMousePosition);
                return true;
            default:
                return false;
        }
   }

}
