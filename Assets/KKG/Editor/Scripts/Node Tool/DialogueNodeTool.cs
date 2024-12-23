using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace KKG.Tool.Dialogue
{
    public class DialogueNodeTool : EditorWindow
    {
        private List<Node> nodes;

        [MenuItem("Tools/Krazy Kraken Games/Dialogue Graph")]
        public static void OpenWindow()
        {
            DialogueNodeTool window = GetWindow<DialogueNodeTool>();
            window.titleContent = new GUIContent("Dialogue Graph");
        }

        private void OnEnable()
        {
            nodes = new List<Node>();
        }

        private void OnGUI()
        {
            //Draw the grid
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.green);

            //Draw any nodes that exist
            DrawNodes();

            //Events Handling
            ProcessEvents(Event.current);

            //Repaint
            if (GUI.changed) Repaint();
        }

        private void DrawGrid(float gridSpacing,float gridOpacity,Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i,0,0),new Vector3(gridSpacing * i, position.height,0));
            }

            for(int j = 0; j< heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(0,gridSpacing * j,0),new Vector3(position.width,gridSpacing * j,0f));  
            }

            Handles.color = Color.white;

            Handles.EndGUI();
        }

        /// <summary>
        /// Method to draw all the nodes that exist in the list
        /// </summary>
        private void DrawNodes()
        {
            if (nodes != null && nodes.Count > 0)
            {
                foreach(Node node in nodes)
                {
                    node.Draw();
                }
            }
        }


        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:

                    if(e.button == 1)
                    {
                        ShowContextMenu(e.mousePosition);
                    }
                    break;
            }
        }


        #region In Tool Functions

        private void ShowContextMenu(Vector2 position)
        {
            GenericMenu menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Node"), false, () => OnClickAddNode(position));
            menu.ShowAsContext();
        }

        private void OnClickAddNode(Vector2 clickPosition)
        {
            nodes.Add(new Node(clickPosition));
        }

        #endregion
    }
}
