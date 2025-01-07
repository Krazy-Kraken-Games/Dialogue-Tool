using KKG.Dialogue;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KKG.Tool.Dialogue
{

    public struct ConnectionTuple
    {
        public Node InputNode;
        public Node OutputNode;

        public ConnectionTuple(Node inputNode, Node outputNode)
        {
            InputNode = inputNode;
            OutputNode = outputNode;
        }
    }

    public class DialogueNodeTool : EditorWindow
    {

        [SerializeField]
        private string fileName = string.Empty;

        private List<Node> nodes;
        private Dictionary<ConnectionTuple, Connection> connections;
        

        private Node selectedNode;

        private Vector2 dragStartPosition;
        private Vector2? dragEndPosition;
        private bool isDraggingConnection;
        private bool isResizing;

        private float zoom = 1.0f; // Default zoom level
        private float zoomScale = 1.0f;
        private Vector2 panOffset = Vector2.zero; // Offset for panning
        private Vector2 dragStart = Vector2.zero; // Track drag start position

        private Vector2 canvasSize = new Vector2(5000, 5000); // Large virtual canvas
        private Vector2 scrollPosition = Vector2.zero;       // Scroll view position

        [MenuItem("Tools/Krazy Kraken Games/Dialogue Graph")]
        public static void OpenWindow()
        {
            DialogueNodeTool window = GetWindow<DialogueNodeTool>();
            window.titleContent = new GUIContent("Dialogue Graph");
        }

        private void OnEnable()
        {
            nodes = new List<Node>();
            connections = new Dictionary<ConnectionTuple, Connection>();   
            selectedNode = null;
            isDraggingConnection = false;
        }

        private void OnGUI()
        {
            Event e = Event.current;
            HandleZoom(e);   // Zoom in/out

            if (selectedNode == null)
            {
               HandlePanning(e); // Drag canvas
            }

            ApplyZoomAndPan(); // Apply transformations

            // Begin scrollable view for the virtual canvas
            scrollPosition = GUI.BeginScrollView(
                new Rect(0, 0, position.width, position.height),
                scrollPosition,
                new Rect(0, 0, canvasSize.x * zoom, canvasSize.y * zoom)
            );

            // Apply zoom and pan transformations
            GUI.BeginGroup(new Rect(panOffset.x, panOffset.y, canvasSize.x * zoom, canvasSize.y * zoom));



            //Draw the grid
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.red);


            // Draw resize handle for user to resize the canvas
            DrawResizeHandle();

            // Handle resizing interaction
            //HandleResize(e);


            //Draw any nodes that exist
            DrawNodes();
            DrawConnections();

            //Events Handling
            ProcessDragging(Event.current);
            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

            if(isDraggingConnection)
            {
                DrawDraggingConnection();
            }

            //Repaint
            if (GUI.changed) Repaint();

            GUI.matrix = Matrix4x4.identity; // Reset matrix

            GUI.EndGroup(); // End scaling and panning

            GUI.EndScrollView(); // End the scroll view

            Rect inspectorRect = new Rect(position.width - 200, 0, 200, position.height);
            DrawInspectorPanel(inspectorRect);

            Rect instructionRect = new Rect(position.width - 200, position.height - 200, 200, 200);
            DrawInstructionsPanel(instructionRect);

        }

        private void UpdateCanvasSize()
        {
            // Increase or decrease the canvas size based on zoom or user interaction
            canvasSize.x = Mathf.Max(5000f, canvasSize.x * zoomScale); // Minimum size 5000, adjust with zoom
            canvasSize.y = Mathf.Max(5000f, canvasSize.y * zoomScale); // Minimum size 5000, adjust with zoom
        }

        #region DRAWING METHODS

        private void DrawInstructionsPanel(Rect rect)
        {
            GUI.Box(rect,"Instructions",GUI.skin.window);

            GUILayout.BeginArea(new Rect(rect.x + 30, rect.y + 30, rect.width - 60, rect.height - 60));

            GUILayout.Label("Press Middle Mouse Button to create nodes");
            GUILayout.Label("Press Left mouse button on nodes to drag and create connections");
            GUILayout.Label("Press Right mouse button on nodes to drag and move the nodes");

            GUILayout.EndArea();
        }

        private void DrawInspectorPanel(Rect rect)
        {
            GUI.Box(rect, "Inspector", GUI.skin.window);

            GUILayout.BeginArea(new Rect(rect.x + 5,rect.y + 5, rect.width - 10, rect.height - 10));

            GUILayout.Space(10);

            GUILayout.Label("File Name:");

            fileName = EditorGUILayout.TextField(fileName);

            if (selectedNode != null)
            {
                GUILayout.Label("Node selected");
            }
            else
            {
                GUILayout.Label("No selected Node");
            }


            if (nodes.Count > 0)
            {
                //Button press to create Dialogue asset
                if (GUILayout.Button("Create Dialogue Asset"))
                {

                    List<DialogueNode> DialogueNodes = new List<DialogueNode>();
                    Dictionary<Node,DialogueNode> AllNodes = new Dictionary<Node,DialogueNode>();

                    //Get all nodes & form Dialogue Nodes from them
                    foreach (var node in nodes)
                    {
                        DialogueNode dialogueNode = new DialogueNode(node.data);

                        if (!DialogueNodes.Contains(dialogueNode))
                        {
                            DialogueNodes.Add(dialogueNode);
                            AllNodes.Add(node, dialogueNode);
                        }
                    }

                    //Check all connections for this node acting as input
                    foreach (var connection in connections)
                    {
                        //Find Dialogue Node from list 
                        var inputNode = AllNodes.Single(n => n.Key == connection.Key.InputNode).Value;

                        var outputNode = AllNodes.Single(n => n.Key == connection.Key.OutputNode).Value;

                        inputNode.SetJumpTo(outputNode.Message.Id); 
                    }

                    //Create the asset file
                    if (DialogueNodes.Count > 0)
                    {
                        if (!string.IsNullOrEmpty(fileName))
                        {

                            CreateDialogueSO(fileName, DialogueNodes);

                            EditorUtility.DisplayDialog("SUCCESS", "Dialogue Asset created successfully in Assets folder", "OK");

                            fileName = string.Empty;
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("ERROR", "File Name cannot be empty", "OK");
                        }
                    }

                }
            }

            GUILayout.EndArea();
        }

        private void DrawGrid(float gridSpacing,float gridOpacity,Color gridColor)
        {
            // Get the dimensions of the current editor window
            Rect canvasRect = new Rect(0, 0, canvasSize.x, canvasSize.y);

            // Draw grid lines within the window dimensions
            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            // Vertical lines
            for (float x = canvasRect.xMin; x < canvasRect.xMax; x += gridSpacing)
            {
                Handles.DrawLine(new Vector3(x, canvasRect.yMin, 0), new Vector3(x, canvasRect.yMax, 0));
            }

            // Horizontal lines
            for (float y = canvasRect.yMin; y < canvasRect.yMax; y += gridSpacing)
            {
                Handles.DrawLine(new Vector3(canvasRect.xMin, y, 0), new Vector3(canvasRect.xMax, y, 0));
            }

            Handles.color = Color.white;
            Handles.EndGUI();
        }


        private void DrawDraggingConnection()
        {
            if (selectedNode != null && dragEndPosition.HasValue)
            {
                dragEndPosition = Event.current.mousePosition;
                Handles.color = Color.white;
                Handles.DrawLine(selectedNode.rect.center,dragEndPosition.Value);
                Handles.color = Color.white;
            }
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

        /// <summary>
        /// Method to draw all the connections that exists between the nodes in the list
        /// </summary>
        private void DrawConnections()
        {
            if(connections != null && connections.Count > 0)
            {
                foreach (var connectionKvp in connections)
                {
                    connectionKvp.Value.Draw();
                }
            }
        }

        #endregion

        #region CAMERA METHODS

        private void HandleZoom(Event e)
        {
            const float zoomMin = 0.2f;
            const float zoomMax = 2.0f;

            if (e.type == EventType.ScrollWheel)
            {
                float zoomDelta = -e.delta.y / 150.0f;
                float newZoom = Mathf.Clamp(zoomScale + zoomDelta, zoomMin, zoomMax);

                // Adjust panOffset to zoom towards mouse position
                Vector2 mouseWorldPos = (e.mousePosition - panOffset) / zoomScale;
                panOffset += (mouseWorldPos * (zoomScale - newZoom));

                zoomScale = newZoom;

                // Update canvas size based on zoom scale
                UpdateCanvasSize();

                e.Use(); // Mark event as handled
            }
        }

        private void HandlePanning(Event e)
        {
            if (e.type == EventType.MouseDown && e.button == 1) // Middle mouse button
            {
                dragStart = e.mousePosition;
            }

            if (e.type == EventType.MouseDrag && e.button == 1)
            {
                Vector2 delta = e.mousePosition - dragStart;
                scrollPosition -= delta;
                dragStart = e.mousePosition;
                e.Use(); // Mark event as handled
            }
        }


        private void ApplyZoomAndPan()
        {
            GUI.matrix = Matrix4x4.TRS(panOffset, Quaternion.identity, Vector3.one * zoomScale);
        }

        private Rect resizeHandleRect;
        private float handleSize = 10f; // Resize handle size

        private void DrawResizeHandle()
        {
            resizeHandleRect = new Rect(canvasSize.x - handleSize, canvasSize.y - handleSize, handleSize, handleSize);

            // Draw the resize handle as a small rectangle
            Handles.color = Color.yellow;
            Handles.DrawSolidRectangleWithOutline(resizeHandleRect, new Color(1f, 1f, 0f, 0.3f), Color.yellow);
        }

        private void HandleResize(Event e)
        {
            // Check if the mouse is over the resize handle and drag to resize
            if (e.type == EventType.MouseDown && e.button == 0 && resizeHandleRect.Contains(e.mousePosition))
            {
                isResizing = true;
                dragStart = e.mousePosition;
            }

            if (e.type == EventType.MouseDrag && isResizing)
            {
                // Calculate how much the user is dragging the resize handle
                Vector2 dragDelta = e.mousePosition - dragStart;

                // Increase the canvas width and height
                canvasSize.x = Mathf.Max(5000f, canvasSize.x + dragDelta.x);  // Ensure width doesn't go below 5000
                canvasSize.y = Mathf.Max(5000f, canvasSize.y + dragDelta.y); // Ensure height doesn't go below 5000

                // Update the resize handle position
                resizeHandleRect.x = canvasSize.x - handleSize;
                resizeHandleRect.y = canvasSize.y - handleSize;

                dragStart = e.mousePosition; // Update the start position for next drag
                e.Use();
            }

            if (e.type == EventType.MouseUp && isResizing)
            {
                isResizing = false;
            }
        }
        #endregion

        #region PROCESSING
        private void ProcessDragging(Event e)
        {
            if (e.type == EventType.MouseDrag && selectedNode != null)
            {
                dragEndPosition = e.mousePosition;
                Repaint();
            }
            else if (e.type == EventType.MouseUp)
            {
                dragEndPosition = null;
                Repaint();
            }
        }

        private void ProcessNodeEvents(Event e)
        {
            if(nodes != null && nodes.Count > 0)
            {
                for(int i = nodes.Count - 1; i >= 0; i--)
                {
                    //if (nodes[i].ProcessEvents(e))
                    //{
                    //    GUI.changed = true;
                    //}

                    bool guiChanged = nodes[i].ProcessEvents(e);

                    if(guiChanged && e.type == EventType.MouseDown && e.button == 0)
                    {
                        //Left mouse click

                        //Check if lands on a node
                        if (nodes[i].rect.Contains(e.mousePosition))
                        {
                            //Check if the state is currently dragging
                            if (!isDraggingConnection)
                            {
                                selectedNode = nodes[i];
                                isDraggingConnection = true;
                                dragStartPosition = selectedNode.rect.center;

                                Debug.Log("Dragging started");
                            }
                        }
                    }
                }
            }
        }


        private void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.KeyDown:

                    if(e.keyCode == KeyCode.LeftAlt)
                    {
                        Debug.Log("Left alt is pressed");
                    }

                    break;

                case EventType.MouseDown:

                    selectedNode = CheckForHitsOnNode(e.mousePosition);

                    //Handle Middle Mouse Button
                    ProcessMiddleMouseDown(e);

                    break;

                case EventType.MouseUp:

                    if (isDraggingConnection)
                    {
                        Node targetNode = GetNodeUnderMouse(e.mousePosition);

                        if (targetNode != null && targetNode != selectedNode)
                        {
                            Connection newConnection = new Connection(selectedNode, targetNode);

                            ConnectionTuple ct = new ConnectionTuple(selectedNode, targetNode);

                            if (!connections.ContainsKey(ct))
                            {
                                Debug.Log("Can make new connection");
                                //Finalize and make the connection
                                connections.Add(ct,newConnection);
                            }
                            else
                            {
                                Debug.LogError("Connection already exists");
                            }
                            
                        }
                    }

                    ResetDrag();
                    break;
            }
        }

        private void ProcessMiddleMouseDown(Event e)
        {
            //For middle mouse button
            if (e.button == 2)
            {
                if (selectedNode != null)
                {
                    ShowNodeContextMenu(e.mousePosition);
                }
                else
                {
                    ShowContextMenu(e.mousePosition);
                }
            }
        }

        #endregion

        #region HELPER FUNCTION REGION
        private Node GetNodeUnderMouse(Vector2 mousePosition)
        {
            foreach (Node node in nodes)
            {
                if (node.rect.Contains(mousePosition))
                {
                    return node;
                }
            }
            return null;
        }

        private Node CheckForHitsOnNode(Vector2 mousePosition)
        {
            foreach (var node in nodes)
            {
                if (node.IsSelected(mousePosition))
                {
                    return node;
                }
            }

            return null;
        }

        private void ResetDrag()
        {
            selectedNode = null;
            isDraggingConnection = false;
            dragStartPosition = Vector2.zero;
            dragEndPosition = null;
        }

        #endregion


        #region IN TOOL FUNCTIONS REGION

        private void ShowContextMenu(Vector2 position)
        {
            Matrix4x4 m4 = GUI.matrix;
            // scale it by your zoom
            GUI.matrix = GUI.matrix * Matrix4x4.Scale(new Vector3(1 / zoomScale, 1 / zoomScale, 1 / zoomScale));

            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Add Node"), false, () => OnClickAddNode(position));
            menu.AddItem(new GUIContent("Reset Zoom"),false, ()=>OnClickResetZoom(position));


            menu.ShowAsContext();

            // restore matrix
            GUI.matrix = m4;
        }

        private void ShowNodeContextMenu(Vector2 position)
        {
            GenericMenu menu = new GenericMenu();

            menu.AddItem(new GUIContent("Delete Node"), false, () => OnClickDeleteNode(position));
            

            menu.ShowAsContext();
        }

        private void OnClickResetZoom(Vector2 clickPosition)
        {
            zoom = 1f;
            zoomScale = 1f;
            panOffset = Vector2.zero;
        }

        private void OnClickAddNode(Vector2 clickPosition)
        {
            nodes.Add(new Node(clickPosition));
        }

        private void OnClickDeleteNode(Vector2 clickPosition)
        {
            if(selectedNode != null)
            {
                var keysToRemove = connections.Keys.Where(ct => ct.InputNode == selectedNode || ct.OutputNode == selectedNode).ToList();

                foreach (var key in keysToRemove)
                {
                    connections.Remove(key);
                }

                nodes.Remove(selectedNode);
                selectedNode = null;
                GUI.changed = true;
            }
        }
        #endregion


        #region SCRIPTABLE OBJECT SECTION
        public void CreateDialogueSO(string _fileName,List<DialogueNode> nodes)
        {
            //Create Instance
            DialogueDataSO dialogueDataSO = ScriptableObject.CreateInstance<DialogueDataSO>();

            //Assign values needed
            dialogueDataSO.SetNodes(nodes);

            //Save the asset
            string path = $"Assets/{_fileName}.asset";
            UnityEditor.AssetDatabase.CreateAsset(dialogueDataSO, path);
            UnityEditor.AssetDatabase.SaveAssets();

            Debug.Log("Dialogue SO created");
        }
        #endregion
    }
}
