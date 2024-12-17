
using System.Collections.Generic;

namespace KKG.Dialogue
{
    public enum MessageType
    {
        DEFAULT = 0,
        ENDER = 1
    }

    public struct DialogueNode
    {
        public int Id;
        public MessageType Type;
        public string SpeakerName;
        public string Message;

        public List<DialogueOption> Options;

        //Subject to change
        public int jumpIndex;
    }
}
