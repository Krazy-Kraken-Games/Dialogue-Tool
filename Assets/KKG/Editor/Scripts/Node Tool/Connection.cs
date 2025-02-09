using UnityEditor;
using UnityEngine;

namespace KKG.Tool.Dialogue
{
    public class Connection
    {
        public DialogueTreeNode input;
        public DialogueTreeNode output;

        private bool multiOutputConnection = false;
        public Rect inputPosition;

        public Connection(DialogueTreeNode input, DialogueTreeNode output, bool isOption = false)
        {
            this.input = input;
            this.output = output;

            if (!isOption)
            {
                input.SetNextIndex(output.data.Id);
            }
            else
            {
                input.SetNextIndex(null);
            }
        }

        public void SetFromOutputNode(Rect _startPosition)
        {
            multiOutputConnection = true;
            inputPosition = _startPosition;
        }

        public void Draw()
        {
            Handles.color = Color.yellow;
            Vector2 startPosition = Vector2.zero;
            Vector2 endPosition = Vector2.zero;
            if (!multiOutputConnection)
            {
                startPosition = new Vector2(input.outputNodeRect.center.x, input.outputNodeRect.center.y);
              
            }
            else
            {
                startPosition = input.rect.position + inputPosition.center;
            }
            endPosition = new Vector2(output.inputNodeRect.center.x, output.inputNodeRect.center.y);
            Handles.DrawAAPolyLine(3f, new Vector3[] { startPosition, endPosition });
            DrawArrowhead(startPosition, endPosition, Color.yellow);

            Handles.color = Color.white;
        }

        private void DrawArrowhead(Vector3 start, Vector3 end, Color drawColor)
        {
            // Calculate the direction and perpendicular vector
            Vector3 direction = (end - start).normalized;
            Vector3 perpendicular = Vector3.Cross(direction, Vector3.forward).normalized;

            // Arrowhead size
            float arrowSize = 10f;

            // Points for the arrowhead triangle
            Vector3 arrowTip = end;
            Vector3 leftBase = end - direction * arrowSize + perpendicular * arrowSize * 0.5f;
            Vector3 rightBase = end - direction * arrowSize - perpendicular * arrowSize * 0.5f;

            // Draw the arrowhead
            Handles.color = drawColor;
            Handles.DrawAAConvexPolygon(arrowTip, leftBase, rightBase);
        }
    }
}
