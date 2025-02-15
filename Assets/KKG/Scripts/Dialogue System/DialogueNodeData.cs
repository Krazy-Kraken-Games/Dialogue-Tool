
using System.Collections.Generic;

namespace KKG.Dialogue
{
    public enum MessageType
    {
        DEFAULT = 0,
        ENDER = 1
    }

    [System.Serializable]
    public class DialogueNodeData
    {
        [ReadOnly]
        public string Id;
        public MessageType Type;
        public string SpeakerName;
        public string Message;

        public Dictionary<string,DialogueOption> Options = new Dictionary<string, DialogueOption>();

        [ReadOnly]
        //ID is generated at runtime automatically, so next jump would be a string
        public string? nextIndex;
    }
}
