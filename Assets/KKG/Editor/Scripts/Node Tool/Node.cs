using UnityEditor;
using UnityEngine;

namespace KKG.Tool.Dialogue
{
    public class Node
    {
        public string title;
        public Rect rect;

        //Styles
        public GUIStyle style;
        public GUIStyle defaultNodeStyle;
        public GUIStyle selectedNodeStyle;
        
        public Node(Vector2 pos, float width = 150, float height = 100)
        {
            rect = new Rect(pos.x,pos.y, width, height);
            title = "Node";

            //Default Style
            defaultNodeStyle = new GUIStyle();
            defaultNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
            defaultNodeStyle.border = new RectOffset(12,12,12, 12);

            //SelectedStyle
            selectedNodeStyle = new GUIStyle();
            selectedNodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
            selectedNodeStyle.border = new RectOffset(12, 12, 12, 12);

            style = defaultNodeStyle;
        }

        public void Draw()
        {
            GUI.Box(rect,title, style); 
        }
    }
}
