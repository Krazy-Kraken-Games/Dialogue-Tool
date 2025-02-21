using KKG.Dialogue;
using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Plastic.Newtonsoft.Json;
using UnityEditor;
using UnityEngine;
using MessageType = KKG.Dialogue.MessageType;

namespace KKG.Tool.Dialogue
{
    [Serializable]
    public class DialogueTreeNode
    {
        public string title;
        public Rect rect;
        private Rect resizeHandleRect;
        public bool isDragged;
        public bool isSelected;
        public bool isResizing;

        //Styles
        public GUIStyle style;
        public GUIStyle defaultNodeStyle;
        public GUIStyle selectedNodeStyle;
        public GUIStyle startingNodeStyle;

        public DialogueNodeData data;

        public string SpeakerName;
        public string NodeName;

        public DialogueTreeNode outputNode;

        public Rect outputNodeRect;
        public Rect inputNodeRect;

        private bool showOptions = false;
        private int optionsCount = 0;
        public int OptionCount => optionsCount;
        private bool createOption = false;

        private bool readyToDraw = false;

        public float PositionX => rect.position.x;
        public float PositionY => rect.position.y;
        public float Width => rect.size.x;
        public float Height => rect.size.y;


        //Option Dragging, used for determining nodes of each dialog options
        public Dictionary<DialogueOption,Rect> dialogueOptionRectCollection;

        //Local Cache for options
        public Dictionary<string, DialogueOption> LocalOptions;


        //Constructor to rebuild node again using Node Data

        [JsonConstructor]
        public DialogueTreeNode(NodeData _data)
        {
            if (_data == null) return;

            rect = new Rect(_data.Position.x,_data.Position.y,_data.Size.x,_data.Size.y);

            data = _data.data;

            title = "";

            //Default Style
            defaultNodeStyle = new GUIStyle();
            defaultNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            defaultNodeStyle.border = new RectOffset(12, 12, 12, 12);

            //SelectedStyle
            selectedNodeStyle = new GUIStyle();
            selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
            selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

            //Starting node style
            startingNodeStyle = new GUIStyle();
            startingNodeStyle.normal.background = CreateColorTexture(new Color(0.8f, 0.47f, 0f));
            startingNodeStyle.border = new RectOffset(20, 20, 20, 20);

            style = defaultNodeStyle;


            //Output collection
            dialogueOptionRectCollection = new Dictionary<DialogueOption, Rect>();

            //Initialize local options cache
            LocalOptions = new Dictionary<string, DialogueOption>();

            if(data.Options.Count > 0)
            {
                showOptions = true;

                foreach(var opt in data.Options)
                {
                    LocalOptions.Add(opt.OptionID, opt.Option);
                }
            }

            readyToDraw = true;
        }


        public DialogueTreeNode(Vector2 pos, float width = 300, float height = 175, bool isNew = true,bool _readyToDraw = true)
        {
            rect = new Rect(pos.x,pos.y, width, height);
            title = "";

            //Default Style
            defaultNodeStyle = new GUIStyle();
            defaultNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            defaultNodeStyle.border = new RectOffset(12,12,12, 12);

            //SelectedStyle
            selectedNodeStyle = new GUIStyle();
            selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
            selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

            //Starting node style
            startingNodeStyle = new GUIStyle();
            startingNodeStyle.normal.background = CreateColorTexture(new Color(0.8f,0.47f,0f));
            startingNodeStyle.border = new RectOffset(20,20,20, 20);    

            style = defaultNodeStyle;

            string nodeID = string.Empty;
            if(isNew)
            {
                nodeID = Guid.NewGuid().ToString();
            }

            //Add data 
            data = new DialogueNodeData()
            {
                Id = nodeID,
                Type = MessageType.DEFAULT,
                SpeakerName = string.Empty,
                Message = string.Empty,
                Options = new List<DialogueOptionPacket>(),
                nextIndex = null
            };

            //Output collection
            dialogueOptionRectCollection = new Dictionary<DialogueOption, Rect>();

            //Initialize local options cache
            LocalOptions = new Dictionary<string, DialogueOption>();

            readyToDraw = _readyToDraw;
        }

