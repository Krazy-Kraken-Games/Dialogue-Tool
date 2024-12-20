using KKG.FileHandling;
using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using KKG.Dialogue;

public class FileReaderEditorWindow : EditorWindow
{
    KrakenFileReader fileReaderInstance;
    string filePath;

    [MenuItem("Tools/File Reader Tool")]
    public static void OpenWindow()
    {
        FileReaderEditorWindow window = GetWindow<FileReaderEditorWindow>("File Reader Tool");
        window.Show();
    }

    private void OnGUI()
    {
        GUILayout.Label("File Reader Tool", EditorStyles.boldLabel);

        if (!string.IsNullOrEmpty(filePath))
        {
            GUILayout.Label($"File Path: {filePath}");
        }

        //Browse Button
        if(GUILayout.Button("Browse File"))
        {
            filePath = EditorUtility.OpenFilePanel("Select CSV File", "", "csv");
        }

        //Process File Button
        if (GUILayout.Button("Process File"))
        {
            //File Path has been found

            if(!string.IsNullOrEmpty(filePath))
            {
                ProcessFile(filePath);
            }
            else
            {
                EditorUtility.DisplayDialog("Error", "Please select a file first", "OK");
            }
        }
    }

    private void ProcessFile(string filePath)
    {
        //Initialize the FileReader
        if(fileReaderInstance == null)
        {
            GameObject tempObject = new GameObject("File Reader");
            //fileReaderInstance = tempObject.AddComponent<KrakenFileReader>();
        }

        //Read file
        List<string[]> fileContent = new List<string[]>();

        CSVReader csvReader = new CSVReader();
        fileContent = csvReader.LoadFileViaPath(filePath);

        List<DialogueNode> DialogueNodes = new List<DialogueNode>();
        //Create dialog nodes for each row in the file Content
        //Skip 0th element, as its the sheet column headers
        for (int i = 1; i < fileContent.Count; i++)
        {
            DialogueNode node = fileReaderInstance.CreateDialogueNode(fileContent[i]);

            Debug.Log($"{node.Data.SpeakerName} : {node.Data.Message} and Has Options:{node.Data.Options.Count > 0}");

            DialogueNodes.Add(node);
        }

        if (DialogueNodes.Count > 0)
        {
            //Create the Scriptable Object Asset file
            fileReaderInstance.CreateDialogueSO(DialogueNodes);
            EditorUtility.DisplayDialog("SUCCESS", "Dialogue SO created successfully", "OK");
        }

        //Clean uo
        //DestroyImmediate(fileReaderInstance.gameObject);
    }
}
