using KKG.Dialogue;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace KKG.FileHandling
{
    public class KrakenFileReader : EditorWindow
    {
        [SerializeField]
        private FileLocator fileLocator;

        [SerializeField]
        private CSVReader fileReader;

        public static void OpenWindow()
        {

        }


        public void ReadFile()
        {
            //Open File Explorer Dialog in Editor to load the file path
            string filePath = fileLocator.SearchForFilePath();

            List<string[]> fileContent = new List<string[]>();

            //Load file content & parse data using the parser
            if (!string.IsNullOrEmpty(filePath))
            {
                fileContent = fileReader.LoadFileViaPath(filePath);
            }


            List<DialogueNode> DialogueNodes = new List<DialogueNode>();    
            //Create dialog nodes for each row in the file Content
            //Skip 0th element, as its the sheet column headers
            for(int i = 1; i < fileContent.Count; i++)
            {
                DialogueNode node = CreateDialogueNode(fileContent[i]);

                Debug.Log($"{node.Data.SpeakerName} : {node.Data.Message} and Has Options:{node.Data.Options.Count > 0}");

                DialogueNodes.Add(node);
            }

            if(DialogueNodes.Count > 0)
            {
                //Create the Scriptable Object Asset file
                CreateDialogueSO(DialogueNodes);
            }
        }

        public DialogueNode CreateDialogueNode(string[] rowData)
        {
            return new DialogueNode(rowData);    
        }

        public void CreateDialogueSO(List<DialogueNode> nodes)
        {
            //Create Instance
            DialogueDataSO dialogueDataSO = ScriptableObject.CreateInstance<DialogueDataSO>();

            //Assign values needed
            dialogueDataSO.SetNodes(nodes);

            //Save the asset
            string path = "Assets/TestDialogueSO.asset";
            UnityEditor.AssetDatabase.CreateAsset(dialogueDataSO, path);
            UnityEditor.AssetDatabase.SaveAssets(); 

            Debug.Log("Dialogue SO created");
        }
    }
}