        public void Draw()
        {
            if (!readyToDraw) return;

            GUI.Box(rect,title, style);

            GUILayout.BeginArea(new Rect(rect.x + 20,rect. y + 20,rect.width - 40, rect.height - 40));

            GUILayout.Label($"ID:{data.Id}", EditorStyles.boldLabel);

            // Message Type
            GUILayout.BeginHorizontal();
            GUILayout.Label("Message Type:");
            data.Type = (MessageType)EditorGUILayout.EnumPopup(data.Type);
            GUILayout.EndHorizontal();

            // Speaker Name
            GUILayout.Label("Speaker Name:");
            data.SpeakerName = EditorGUILayout.TextField(data.SpeakerName);

            // Message
            GUILayout.Label("Message:");
            data.Message = EditorGUILayout.TextArea(data.Message);

            //Next Index
            GUILayout.Label($"Next Index:  {data.nextIndex}");

            //Toggle button to show/Hide Options area
            if (GUILayout.Button("Toggle Dialogue Options"))
            {
                showOptions = !showOptions;
            }


            if (showOptions)
            {
                // Display all existing options
                List<string> keysToRemove = new List<string>(); // Track keys for removal

                if(data.Options.Count > 0) 
                {

                    foreach (var keyValuePair in data.Options)
                    {
                        string optionId = keyValuePair.OptionID;
                        DialogueOption option = keyValuePair.Option;

                        GUILayout.BeginVertical("box"); // Group each option in a box for clarity

                        // Option ID (Key)
                        GUILayout.BeginHorizontal();
                        GUILayout.Label($"Option ID: {optionId}");
                        string newOptionId = optionId;
                        GUILayout.EndHorizontal();

                        //if (newOptionId != optionId && !data.Options.ContainsKey(newOptionId))
                        //{
                        //    // Rename key if it changed and doesn't already exist
                        //    data.Options.Remove(optionId);
                        //    data.Options[newOptionId] = option;
                        //    break; // Exit loop to avoid modifying the dictionary during iteration
                        //}

                        // Option Message
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Option Message:");
                        string updatedMessage = EditorGUILayout.TextField(option.OptionMessage);

                        if (!string.Equals(updatedMessage, option.OptionMessage))
                        {
                            option.OptionMessage = updatedMessage;
                            LocalOptions[optionId] = option;
                        }

                        GUILayout.EndHorizontal();

                        // Speaker Name
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Speaker Name:");
                        string updatedSpeakerName = EditorGUILayout.TextField(option.SpeakerName);

                        //Detect change
                        if (!string.Equals(updatedSpeakerName, option.SpeakerName))
                        {
                            option.SpeakerName = updatedSpeakerName;
                            LocalOptions[optionId] = option;
                        }

                        GUILayout.EndHorizontal();

                        // Next Index
                        GUILayout.BeginHorizontal();
                        GUILayout.Label($"Next Index:  {option.NextIndex}", GUILayout.Width(150));

                        // Draw output node as a small button
                        Rect outputNodeRect = GUILayoutUtility.GetRect(15, 15); // Adjust size as needed
                        EditorGUI.DrawRect(outputNodeRect, Color.green); // Draw the node

                        if (!dialogueOptionRectCollection.ContainsKey(option))
                        {
                            //Add the output rect 
                            dialogueOptionRectCollection.Add(option, outputNodeRect);
                        }

                        //Store the rect information in the Option Data packet
                        keyValuePair.rect = outputNodeRect;

                        // Make it interactive (clickable and draggable)
                        if (Event.current.type == EventType.MouseDown && outputNodeRect.Contains(Event.current.mousePosition))
                        {
                            Debug.Log($"Output node for Option '{optionId}' clicked!");
                            Vector2 startPosition = rect.position + outputNodeRect.center;
                            DialogueNodeTool.StartDraggingConnection(this, optionId, startPosition, outputNodeRect, option);
                            Event.current.Use(); // Consume the event
                        }

                        GUILayout.EndHorizontal();

                        // Remove option button
                        if (GUILayout.Button("Remove Option", GUILayout.Width(150)))
                        {
                            DialogueNodeTool instance = DialogueNodeTool.GetWindow<DialogueNodeTool>();
                            instance.RemovingOptionConnectionWithID(optionId);
                            keysToRemove.Add(optionId);
                            optionsCount--;
                            createOption = false; // Reset the flag
                            break; // Exit loop to avoid modifying the dictionary during iteration
                        }

                        GUILayout.EndVertical(); // End of option box
                    }

                    // Apply updates after the loop
                    foreach (var updatedOption in LocalOptions)
                    {
                        var optionPacket = data.Options.Single(opt => opt.OptionID == updatedOption.Key);
                        optionPacket.Option = updatedOption.Value;
                    }
                }


                
                // Remove options marked for deletion
                foreach (string key in keysToRemove)
                {
                    var optionPacket = data.Options.Single(opt => opt.OptionID == key);
                    data.Options.Remove(optionPacket);
                }
                // Add new option
                GUILayout.Space(10);
                GUILayout.BeginHorizontal();
                GUILayout.Label("");
                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    CreateDialogueOption();
                }
                GUILayout.EndHorizontal();
            }

