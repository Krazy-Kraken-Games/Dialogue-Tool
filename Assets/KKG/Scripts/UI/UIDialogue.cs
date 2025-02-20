using KKG.Dialogue;
using TMPro;
using UnityEngine;

public class UIDialogue : MonoBehaviour
{
    [SerializeField]
    private TextMeshProUGUI SpeakerNameText;

    [SerializeField]
    private TextMeshProUGUI MessageText;

    public void PopulateMessage(DialogueNodeData _data)
    {
        SpeakerNameText.text = _data.SpeakerName;
        MessageText.text = _data.Message;

    }
}
