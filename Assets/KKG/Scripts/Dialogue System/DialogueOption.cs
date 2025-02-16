using UnityEngine;

namespace KKG.Dialogue
{
    [System.Serializable]
    public class DialogueOptionPacket
    {
        public string OptionID;
        public DialogueOption Option;

        //Structural information about the Dialogue Option Node
        public Rect rect;

        public DialogueOptionPacket(string _optionID, DialogueOption _option,Rect _rect) 
        {
            OptionID = _optionID;
            Option = _option;
            rect = _rect;
        }

    }


    [System.Serializable]
    public class DialogueOption
    {
        //Id of the next message to fire from the tree
        [ReadOnly]
        public string OptionId;
        [ReadOnly]
        public string NextIndex;
        public string SpeakerName;
        public string OptionMessage;

        [ReadOnly]
        public string parentNodeRef;

    }
}