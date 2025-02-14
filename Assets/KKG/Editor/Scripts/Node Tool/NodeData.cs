using KKG.Tool.Dialogue;
using System;
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

        public NodeData()
        {
        }

        public NodeData(DialogueTreeNode _node)
        {
            Rect rect = _node.rect;

            Position = new Vector2(_node.PositionX, _node.PositionX);
            Size = new Vector2(_node.Width, _node.Height);

            data = _node.data;
        }
    }
}
