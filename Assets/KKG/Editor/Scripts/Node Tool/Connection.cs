using UnityEditor;
using UnityEngine;

namespace KKG.Tool.Dialogue
{
    public class Connection
    {
        public Node input;
        public Node output;

        public Connection(Node input, Node output)
        {
            this.input = input;
            this.output = output;
        }

        public void Draw()
        {
            Handles.color = Color.yellow;

            Vector2 startPosition = new Vector2(input.rect.xMax,input.rect.yMax/2);
            Vector2 endPosition = new Vector2(output.rect.xMin,output.rect.yMax/2);

            Handles.DrawAAPolyLine(3f, new Vector3[] {startPosition,endPosition} );

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
