using System;
using System.Collections.Generic;
using KKG.Tool.Dialogue;
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

        public void SaveToolData(List<DialogueTreeNode> _nodes)
        {
            nodes = new List<NodeData>();

            //Convert to nodes
            foreach(var _node in _nodes)
            {
                NodeData node = new NodeData(_node);
                nodes.Add(node);
            }
           
        }

        public List<NodeData> Nodes => nodes;   
    }
}