            GUILayout.EndArea();

            //Input Node
            DrawInputNode();

            //Output Node
            DrawOutputNode();

            DrawResizeHandle();
        }

        private void CreateDialogueOption()
        {
            // Create a new dialogue option and add it to the list
            DialogueOption newOption = new DialogueOption
            {
                OptionId = Guid.NewGuid().ToString(),
                NextIndex = string.Empty, // Default to empty string to indicate no next index yet
                SpeakerName = $"Speaker {optionsCount + 1}", // Default speaker name
                OptionMessage = $"Option Message {optionsCount + 1}", // Default message

                parentNodeRef = data.Id
            };

            //Adds Option ID as Key and Dialogue Option as Value 
            DialogueOptionPacket optionPacket = new DialogueOptionPacket(newOption.OptionId, newOption, new Rect(0,0,0,0));
            data.Options.Add(optionPacket); // Add to the options list
            optionsCount++; // Increment the options count
            createOption = false; // Reset the flag

            //Notify Tool about it
            DialogueNodeTool tool = DialogueNodeTool.GetWindow<DialogueNodeTool>();
            tool.RemoveFromConnection(data.Id);

            //Resetting next Index jump
            SetNextIndex(null);
        }

        public DialogueOptionPacket CreateOptionWithData(DialogueOptionPacket _OptionPacket)
        {
            //data.Options.Add(_Option.OptionId,_Option);
            LocalOptions.Add(_OptionPacket.OptionID, _OptionPacket.Option);
            optionsCount++;

            return _OptionPacket;
        }

        public void AddNextIndexOnTool(string _optionId, string nextIndex)
        {
            var option = data.Options.Single(opt => opt.OptionID == _optionId);

            option.Option.NextIndex = nextIndex;
        }

        public void AllowDrawingNode()
        {
            readyToDraw = true;
        }

        private void DrawInputNode()
        {
            const float nodeSize = 20f;

            inputNodeRect = new Rect(rect.x, rect.y + 20f, nodeSize, nodeSize);

            // Load your texture (this assumes you have a texture at the specified path).
            Texture2D inputTexture = EditorGUIUtility.Load("Assets/KKG/Editor/Textures/InputCircle.png") as Texture2D;

            if (inputTexture != null)
            {
                GUI.DrawTexture(inputNodeRect, inputTexture);
            }
            else
            {
                EditorGUI.DrawRect(inputNodeRect, Color.green);
            }
        }

        private void DrawOutputNode()
        {
            if (data.Options == null)
            {
                data.Options = new List<DialogueOptionPacket>();
            }

            if (data.Options.Count > 0) return;


            const float nodeSize = 20f;

            outputNodeRect = new Rect(rect.xMax, rect.y + 20f, nodeSize, nodeSize);


            // Load your texture (this assumes you have a texture at the specified path).
            Texture2D outputTexture = EditorGUIUtility.Load("Assets/KKG/Editor/Textures/OutputCircle.png") as Texture2D;

            if (outputTexture != null)
            {
                GUI.DrawTexture(outputNodeRect, outputTexture);
            }
            else
            {
                EditorGUI.DrawRect(outputNodeRect, Color.green);
            }
        }

