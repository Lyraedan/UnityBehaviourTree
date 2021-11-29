using System;
using UnityEngine;
using UnityEditor;

public class EditorConnection
{
    public EditorConnectionPoint inPoint, outPoint;
    public Action<EditorConnection> OnClickRemoveConnection;

    public EditorConnection(EditorConnectionPoint inPoint, EditorConnectionPoint outPoint, Action<EditorConnection> OnClickRemoveConnection)
    {
        this.inPoint = inPoint;
        this.outPoint = outPoint;
        this.OnClickRemoveConnection = OnClickRemoveConnection;
    }

    public void Draw()
    {
        Handles.DrawBezier(inPoint.rect.center, outPoint.rect.center,
                           inPoint.rect.center + Vector2.left * 50f, outPoint.rect.center - Vector2.left * 50f,
                           Color.white,
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
