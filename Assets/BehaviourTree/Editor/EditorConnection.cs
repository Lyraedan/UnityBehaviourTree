using System;
using UnityEngine;
using UnityEditor;

public class EditorConnection
{
    public EditorConnectionPoint inPoint, outPoint;
    public Action<EditorConnection> OnClickRemoveConnection;
    public Node node;

    public EditorConnection(Node node, EditorConnectionPoint inPoint, EditorConnectionPoint outPoint, Action<EditorConnection> OnClickRemoveConnection)
    {
        this.node = node;
        this.inPoint = inPoint;
        this.outPoint = outPoint;
        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }

    public void Draw()
    {
        Color color = Color.white;
        if(node != null)
        {
            color = node.hasExecuted ? Color.yellow : Color.white;
        }
        Handles.DrawBezier(inPoint.rect.center, outPoint.rect.center,
                           inPoint.rect.center + Vector2.left * 50f, outPoint.rect.center - Vector2.left * 50f,
                           color,
                           null,
                           2f);

        if(Handles.Button((inPoint.rect.center + outPoint.rect.center) * 0.5f, Quaternion.identity, 4, 8, Handles.RectangleHandleCap))
        {
            if(OnClickRemoveConnection != null)
            {
                OnClickRemoveConnection(this);
            }
        }
    }
}
