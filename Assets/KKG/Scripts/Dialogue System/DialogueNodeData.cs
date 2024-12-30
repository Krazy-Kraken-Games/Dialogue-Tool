
using System.Collections.Generic;
using UnityEngine;

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
        [ReadOnly]
        public string Id;
        public MessageType Type;
        public string SpeakerName;
        public string Message;

        public List<DialogueOption> Options;

        [ReadOnly]
        //ID is generated at runtime automatically, so next jump would be a string
        public string? jumpIndex;
    }
}
