using UnityEngine;
using System.Collections.Generic;

namespace KKG.Dialogue
{
    public class DialogueDataSO : ScriptableObject
    {
        [SerializeField]
        private List<DialogueNode> Nodes = new List<DialogueNode>();

        public void AddNode(DialogueNode node)
        {
            if (Nodes.Contains(node)) return;

            Nodes.Add(node);
        }

        public void SetNodes(List<DialogueNode> nodes)
        {
            Nodes = nodes;  
        }

    }
}
