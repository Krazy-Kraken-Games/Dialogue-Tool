using System;
using System.Collections.Generic;
using System.IO;
using KKG.Tool.Dialogue;
using TreeEditor;
using UnityEngine;

namespace KKG.Dialogue
{

    /// <summary>
    /// This class will represent how a node data is stored in backend
    /// </summary>
    [Serializable]
    public class NodeData
    {
        public Vector2 Position;
        public Vector2 Size;

        public DialogueNodeData data;

        public NodeData(DialogueTreeNode _node)
        {
            Rect rect = _node.rect;

            Position = rect.center;
            Size = rect.size;

            data = _node.data;
        }
    }



    [Serializable]
    public class ConnectionData
    {
        public string fromNodeID;
        public string toNodeID;
    }

    [Serializable]
    public class ConnectionOptionData
    {
        public string fromNodeID;
        public string toNodeID;
        public string optionText;
    }


    /// <summary>
    /// The scriptable object structure to hold the information of nodes, connections and options
    /// in the data field
    /// </summary>
    public class DialogueToolTreeSO : ScriptableObject
    {
        [SerializeField]
        private List<NodeData> nodes;
        private Dictionary<ConnectionTuple, Connection> connections;
        private Dictionary<ConnectionOptionTuple, Connection> connectionOptions;

        [SerializeField]
        private NodeData startingNodeData;

        [SerializeField]
        private DialogueToolTreeData treeData = new DialogueToolTreeData();

        public void SaveToolData(string path,DialogueTreeNode _start,List<DialogueTreeNode> _nodes,
            Dictionary<ConnectionTuple, Connection> _connections, Dictionary<ConnectionOptionTuple, Connection> _connectionOptions)
        {
            nodes = new List<NodeData>();

            //Convert to nodes
            foreach(var _node in _nodes)
            {
                NodeData node = new NodeData(_node);
                nodes.Add(node);
            }

            //Add reference to the starting node
            //startingNodeData = new NodeData(_start);

            //Save the connections and connections Options
            connections = new Dictionary<ConnectionTuple, Connection>();

            foreach(var connection in _connections)
            {
                connections.Add(connection.Key, connection.Value);
            }

            //Save the connection options
            connectionOptions = new Dictionary<ConnectionOptionTuple, Connection>();
            foreach(var connectionOption in _connectionOptions)
            {
                connectionOptions.Add(connectionOption.Key, connectionOption.Value);
            }

            //Create JSON File
            DialogueJSONGetter fileGetter = new DialogueJSONGetter(nodes, connections, connectionOptions);


            
        }

        public List<NodeData> Nodes => treeData.nodes;

        public Dictionary<ConnectionTuple, Connection> Connections => treeData.connections;

        public Dictionary<ConnectionOptionTuple, Connection> ConnectionOptions => treeData.connectionOptions;

    }
}
