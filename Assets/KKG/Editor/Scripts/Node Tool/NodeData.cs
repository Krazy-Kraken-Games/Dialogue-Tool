using KKG.Tool.Dialogue;
using System.Collections.Generic;
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
        public NodeVector2 Position;
        public NodeVector2 Size;

        public DialogueNodeData data;

        public NodeData(DialogueTreeNode _node)
        {
            Rect rect = _node.rect;

            Position = new NodeVector2(rect.center.x, rect.center.y);
            Size = new NodeVector2(rect.size.x, rect.size.y);

            data = _node.data;
        }
    }
}
