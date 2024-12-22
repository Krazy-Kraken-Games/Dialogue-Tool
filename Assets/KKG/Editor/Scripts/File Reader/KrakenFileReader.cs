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

        private string filePath;

        [MenuItem("Tools/Krazy Kraken Games/Dialogue File Reader")]
        public static void OpenWindow()
        {
            KrakenFileReader fileReader = GetWindow<KrakenFileReader>("Kraken Dialogue File Reader");
            fileReader.Show();
        }

        private void OnGUI()
        {
            GUILayout.Label("Kraken Dialogue Reader",EditorStyles.boldLabel);

            if (!string.IsNullOrEmpty(filePath))
            {
                GUILayout.Label($"Location:{filePath}");
            }

            //Browse Button
            if(GUILayout.Button("Browse File"))
            {
                if(fileLocator == null)
                {
                    fileLocator = new FileLocator();
                    filePath = fileLocator.SearchForFilePath();
                }
            }

            //File Path exists, File can be processed

            if (!string.IsNullOrEmpty(filePath))
            {
                if(GUILayout.Button("Create Dialogue Asset File"))
                {
                    CreateDialogueFile();
                }
            }
        }


        public void CreateDialogueFile()
        {
            List<string[]> fileContent = new List<string[]>();

            //Load file content & parse data using the parser
            if (!string.IsNullOrEmpty(filePath))
            {
                if (fileReader == null)
                {
                    fileReader = new CSVReader();
                }

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

                //Log out to user about success
                EditorUtility.DisplayDialog("SUCCESS", "Dialogue SO successfully created", "OK");
            }
            else
            {
                EditorUtility.DisplayDialog("ERROR", "Check Dialogue csv file, bad data", "OK");
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
            string path = "Assets/DialogueSO.asset";
            UnityEditor.AssetDatabase.CreateAsset(dialogueDataSO, path);
            UnityEditor.AssetDatabase.SaveAssets(); 

            Debug.Log("Dialogue SO created");
        }
    }
}
