using KKG.Dialogue;
using System.Collections.Generic;
using UnityEngine;

public class DialogueOptionPanel : MonoBehaviour
{
    [SerializeField]
    private UIOption[] UIOptionsArray;

    private int optionIndex = 0;

    public void PopulateDialogOptions(List<DialogueOptionPacket> optionPackets)
    {
        optionIndex = 0;
        foreach (var optionPacket in optionPackets)
        {
            var UIOption = UIOptionsArray[optionIndex];

            UIOption.PopulateOption(optionPacket.Option);

            optionIndex++;
        }

        ShowOptions(optionPackets.Count);
    }

    public void HideAllOptions()
    {
        foreach(var optionUI in UIOptionsArray)
        {
            optionUI.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Show the first n number of options 
    /// where n is reflected by the count
    /// </summary>
    /// <param name="_count"></param>
    public void ShowOptions(int _count)
    {
        if(_count >= UIOptionsArray.Length)
        {
            Debug.LogError("Specified count is greater than UI length");
            return;
        }

        for(int i = 0; i < _count; i++)
        {
            UIOptionsArray[i].gameObject.SetActive(true);
        }
    }
}
