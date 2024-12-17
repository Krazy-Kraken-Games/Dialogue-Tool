using UnityEngine;
using System.Collections.Generic;

namespace KKG.Dialogue
{
    public class DialogueDataSO : ScriptableObject
    {
        public List<DialogueNode> Nodes = new List<DialogueNode>();
    }
}
