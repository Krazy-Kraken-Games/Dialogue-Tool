using UnityEngine;

namespace KKG.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance = null;

        [SerializeField]
        private bool isDialoguePlaying = false;

        [Space(10)]
        [Header("Dialogue Data")]
        [SerializeField]
        private DialogueDataSO activeDialogueSO;

        //Local reference to active dialogue
        [Space(10)]
        [Header("Local Dialogue Reference")]
        [SerializeField]
        private Dialogue activeDialogue;

        [Space(10)]
        [Header("Dialogue Screen Reference")]
        [SerializeField]
        private DialogueScreen dialogueScreen;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            if(dialogueScreen == null)
            {
                Debug.LogError("Dialogue UI reference is missing. Init process discontinued");
                return;
            }

            Initialization();
        }

        /// <summary>
        /// Set Reference to Active Dialogue
        /// </summary>
        public void SetActiveDialogue(Dialogue _dialogue)
        {
            activeDialogue = _dialogue;
        }

        /// <summary>
        /// Reset Reference
        /// </summary>
        public void ResetActiveDialogue()
        {
            activeDialogue = null;
        }

        /// <summary>
        /// Method trigger to start the active dialogue
        /// </summary>
        public void StartDialogue()
        {
            //Check if the active dialogue is not null
            isDialoguePlaying = true;

            activeDialogue.StartDialogue();

            //Populate the UI with the same
            ShowCurrentMessage();
        }

        /// <summary>
        /// Stops the active dialogue from playing
        /// </summary>
        public void StopDialogue()
        {
            if (!isDialoguePlaying) return;

            isDialoguePlaying = false;
            ResetActiveDialogue();
        }


        #region DATA HANDLING SECTION

        public void SetDialogueData(DialogueDataSO data)
        {
            activeDialogueSO = data;

            //Fire the set active dialogue from here
            Dialogue dialogue = new Dialogue(activeDialogueSO.Nodes);

            //Set as active dialogue
            SetActiveDialogue(dialogue);
        }

        #endregion


        #region INITIALIZATION

        private void Initialization()
        {
            //Showcase dialogue data on start if SO exists
            if(activeDialogueSO != null)
            {
                //Make dialogue from it
                SetDialogueData(activeDialogueSO);
            }
        }

        #endregion

        #region UI SECTION

        public void ShowCurrentMessage()
        {
            if(dialogueScreen != null)
            {
                var message = activeDialogue.GetCurrentMessage().Message;
                dialogueScreen.PopulateMessage(message);
            }
        }

        #endregion

        #region INPUT HANDLER

        /// <summary>
        /// The interaction point between the player input & the dialogue system
        /// </summary>
        public void OnInteractionFromPlayer()
        {
            if (!isDialoguePlaying)
            {
                //Start Dialogue
                StartDialogue();
            }
            else
            {
                //Already playing, play next message or hide
                var nextMessage = activeDialogue.ShowNextMessage();

                if(nextMessage == null)
                {
                    Debug.Log("End dialogue reached");
                    isDialoguePlaying = false;
                    dialogueScreen.Toggle();
                }
                else
                {
                    ShowCurrentMessage();
                }
            }
        }

        #endregion

        #region Option Input Handling Secction

        public void OnOptionSelectedByID(string _nextMessageId)
        {
            if (activeDialogue == null) return;

            var nextMessage = activeDialogue.SetNextMessageById(_nextMessageId);


            if (nextMessage == null)
            {
                Debug.Log("End dialogue reached");
                isDialoguePlaying = false;
                dialogueScreen.Toggle();
            }
            else
            {
                ShowCurrentMessage();
            }
        }

        #endregion
    }
}
