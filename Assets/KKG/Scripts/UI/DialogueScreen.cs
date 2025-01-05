using UnityEngine;

namespace KKG.Dialogue
{
    public class DialogueScreen : MonoBehaviour
    {
        [SerializeField]
        private GameObject screenContent;

        [SerializeField]
        private bool visible = true;

        [Space(10)]
        [Header("UI Dialogue Reference")]
        [SerializeField]
        private UIDialogue DialogueUI;

        private void Start()
        {
            Toggle();
        }

        public void Toggle()
        {
            visible = !visible; 

            screenContent.SetActive(visible);
        }

        public void PopulateMessage(DialogueNodeData _data)
        {
            if (!visible)
            {
                Toggle();
            }

            DialogueUI.PopulateMessage(_data);
        }
    }
}
