using KKG.Dialogue;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using MessageType = KKG.Dialogue.MessageType;
using System;

namespace KKG.Tool.Dialogue
{
    public class Node
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

        public DialogueNodeData data;

        public string SpeakerName;
        public string NodeName;

        public Node(Vector2 pos, float width = 300, float height = 175)
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

            style = defaultNodeStyle;

            //Add data 
            data = new DialogueNodeData()
            {
                Id = Guid.NewGuid().ToString(),
                Type = MessageType.DEFAULT,
                SpeakerName = string.Empty,
                Message = string.Empty,
                Options = new List<DialogueOption>(),
                jumpIndex = null
            };
        }

        public void Draw()
        {
            GUI.Box(rect,title, style);

            GUILayout.BeginArea(new Rect(rect.x + 20,rect. y + 20,rect.width - 40, rect.height - 40));

            GUILayout.Label("Data", EditorStyles.boldLabel);

            // Message Type
            GUILayout.Label("Message Type:");
            data.Type = (MessageType)EditorGUILayout.EnumPopup(data.Type);

            // Speaker Name
            GUILayout.Label("Speaker Name:");
            data.SpeakerName = EditorGUILayout.TextField(data.SpeakerName);

            // Message
            GUILayout.Label("Message:");
            data.Message = EditorGUILayout.TextField(data.Message);

            //Jump Index
            GUILayout.Label("Jump Index");
            data.jumpIndex = EditorGUILayout.IntField(data.jumpIndex.Value);

            GUILayout.EndArea();

            DrawResizeHandle();
        }

        private void DrawResizeHandle()
        {
            const float handleSize = 10f;

            resizeHandleRect = new Rect(rect.xMax - handleSize, rect.yMax - handleSize, handleSize, handleSize);

            EditorGUI.DrawRect(resizeHandleRect, Color.yellow);

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
                            style = selectedNodeStyle;
                            GUI.changed = true;
                        }
                        else
                        {
                            isSelected = false;
                            style = defaultNodeStyle;
                            GUI.changed = true;
                        }
                    }
                        return true;

                case EventType.MouseUp:
                    isDragged = false;
                    break;

                case EventType.MouseDrag:

                    if(e.button == 1 && isDragged)
                    {
                        Drag(e.delta);
                        e.Use();
                        return true;
                    }
                    break;
            }

            return false;
        }
    }
}
