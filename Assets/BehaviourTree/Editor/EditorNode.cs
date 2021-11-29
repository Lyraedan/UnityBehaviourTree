using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


//Ref https://gram.gs/gramlog/creating-node-based-editor-unity/
public class EditorNode
{
    public Rect rect;
    public Node node;
    public string title;
    public bool isDragged, isSelected;

    public EditorConnectionPoint inPoint, outPoint;

    public GUIStyle style;
    public GUIStyle defaultNodeStyle;
    public GUIStyle selectedNodeStyle;
    public GUIStyle executingNodeStyle;
    public GUIStyle executingSelectedNodeStyle;

    public Action<EditorNode> OnRemoveNode;

    // Node properties
    public string actionId;
    private Vector2 mousePosition;
    private int v1;
    private int v2;
    private GUIStyle nodeStyle;
    private GUIStyle selectedStyle;
    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;
    private Action<EditorConnectionPoint> onClickInPoint;
    private Action<EditorConnectionPoint> onClickOutPoint;
    private Action<EditorNode> onClickRemoveNode;

    public EditorNode(Node node, Vector2 position, float width, float height, GUIStyle style, GUIStyle selectedStyle, GUIStyle executingStyle, GUIStyle executingSelected, GUIStyle inPointStyle, GUIStyle outPointStyle, Action<EditorConnectionPoint> OnClickInPoint, Action<EditorConnectionPoint> OnClickOutPoint, Action<EditorNode> OnClickRemoveNode)
    {
        this.node = node;
        rect = new Rect(position.x, position.y, width, height);
        this.style = style;
        inPoint = new EditorConnectionPoint(this, ConnectionPointType.IN, inPointStyle, OnClickInPoint);
        outPoint = new EditorConnectionPoint(this, ConnectionPointType.OUT, outPointStyle, OnClickOutPoint);
        defaultNodeStyle = style;
        selectedNodeStyle = selectedStyle;
        executingNodeStyle = executingStyle;
        executingSelectedNodeStyle = executingSelected;
        OnRemoveNode = OnClickRemoveNode;
    }

    public void Drag(Vector2 delta)
    {
        rect.position += delta;
    }

    public void Draw()
    {
        inPoint.Draw();
        outPoint.Draw();
        GUI.Box(rect, title, style);
        GUI.Label(new Rect(rect.x + 20, rect.y + 10, rect.width / 4, 20), "ID:");
        actionId = GUI.TextField(new Rect(rect.x + 40, rect.y + 10, rect.width / 2, 20), actionId);
    }

    public bool ProcessEvents(Event e)
    {
        switch(e.type)
        {
            case EventType.MouseDown:
                if(e.button == 0)
                {
                    if(rect.Contains(e.mousePosition))
                    {
                        isDragged = true;
                        GUI.changed = true;
                        isSelected = true;
                        style = selectedNodeStyle;
                    } else
                    {
                        GUI.changed = true;
                        isSelected = false;
                        style = defaultNodeStyle;
                    }
                }

                if(e.button == 1 && isSelected && rect.Contains(e.mousePosition))
                {
                    ProcessContextMenu();
                    e.Use();
                }
                break;
            case EventType.MouseUp:
                isDragged = false;
                break;
            case EventType.MouseDrag:
                if(e.button == 0 && isDragged)
                {
                    Drag(e.delta);
                    e.Use();
                    return true;
                }
                break;
        }
        if (node != null)
        {
            if (node.hasExecuted)
            {
                style = isSelected ? executingSelectedNodeStyle : executingNodeStyle;
            }
        }
        return false;
    }

    private void ProcessContextMenu()
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Remove node"), false, OnClickRemoveNode);
        genericMenu.ShowAsContext();
    }
    private void OnClickRemoveNode()
    {
        if (OnRemoveNode != null)
        {
            OnRemoveNode(this);
        }
    }
}
