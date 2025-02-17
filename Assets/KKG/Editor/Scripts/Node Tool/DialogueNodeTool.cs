using KKG.Dialogue;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KKG.Tool.Dialogue
{

    public class DialogueNodeTool : EditorWindow
    {

        [SerializeField]
        private string fileName = string.Empty;

        private List<DialogueTreeNode> nodes;
        private Dictionary<ConnectionTuple, Connection> connections;
        private Dictionary<ConnectionOptionTuple, Connection> connectionOptions;

        private DialogueTreeNode startingNode;
        private DialogueTreeNode selectedNode;

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

        private DialogueTreeNode draggingOutputNode;
        private string draggingOptionId;
        private Vector2 draggingStartPosition;
        private Rect draggingRect;
        private Vector2 draggingCurrentPosition;
        private bool isDraggingOutputConnection;
        private DialogueOption currentDialogueOption;


        [MenuItem("Tools/Krazy Kraken Games/Dialogue Graph")]
        public static void OpenWindow()
        {
            DialogueNodeTool window = GetWindow<DialogueNodeTool>();
            window.titleContent = new GUIContent("Dialogue Graph");
        }

        private void OnEnable()
        {
            nodes = new List<DialogueTreeNode>();
            connections = new Dictionary<ConnectionTuple, Connection>();
            connectionOptions = new Dictionary<ConnectionOptionTuple, Connection>();
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


            //Draw any nodes that exist
            DrawNodes();
            DrawConnections();

            //Events Handling
            ProcessDragging(Event.current);
            ProcessNodeEvents(Event.current);
            ProcessEvents(Event.current);

            if(isDraggingConnection || isDraggingOutputConnection)
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

        private async void DrawInspectorPanel(Rect rect)
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

            if (startingNode != null)
            {
                GUILayout.Label($"Starting Node: {startingNode.data.Id}");
            }
            else
            {
                GUILayout.Label("No Starting Node");
            }


            if (nodes.Count > 0)
            {
                //Button press to create Dialogue asset
                if (GUILayout.Button("Create Dialogue Asset"))
                {

                    List<DialogueNode> DialogueNodes = new List<DialogueNode>();
                    Dictionary<DialogueTreeNode,DialogueNode> AllNodes = new Dictionary<DialogueTreeNode,DialogueNode>();

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

                            CreateDialogueTreeSO(fileName);

                            CreateDialogueJSON(fileName);

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

            //Load from save file
            if (!string.IsNullOrEmpty(fileName))
            {
                //Get file name + SO
                if(GUILayout.Button("Load from Record"))
                {
                    LoadDialogueTreeSO(fileName);
                }
            }

            //Clear everything button
            if (nodes.Count > 0)
            {
                if(GUILayout.Button("Clear Graph"))
                {
                    nodes.Clear();
                    connectionOptions.Clear();
                    connections.Clear();
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
            if (isDraggingOutputConnection)
            {
                draggingCurrentPosition = Event.current.mousePosition;
                Handles.color = Color.red;
                Handles.DrawLine(draggingStartPosition, draggingCurrentPosition);
                Repaint();
            }
            else
            {
                if (selectedNode != null && dragEndPosition.HasValue)
                {
                    dragEndPosition = Event.current.mousePosition;
                    Handles.color = Color.white;
                    Handles.DrawLine(selectedNode.outputNodeRect.center, dragEndPosition.Value);
                    Handles.color = Color.white;
                }
            }
        }

        /// <summary>
        /// Method to draw all the nodes that exist in the list
        /// </summary>
        private void DrawNodes()
        {
            if (nodes != null && nodes.Count > 0)
            {
                foreach(DialogueTreeNode node in nodes)
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

            if(connectionOptions != null && connectionOptions.Count > 0)
            {
                foreach(var connectionOptionKvp in connectionOptions)
                {
                    connectionOptionKvp.Value.Draw();
                }
            }
        }

        public static void StartDraggingConnection(DialogueTreeNode node, string optionId, Vector2 startPosition,Rect rect,DialogueOption _option)
        {
            DialogueNodeTool instance = GetWindow<DialogueNodeTool>();
            instance.draggingOutputNode = node;
            instance.draggingOptionId = optionId;
            instance.draggingStartPosition = startPosition;
            instance.isDraggingOutputConnection = true;
            instance.draggingRect = rect;
            instance.currentDialogueOption = _option;

            Debug.Log($"Tool found, drag output node: {instance.draggingOutputNode.data.Id}");
        }

        private void ResetDraggingConnection()
        {
            draggingOutputNode = null;
            draggingOptionId = null;
            draggingStartPosition = Vector2.zero;
            draggingCurrentPosition = Vector2.zero;
            isDraggingOutputConnection = false;

            currentDialogueOption = new DialogueOption();
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

                                if (selectedNode.OptionCount == 0)
                                {
                                    isDraggingConnection = true;
                                    dragStartPosition = selectedNode.rect.center;
                                }
                                else
                                {
                                    isDraggingConnection = false;
                                }
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
                        //Debug.Log("Left alt is pressed");
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
                        DialogueTreeNode targetNode = GetNodeUnderMouse(e.mousePosition);

                        if (targetNode != null && targetNode != selectedNode)
                        {
                            Connection newConnection = new Connection(selectedNode, targetNode);

                            ConnectionTuple ct = new ConnectionTuple(selectedNode, targetNode);

                            //Check if option already connected to something else

                            var existing = connections.Where(opt =>
                            string.Equals(opt.Key.InputNode.data.Id, selectedNode.data.Id)).ToList();

                            if (existing.Count > 0)
                            {
                                //Remove the all the current ones
                                Debug.Log("Connection input already exists");

                                foreach (var option in existing)
                                {
                                    connections.Remove(option.Key);
                                }
                            }

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

                    if (isDraggingOutputConnection)
                    {
                        DialogueTreeNode targetNode = GetNodeUnderMouse(Event.current.mousePosition);

                        if (targetNode != null && draggingOutputNode != null && draggingOutputNode != targetNode)
                        {
                            Connection newConnection = new Connection(draggingOutputNode, targetNode,true);

                            //ConnectionTuple ct = new ConnectionTuple(draggingOutputNode, targetNode);

                            ConnectionOptionTuple cot = new ConnectionOptionTuple(currentDialogueOption, targetNode);

                            //Check if option already connected to something else

                            var existing = connectionOptions.Where(opt => 
                            string.Equals(opt.Key.InputOption.OptionId, currentDialogueOption.OptionId)).ToList();

                            if(existing.Count > 0)
                            {
                                //Remove the all the current ones
                                Debug.Log("Option connection already exists");

                                foreach(var option in existing)
                                {
                                    connectionOptions.Remove(option.Key);
                                }
                            }


                            if (!connectionOptions.ContainsKey(cot))
                            {
                                connectionOptions.Add(cot, newConnection);
                                Debug.Log($"Connected output '{draggingOptionId}' to node '{targetNode}'");
                                newConnection.SetFromOutputNode(draggingRect);

                                draggingOutputNode.AddNextIndexOnTool(draggingOptionId, targetNode.data.Id);
                                Debug.Log("Option next index updated");
                            }
                            else
                            {
                                Debug.LogError("Connection already exists");
                            }
                        }
                    }
                        ResetDrag();

                    ResetDraggingConnection();
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
        private DialogueTreeNode GetNodeUnderMouse(Vector2 mousePosition)
        {
            foreach (DialogueTreeNode node in nodes)
            {
                if (node.rect.Contains(mousePosition))
                {
                    return node;
                }
            }
            return null;
        }

        private DialogueTreeNode CheckForHitsOnNode(Vector2 mousePosition)
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
            menu.AddItem(new GUIContent("Make Starting Node"), false, () => OnClickStartNode(position));

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
            DialogueTreeNode newNode = new DialogueTreeNode(clickPosition);

            if(nodes.Count == 0)
            {
                SetStartingNode(newNode);
            }

            nodes.Add(newNode);
        }

        private void OnClickStartNode(Vector2 clickPosition)
        {
            var node = GetNodeUnderMouse(clickPosition);

            if (node != null)
            {
                SetStartingNode(node);
            }
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

                //Check if there are any options and remove them

                var options = selectedNode.data.Options;

                foreach (var opt in options)
                {
                    var optionKeysToRemove = connectionOptions.Keys.Where(ct => ct.InputOption.OptionId == opt.OptionID || ct.OutputNode == selectedNode).ToList();

                    foreach (var key in optionKeysToRemove)
                    {
                        connectionOptions.Remove(key);
                    }
                }

                //Remove if output node is connected with dialogue options
                var outputConnectedToOptions = connectionOptions.Keys.Where(ct => ct.OutputNode == selectedNode).ToList();

                foreach (var key in outputConnectedToOptions)
                {
                    connectionOptions.Remove(key);
                }

                nodes.Remove(selectedNode);

                //Check if we are deleting the starting node
                if (selectedNode == startingNode)
                {
                    startingNode.SetNormalNode();
                    startingNode = null;

                    if (nodes.Count > 0)
                    {
                        //The first node we find, will be set as starting node automatically
                        SetStartingNode(nodes.First());
                    }
                }

                selectedNode = null;
                GUI.changed = true;
            }
        }

        public void RemovingOptionConnectionWithID(string optionID)
        {
            Debug.Log($"Removing with Option: {optionID}");

            var optionKeysToRemove = connectionOptions.Keys.Where(ct => ct.InputOption.OptionId == optionID).ToList();

            foreach (var key in optionKeysToRemove)
            {
                connectionOptions.Remove(key);
            }
        }
        #endregion

        #region CREATE JSON FILE SECTION
        public void CreateDialogueJSON(string _fileName)
        {
            //string fullFileName = $"{_fileName}.json";


            //if (dialogueGetter == null)
            //{
            //    var nodeDataList = new List<NodeData>();

            //    foreach (var node in nodes)
            //    {
            //        nodeDataList.Add(new NodeData(node));
            //    }

            //    dialogueGetter = new DialogueJSONGetter(nodeDataList, connections, connectionOptions);
            //}
            //else
            //{

            //    var nodeDataList = new List<NodeData>();

            //    foreach (var node in nodes)
            //    {
            //        nodeDataList.Add(new NodeData(node));
            //    }

            //    dialogueGetter.UpdateData(nodeDataList, connections, connectionOptions);
            //}


            //string JsonPath = $"Assets/Resources/Content/{fullFileName}.json";
            //dialogueGetter.SaveData(JsonPath);

            //Debug.Log("JSON File created successfully");
        }
        #endregion


        #region SCRIPTABLE OBJECT SECTION
        public void CreateDialogueSO(string _fileName,List<DialogueNode> nodes)
        {
            //Save the asset
            string fullFileName = $"{_fileName}SO";
            string path = $"Assets/{fullFileName}.asset";

            // Check if the asset exists
            DialogueDataSO existingAsset = AssetDatabase.LoadAssetAtPath<DialogueDataSO>(path);

            if (existingAsset != null)
            {
                // Asset exists, update its data
                existingAsset.SetNodes(nodes);
                EditorUtility.SetDirty(existingAsset); // Mark the asset as dirty for saving
                Debug.Log($"Existing DialogueToolTreeSO updated at: {path}");
            }
            else
            {
                // Asset doesn't exist, create a new one
                DialogueDataSO dialogueDataSO = CreateInstance<DialogueDataSO>();
                dialogueDataSO.SetNodes(nodes);
                AssetDatabase.CreateAsset(dialogueDataSO, path);
                Debug.Log($"New DialogueToolTreeSO created at: {path}");
            }
            AssetDatabase.SaveAssets();
        }

        private void CreateDialogueTreeSO(string _fileName)
        {
            string fullFileName = $"{_fileName}_TreeSO";
            string path = $"Assets/{fullFileName}.asset";

            // Check if the asset exists
            DialogueToolTreeSO existingAsset = AssetDatabase.LoadAssetAtPath<DialogueToolTreeSO>(path);

            if (existingAsset != null)
            {
                // Asset exists, update its data
                existingAsset.SaveToolData(path, startingNode, nodes, connections, connectionOptions);
                EditorUtility.SetDirty(existingAsset); // Mark the asset as dirty for saving
                Debug.Log($"Existing DialogueToolTreeSO updated at: {path}");
            }
            else
            {
                // Asset doesn't exist, create a new one
                DialogueToolTreeSO dialogueDataSO = CreateInstance<DialogueToolTreeSO>();
                dialogueDataSO.SaveToolData(path, startingNode, nodes, connections, connectionOptions);
                AssetDatabase.CreateAsset(dialogueDataSO, path);
                Debug.Log($"New DialogueToolTreeSO created at: {path}");
            }
            AssetDatabase.SaveAssets();

        }



        private DialogueJSONGetter dialogueGetter = null;

        private void LoadDialogueTreeSO(string _fileName)
        {

            if(dialogueGetter == null)
            {
                dialogueGetter = new DialogueJSONGetter();
            }

            string fullFileName = $"{_fileName}_TreeSO";

            // Construct the asset path
            string path = $"Assets/{fullFileName}.asset";

            // Load the asset
            DialogueToolTreeSO dialogueToolTreeSO = AssetDatabase.LoadAssetAtPath<DialogueToolTreeSO>(path);



            if (dialogueToolTreeSO == null)
            {
                Debug.LogError($"DialogueTreeSO not found at path: {dialogueToolTreeSO}");
                return;
            }

            // Clear existing nodes (if necessary)
            nodes.Clear();
            connections.Clear();
            connectionOptions.Clear();

            List<DialogueOption> allOptions = new List<DialogueOption>();

            if (dialogueToolTreeSO.Nodes.Count > 0)
            {
                // Load data from the asset and recreate nodes
                foreach (var nodeData in dialogueToolTreeSO.Nodes)
                {
                    var pos = nodeData.Position;
                    var size = nodeData.Size;

                    Vector2 Position = new Vector2(pos.x, pos.y);
                    Vector2 Size = new Vector2(size.x, size.y);

                    var data = nodeData.data;

                    DialogueTreeNode newNode = new DialogueTreeNode(Position, Size.x, Size.y,false,false);

                    newNode.data.Id = data.Id;

                    //Manually toggle the button to show options if options exist
                    if(nodeData.data.Options.Count > 0)
                    {
                        newNode.SetShowOptions(true);
                    }

                    // Add the recreated node to the list
                    nodes.Add(newNode);
                }

                foreach(var node in dialogueToolTreeSO.Nodes)
                {
                    var data = node.data;

                    var existingNode = nodes.First(x => x.data.Id == data.Id);

                    if(existingNode != null)
                    {
                        existingNode.data = data;

                        if (existingNode.data.Options != null && existingNode.data.Options.Count > 0)
                        {

                            foreach (var option in existingNode.data.Options)
                            {
                               var createdOption =  existingNode.CreateOptionWithData(option);
                               allOptions.Add(createdOption.Option);
                            }
                        }

                        existingNode.AllowDrawingNode();
                    }
                }

                //Load the connections and connection Options

                //Handle first the node to node connections
                if(dialogueToolTreeSO.Connections.Count > 0 && dialogueToolTreeSO.Connections != null)
                {
                    foreach(var connection in dialogueToolTreeSO.Connections)
                    {
                        //Break the connection and its tuple to get info about start and end node

                        var InputKey = connection.ConnectionTuple.InputNode;
                        var OutputKey = connection.ConnectionTuple.OutputNode;

                        //Find the input & output nodes in the existing/newly created dialogue nodes
                        var InputNode = nodes.First(node => node.data.Id.Equals(InputKey.data.Id));
                        var OutputNode = nodes.First(node => node.data.Id.Equals(OutputKey.data.Id));

                        ConnectionTuple ct = new ConnectionTuple(InputNode,OutputNode);
                        Connection conn = new Connection(InputNode, OutputNode);

                        connections.Add(ct, conn);
                    }
                }

                //Handle the connection options
                if(dialogueToolTreeSO.ConnectionOptions != null && dialogueToolTreeSO.ConnectionOptions.Count > 0)
                {
                    foreach(var connectionOpt in dialogueToolTreeSO.ConnectionOptions)
                    {
                        var InputKey = connectionOpt.ConnectionOptionTuple.InputOption;
                        var OutputKey = connectionOpt.ConnectionOptionTuple.OutputNode;

                        //Find the input & output nodes in the existing/newly created dialogue nodes
                        var InputNode = nodes.First(node => node.data.Id.Equals(InputKey.parentNodeRef));
                        var InputOption = allOptions.First(node => node.OptionId.Equals(InputKey.OptionId));
                        var OutputNode = nodes.First(node => node.data.Id.Equals(OutputKey.data.Id));

                        Connection newConnection = new Connection(InputNode, OutputNode, true);
                        ConnectionOptionTuple cot = new ConnectionOptionTuple(InputOption, OutputNode);

                        connectionOptions.Add(cot, newConnection);

                        //Get Output rect from Dialogue Tree Node

                        var nodePacket = InputNode.data.Options.Single(opt => opt.OptionID == InputKey.OptionId); 
                        newConnection.SetFromOutputNode(nodePacket.rect);
                    }
                }

                //Set the starting node to proper mode
                if (!string.IsNullOrEmpty(dialogueToolTreeSO.StartNodeId))
                {
                    var startingNode = nodes.Single(node => node.data.Id == dialogueToolTreeSO.StartNodeId);

                    startingNode.SetStartingNode();
                }

                Debug.Log("Dialogue tree loaded successfully!");
            }
            else
            {
                Debug.LogError("Dialogue tree missing nodes");
            }
        }

        #endregion


        #region MISC FUNCTION SECTION

        public void RemoveFromConnection(string inputID)
        {
            var relevantConnections = connections.Where(node => string.Equals(node.Key.InputNode.data.Id,inputID)).ToList();
            
            if (relevantConnections.Count == 0) return;

            foreach(var connection in relevantConnections)
            {
                connections.Remove(connection.Key);
            }
        }

        #endregion

        private void SetStartingNode(DialogueTreeNode _node)
        {
            if(startingNode != null)
            {
                startingNode.SetNormalNode();
                startingNode = null;
            }

            startingNode = _node;
            startingNode.SetStartingNode();
        }


    }
}
