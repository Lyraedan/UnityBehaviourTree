#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(NodeTree))]
[CanEditMultipleObjects]
public class NodeEditor : Editor
{
    //SerializedProperty sp;

    private Material material;

    private void OnEnable()
    {
        //sp = serializedObject.FindProperty("sp");
        material = new Material(Shader.Find("Hidden/Internal-Colored"));
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        NodeTree tree = (NodeTree) target;
        //EditorGUILayout.PropertyField(sp);

        GUILayout.BeginVertical(EditorStyles.helpBox);

        Rect bounds = GUILayoutUtility.GetRect(10, 10000, 200, 200);
        if (Event.current.type == EventType.Repaint)
        {
            GUI.BeginClip(bounds);
            GL.PushMatrix();
            GL.Clear(true, false, Color.black);
            material.SetPass(0);

            GL.Begin(GL.QUADS);
            GL.Color(Color.black);
            GL.Vertex3(0, 0, 0);
            GL.Vertex3(bounds.width, 0, 0);
            GL.Vertex3(bounds.width, bounds.height, 0);
            GL.Vertex3(0, bounds.height, 0);
            GL.End();

            GL.PushMatrix();
            if (tree.root != null)
            {
                DrawNode(tree.root, new Vector2((bounds.width / 2) - 55, 0));
                DrawChildren(tree.root);
                //DrawLine(new Vector2(0, 0), new Vector2(100, 100));
            }
            GL.PopMatrix();

            GL.PopMatrix();
            GUI.EndClip();
        }

        GUILayout.EndVertical();

        serializedObject.ApplyModifiedProperties();
    }

    void DrawLine(Vector2 src, Vector2 dest)
    {
        GL.Begin(GL.LINES);
        GL.Color(Color.red);
        GL.Vertex3(src.x, src.y, 0f);
        GL.Vertex3(dest.x, dest.y, 0f);
        GL.End();
    }

    void DrawNode(Node node, Vector2 v)
    {
        int w = 100;
        int h = 80;
        Rect bounds = new Rect(v.x, v.y, w, h);

        GL.Begin(GL.QUADS);
        Color color = Color.white;
        if (node.hasExecuted) color = Color.yellow;
        else if (node.conditionsNotMet) color = Color.red;
        GL.Color(color);
        GL.Vertex3(bounds.x, bounds.y, 0);
        GL.Vertex3(bounds.x + bounds.width, bounds.y, 0);
        GL.Vertex3(bounds.x + bounds.width, bounds.y + bounds.height, 0);
        GL.Vertex3(bounds.x, bounds.y + bounds.height, 0);
        GL.End();
    }

    // Span through every possible child and draw them
    void DrawChildren(Node node, int yOffset = 0)
    {
        int nodeXOffset = 110;
        int nodeYOffset = 90;
        // 110, 90
        for(int i = 0; i < node.children.Count; i++)
        {
            int xOff = nodeXOffset + (nodeXOffset * i);
            int yOff = nodeYOffset + (nodeYOffset * yOffset);
            Vector2 nodePos = new Vector2(xOff, yOff);
            DrawNode(node.children[i], nodePos);
        }

        for(int i = 0; i < node.children.Count; i++)
        {
            DrawChildren(node.children[i], yOffset + 1);
        }
    }
}
#endif