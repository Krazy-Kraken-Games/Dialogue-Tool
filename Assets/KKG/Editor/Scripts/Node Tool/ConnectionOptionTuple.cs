using KKG.Dialogue;

namespace KKG.Tool.Dialogue
{
    public class ConnectionOptionTuple
    {
        public DialogueOption InputOption;
        public DialogueTreeNode OutputNode;

        public ConnectionOptionTuple(DialogueOption inputOption, DialogueTreeNode outputNode)
        {
            InputOption = inputOption;
            OutputNode = outputNode;
        }
    }
}
