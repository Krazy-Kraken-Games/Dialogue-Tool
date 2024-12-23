using UnityEngine;
using UnityEditor;

namespace KKG.Tool.Dialogue
{
    public class DialogueNodeTool : EditorWindow
    {

        [MenuItem("Tools/Krazy Kraken Games/Dialogue Graph")]
        public static void OpenWindow()
        {
            DialogueNodeTool window = GetWindow<DialogueNodeTool>();
            window.titleContent = new GUIContent("Dialogue Graph");
        }

        private void OnGUI()
        {
            DrawGrid(20, 0.2f, Color.gray);
            DrawGrid(100, 0.4f, Color.green);
        }

        private void DrawGrid(float gridSpacing,float gridOpacity,Color gridColor)
        {
            int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
            int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

            Handles.BeginGUI();
            Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

            for (int i = 0; i < widthDivs; i++)
            {
                Handles.DrawLine(new Vector3(gridSpacing * i,0,0),new Vector3(gridSpacing * i, position.height,0));
            }

            for(int j = 0; j< heightDivs; j++)
            {
                Handles.DrawLine(new Vector3(0,gridSpacing * j,0),new Vector3(position.width,gridSpacing * j,0f));  
            }

            Handles.color = Color.white;

            Handles.EndGUI();
        }
    }
}
