namespace KKG.Tool.Dialogue
{
    [System.Serializable]
    public class ConnectionTuple
    {
        public DialogueTreeNode InputNode;
        public DialogueTreeNode OutputNode;

        public ConnectionTuple(DialogueTreeNode inputNode, DialogueTreeNode outputNode)
        {
            InputNode = inputNode;
            OutputNode = outputNode;
        }
    }
}
