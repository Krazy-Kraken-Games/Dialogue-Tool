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

        [SerializeField]
        private DialogueOptionPanel OptionPanelUI;

        
        [SerializeField]
        private GameObject NextButton;

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


            if (_data.Options.Count > 0)
            {
                //Options exists, have dialogue manager populate options
                OptionPanelUI.PopulateDialogOptions(_data.Options);
                NextButton.SetActive(false);
            }
            else
            {
                //There are no options present. Have dialogue manager hide options
                OptionPanelUI.HideAllOptions();
                NextButton.SetActive(true);
            }
        }
    }
}
