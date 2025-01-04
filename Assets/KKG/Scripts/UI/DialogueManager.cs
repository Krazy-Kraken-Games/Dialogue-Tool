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
        [Header("UI Dialogue Reference")]
        [SerializeField]
        private UIDialogue DialogueUI;

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

            SetActiveDialogue(dialogue);

            //Populate the UI with the same
        }

        #endregion
    }
}
