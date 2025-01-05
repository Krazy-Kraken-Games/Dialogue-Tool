
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KKG.Dialogue
{
    [System.Serializable]
    public class Dialogue
    {
        public Dictionary<string,DialogueNode> Messages;

        [SerializeField]
        private bool isRunning = false;

        [SerializeField]
        private DialogueNode activeMessage;

        [SerializeField]
        private string currentMessageId => activeMessage.Message.Id;

        public string CurrentMessageID => currentMessageId;

        public Action<DialogueNode> OnMessageUpdatedEvent;
        public Action OnDialogueEndReachedEvent;

        private bool isLastMessage => string.IsNullOrEmpty(activeMessage.Message.jumpIndex);

        public bool LastMessage => isLastMessage;


        #region CONSTRUCTORS
        public Dialogue(List<DialogueNode> nodes)
        {
            Messages = new Dictionary<string, DialogueNode>();

            foreach (var node in nodes)
            {
                Messages.Add(node.Message.Id, node);
            }
        }

        #endregion

        public void StartDialogue()
        {
            if (!isRunning)
            {
                //Get the first message of the list
                var firstNodeKvp = Messages.First();
                activeMessage = firstNodeKvp.Value;
                isRunning = true;

            }
        }

        public DialogueNode GetCurrentMessage()
        {
            return activeMessage;
        }

        private DialogueNode GetNextMessage()
        {
            if(isLastMessage)
            {
                //Last Message was shown
                return null;
            }
            else
            {
                //There still are more messages to show in the dialogue tree
                var nextMessageID = activeMessage.Message.jumpIndex;
                var nextMessage = Messages.Single(id => id.Key == nextMessageID).Value;

                return nextMessage;
            }
        }

        

        public void ShowNextMessage()
        {
            //Sets the next active message
            activeMessage = GetNextMessage();

            //Check if active Message is valid
            if (activeMessage == null)
            {
                isRunning = false;
                OnDialogueEndReachedEvent?.Invoke();
            }
            else
            {
                OnMessageUpdatedEvent?.Invoke(activeMessage);
            }
        }
    }
}