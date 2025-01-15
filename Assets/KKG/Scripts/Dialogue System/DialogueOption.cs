
namespace KKG.Dialogue
{
    [System.Serializable]
    public struct DialogueOption
    {
        //Id of the next message to fire from the tree
        [ReadOnly]
        public string OptionId;
        [ReadOnly]
        public string NextIndex;
        public string SpeakerName;
        public string OptionMessage;

    }
}