        private void DrawResizeHandle()
        {
            const float handleSize = 30f;

            resizeHandleRect = new Rect(rect.xMax - handleSize, rect.yMax - handleSize, handleSize, handleSize);

            // Load your texture (this assumes you have a texture at the specified path).
            Texture2D handleTexture = EditorGUIUtility.Load("Assets/KKG/Editor/Textures/ResizeArrows.png") as Texture2D;

            // If the texture is loaded successfully, draw it; otherwise, fallback to a colored rectangle.
            if (handleTexture != null)
            {
                GUI.DrawTexture(resizeHandleRect, handleTexture);
            }
            else
            {
                EditorGUI.DrawRect(resizeHandleRect, Color.yellow);
            }

            EditorGUIUtility.AddCursorRect(resizeHandleRect, MouseCursor.ResizeUpLeft);

            Event e = Event.current;

            if (e.type == EventType.MouseDown && resizeHandleRect.Contains(e.mousePosition))
            {
                isResizing = true;
                e.Use();
            }

            if (isResizing)
            {
                if (e.type == EventType.MouseDrag)
                {
                    // Update width and height
                    float newWidth = Mathf.Max(100, e.mousePosition.x - rect.x);
                    float newHeight = Mathf.Max(50, e.mousePosition.y - rect.y);

                    // Avoid unnecessary repaints if size hasn't changed
                    if (!Mathf.Approximately(rect.width, newWidth) || !Mathf.Approximately(rect.height, newHeight))
                    {
                        rect.width = newWidth;
                        rect.height = newHeight;
                        GUI.changed = true; // Mark GUI as changed to trigger repaint
                    }
                }
                else if (e.type == EventType.MouseUp)
                {
                    isResizing = false;
                    e.Use(); // Mark event as used
                }
            }
        }

        public void Drag(Vector2 delta)
        {
            rect.position += delta;
        }

        /// <summary>
        /// Checks if the mouse position lands within a rect
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <returns></returns>
        public bool IsSelected(Vector2 mousePosition)
        {
            return rect.Contains(mousePosition);
        }

        public bool ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:

                    if (resizeHandleRect.Contains(e.mousePosition))
                    {
                        Debug.Log("Trying to resize?");
                    }
                    else
                    {
                        //Right to drag and drop
                        if (rect.Contains(e.mousePosition))
                        {
                            //Mouse click happened within bounds of node rect
                            isDragged = true;
                            isSelected = true;

                            if (style != startingNodeStyle)
                            {
                                style = selectedNodeStyle;
                            }

                            GUI.changed = true;
                        }
                        else
                        {
                            isSelected = false;

                            if(style != startingNodeStyle)
                            {
                                style = defaultNodeStyle;
                            }
                           
                            GUI.changed = true;
                        }
                    }
                        return true;

                case EventType.MouseUp:
                    isDragged = false;
                    break;

                case EventType.MouseDrag:

                    if(e.button == 2 && isDragged)
                    {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }

            return false;
        }

        public void SetNextIndex(string _nextNodeID)
        {
            data.nextIndex = _nextNodeID;
        }

        //Visual Change methods

        // Method to create a Texture2D with a solid color
        Texture2D CreateColorTexture(Color color)
        {
            Texture2D texture = new Texture2D(1, 1, TextureFormat.ARGB32, false);
            texture.SetPixel(0, 0, color);
            texture.Apply();
            return texture;
        }

        /// <summary>
        /// Sets the node as the starting node
        /// </summary>
        public void SetStartingNode()
        {
            style = startingNodeStyle;
        }

        public void SetNormalNode()
        {
            style = defaultNodeStyle;
        }

        public void SetShowOptions(bool _show)
        {
            showOptions = _show;
        }

    }
}
