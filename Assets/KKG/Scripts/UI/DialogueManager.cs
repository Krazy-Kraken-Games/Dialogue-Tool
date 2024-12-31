using UnityEngine;

namespace KKG.Dialogue
{
    public class DialogueManager : MonoBehaviour
    {
        public static DialogueManager Instance = null;

        [SerializeField]
        private bool isDialoguePlaying = false;

        //Local reference to active dialogue

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
        public void SetActiveDialogue()
        {

        }

        /// <summary>
        /// Reset Reference
        /// </summary>
        public void ResetActiveDialogue()
        {

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
    }
}
