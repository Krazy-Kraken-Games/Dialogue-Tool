using UnityEngine;
using UnityEditor;

public static class EditorZoomArea
{
    private static Matrix4x4 prevGuiMatrix;

    public static Rect Begin(float zoomScale, Rect screenCoordsArea)
    {
        GUI.EndGroup(); // End the group Unity automatically begins for an EditorWindow to clip content
        Rect clippedArea = screenCoordsArea.ScaleSizeBy(1f / zoomScale, screenCoordsArea.TopLeft());
        GUI.BeginGroup(clippedArea);

        prevGuiMatrix = GUI.matrix;
        Matrix4x4 translation = Matrix4x4.TRS(clippedArea.TopLeft(), Quaternion.identity, Vector3.one);
        Matrix4x4 scaling = Matrix4x4.Scale(new Vector3(zoomScale, zoomScale, 1f));
        GUI.matrix = translation * scaling * translation.inverse * GUI.matrix;

        return clippedArea;
    }

    public static void End()
    {
        GUI.matrix = prevGuiMatrix;
        GUI.EndGroup();
        GUI.BeginGroup(new Rect(0, 0, Screen.width, Screen.height)); // Restart Unity's automatic group
    }

    private static Vector2 TopLeft(this Rect rect)
    {
        return new Vector2(rect.xMin, rect.yMin);
    }

    private static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
    {
        Rect result = rect;
        result.x -= pivotPoint.x;
        result.y -= pivotPoint.y;
        result.xMin *= scale;
        result.yMin *= scale;
        result.xMax *= scale;
        result.yMax *= scale;
        result.x += pivotPoint.x;
        result.y += pivotPoint.y;
        return result;
    }
}
