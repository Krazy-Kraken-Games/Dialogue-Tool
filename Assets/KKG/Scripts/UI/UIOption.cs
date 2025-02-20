using KKG.Dialogue;
using TMPro;
using UnityEngine;

public class UIOption : MonoBehaviour
{
    [SerializeField]
    private DialogueOption option;

    [SerializeField]
    private TextMeshProUGUI MessageText;

    public void PopulateOption(DialogueOption _data)
    {
        option = _data;
        MessageText.text = option.OptionMessage;
    }

    public void OnOptionSelected()
    {
        string nextIndex = option.NextIndex;

        DialogueManager.Instance.OnOptionSelectedByID(nextIndex);
    }
}
