
using System.Collections.Generic;
using UnityEngine;

namespace KKG.Dialogue
{
    [System.Serializable]
    public class DialogueNode
    {
        [SerializeField]
        private DialogueNodeData data;

        public DialogueNodeData Data => data;


        #region CONSTRUCTORS
        /// <summary>
        /// Dialogue Node constructor for Dialogue Tool
        /// </summary>
        /// <param name="_data"></param>
        public DialogueNode(DialogueNodeData _data)
        {
           data = _data;
        }

        /// <summary>
        /// Dialogue Node constructor for Excel Sheet (.csv) file
        /// </summary>
        /// <param name="rowData"></param>
        public DialogueNode(string[] rowData)
        {
            int maxCount = rowData.Length;

            //First line is ID
            string ID = rowData[0];

            //Second line is the Speaker's Name
            string speakerName = rowData[1];

            //Third column is Message
            string message = rowData[2];

            //Jump index
            //When using jumpIndex, you can check if it has a value using .HasValue or retrieve it using .Value.
            string? jumpIndex = null;
            if (maxCount > 3 && !string.IsNullOrEmpty(rowData[3]))
            {
                jumpIndex = rowData[3];
            }

            List<DialogueOption> options = new List<DialogueOption>();
            
            //Dialogue Options start from index 4
            for(int i = 4; i < maxCount; i+=2)
            {
                if(i + 1< maxCount && !string.IsNullOrEmpty(rowData[i]) && !string.IsNullOrEmpty(rowData[i+1]))
                {
                    DialogueOption dialogOption = new DialogueOption()
                    {
                        OptionMessage = rowData[i],
                        nextIndex = int.Parse(rowData[i + 1])
                    };
                    options.Add(dialogOption);
                }
                else
                {
                    Debug.LogWarning($"Dialogue Options seem to be corrupted");
                }
            }

            data = new DialogueNodeData()
            {
                Id = ID,
                SpeakerName = speakerName,
                Message = message,
                jumpIndex = jumpIndex,

                Options = options
            };
        }

        #endregion


        public void SetJumpTo(string jumpTo)
        {
            data.jumpIndex = jumpTo;   
        }
    }
}