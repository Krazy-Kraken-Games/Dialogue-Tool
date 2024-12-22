
using System.Collections.Generic;

namespace KKG.Dialogue
{
    public enum MessageType
    {
        DEFAULT = 0,
        ENDER = 1
    }

    [System.Serializable]
    public struct DialogueNodeData
    {
        public int Id;
        public MessageType Type;
        public string SpeakerName;
        public string Message;

        public List<DialogueOption> Options;

        //Subject to change
        public int? jumpIndex;
    }
}
