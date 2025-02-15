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
        [SerializeField]
        private List<ConnectionPacket> connections = new List<ConnectionPacket>();
        private Dictionary<ConnectionOptionTuple, Connection> connectionOptions = new Dictionary<ConnectionOptionTuple, Connection>();

        [SerializeField]
        private string startingNodeId;

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
            startingNodeId = _start.data.Id;

            //Save the connections and connections Options
            connections = new List<ConnectionPacket>();

            foreach(var connection in _connections)
            {
                ConnectionPacket cp = new ConnectionPacket(connection.Key, connection.Value);
                connections.Add(cp);
            }

            //Save the connection options
            connectionOptions = new Dictionary<ConnectionOptionTuple, Connection>();
            foreach(var connectionOption in _connectionOptions)
            {
                connectionOptions.Add(connectionOption.Key, connectionOption.Value);
            }


            
        }

        public List<NodeData> Nodes => nodes;

        public List<ConnectionPacket> Connections => connections;

        public Dictionary<ConnectionOptionTuple, Connection> ConnectionOptions => connectionOptions;

    }
}
