using UnityEngine;
using System.Collections.Generic;

namespace KKG.Dialogue
{
    public class DialogueDataSO : ScriptableObject
    {
        [SerializeField]
        private List<DialogueNode> nodes = new List<DialogueNode>();

        public List<DialogueNode> Nodes { get { return nodes; } }

        public int Count;

        [ReadOnly]
        public string StartingNodeId;

        public void SetNodes(List<DialogueNode> nodes)
        {
            this.nodes = nodes;  

            Count = nodes.Count;
        }

        public void SetStartingNode(string _startingNodeId)
        {
            StartingNodeId = _startingNodeId;
        }

    }
}
