namespace KKG.Tool.Dialogue
{
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
