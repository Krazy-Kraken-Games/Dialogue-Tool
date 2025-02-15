using KKG.Tool.Dialogue;
using System.Collections.Generic;
using UnityEngine;

namespace KKG.Dialogue
{
    [System.Serializable]
    public class ConnectionPacket
    {
        public ConnectionTuple ConnectionTuple;
        public Connection Connection;

        public ConnectionPacket(ConnectionTuple connectionTuple, Connection connection)
        {
            ConnectionTuple = connectionTuple;
            Connection = connection;
        }
    }

    /// <summary>
    /// The scriptable object structure to hold the information of nodes, connections and options
    /// in the data field
    /// </summary>
    public class DialogueToolTreeSO : ScriptableObject
    {
        [SerializeField]
        private List<NodeData> nodes = new List<NodeData>();
        private Dictionary<ConnectionTuple, Connection> connections = new Dictionary<ConnectionTuple, Connection>();
        private Dictionary<ConnectionOptionTuple, Connection> connectionOptions = new Dictionary<ConnectionOptionTuple, Connection>();

        [SerializeField]
        private NodeData startingNodeData;

        //[SerializeField]
        //private DialogueToolTreeData treeData = new DialogueToolTreeData();

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


            
        }

        public List<NodeData> Nodes => nodes;

        public Dictionary<ConnectionTuple, Connection> Connections => connections;

        public Dictionary<ConnectionOptionTuple, Connection> ConnectionOptions => connectionOptions;

    }
}
