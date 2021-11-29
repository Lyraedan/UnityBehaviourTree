using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class NodeBasedEditor : EditorWindow
{
    private static NodeBasedEditor window;
    [SerializeField] private NodeTree currentTree;
    private bool loadedTree = false;

    private List<EditorNode> nodes;
    private List<EditorConnection> connections;

    private GUIStyle nodeStyle;
    private GUIStyle selectedStyle;

    private GUIStyle executingStyle;
    private GUIStyle executingSelectedStyle;

    private GUIStyle inPointStyle;
    private GUIStyle outPointStyle;

    private EditorConnectionPoint selectedInPoint;
    private EditorConnectionPoint selectedOutPoint;

    private Vector2 offset;
    private Vector2 drag;

    [MenuItem("Window/Node Based Editor")]
    private static void OpenWindow()
    {
        window = GetWindow<NodeBasedEditor>();
        window.titleContent = new GUIContent("Node Based Editor");
    }

    private void OnEnable()
    {
        nodeStyle = new GUIStyle();
        nodeStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        nodeStyle.border = new RectOffset(12, 12, 12, 12);

        selectedStyle = new GUIStyle();
        selectedStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1 on.png") as Texture2D;
        selectedStyle.border = new RectOffset(12, 12, 12, 12);

        executingStyle = new GUIStyle();
        executingStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2.png") as Texture2D;
        executingStyle.border = new RectOffset(12, 12, 12, 12);

        executingSelectedStyle = new GUIStyle();
        executingSelectedStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2 on.png") as Texture2D;
        executingSelectedStyle.border = new RectOffset(12, 12, 12, 12);

        inPointStyle = new GUIStyle();
        inPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left.png") as Texture2D;
        inPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn left on.png") as Texture2D;
        inPointStyle.border = new RectOffset(4, 4, 12, 12);

        outPointStyle = new GUIStyle();
        outPointStyle.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right.png") as Texture2D;
        outPointStyle.active.background = EditorGUIUtility.Load("builtin skins/darkskin/images/btn right on.png") as Texture2D;
        outPointStyle.border = new RectOffset(4, 4, 12, 12);
    }

    private void OnGUI()
    {
        DrawGrid(20, 0.2f, Color.gray);
        DrawGrid(100, 0.4f, Color.gray);

        DrawNodes();
        DrawConnections();

        DrawConnectionLine(Event.current);
        DrawInspector();

        ProcessNodeEvents(Event.current);
        ProcessEvents(Event.current);

        if (GUI.changed)
            Repaint();
    }

    private void DrawInspector()
    {
        var obj = new SerializedObject(this);

        SerializedProperty tree = obj.FindProperty("currentTree");
        var selectedTree = EditorGUILayout.PropertyField(tree, new GUIContent("Current Tree"), GUILayout.Height(25));
        currentTree = (NodeTree)tree.objectReferenceValue;
        if(currentTree != null)
        {
            // Load tree
            if (!loadedTree)
            {
                loadedTree = true;
                LoadTree(currentTree);
            }
        } else
        {
            if (loadedTree)
            {
                ClearAllNodes();
                ClearConnections();
                loadedTree = false;
            }
        }
        obj.ApplyModifiedProperties();
    }

    private void DrawNodes()
    {
        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Draw();
            }
        }
    }

    private void DrawConnections()
    {
        if (connections != null)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                connections[i].Draw();
            }
        }
    }

    private void DrawConnectionLine(Event e)
    {
        if (selectedInPoint != null && selectedOutPoint == null)
        {
            Handles.DrawBezier(
                selectedInPoint.rect.center,
                e.mousePosition,
                selectedInPoint.rect.center + Vector2.left * 50f,
                e.mousePosition - Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }

        if (selectedOutPoint != null && selectedInPoint == null)
        {
            Handles.DrawBezier(
                selectedOutPoint.rect.center,
                e.mousePosition,
                selectedOutPoint.rect.center - Vector2.left * 50f,
                e.mousePosition + Vector2.left * 50f,
                Color.white,
                null,
                2f
            );

            GUI.changed = true;
        }
    }

    private void DrawGrid(float gridSpacing, float gridOpacity, Color gridColor)
    {
        int widthDivs = Mathf.CeilToInt(position.width / gridSpacing);
        int heightDivs = Mathf.CeilToInt(position.height / gridSpacing);

        Handles.BeginGUI();
        Handles.color = new Color(gridColor.r, gridColor.g, gridColor.b, gridOpacity);

        offset += drag * 0.5f;
        Vector3 newOffset = new Vector3(offset.x % gridSpacing, offset.y % gridSpacing, 0);

        for (int i = 0; i < widthDivs; i++)
        {
            Handles.DrawLine(new Vector3(gridSpacing * i, -gridSpacing, 0) + newOffset, new Vector3(gridSpacing * i, position.height, 0f) + newOffset);
        }

        for (int j = 0; j < heightDivs; j++)
        {
            Handles.DrawLine(new Vector3(-gridSpacing, gridSpacing * j, 0) + newOffset, new Vector3(position.width, gridSpacing * j, 0f) + newOffset);
        }

        Handles.color = Color.white;
        Handles.EndGUI();
    }

    private void ProcessEvents(Event e)
    {
        drag = Vector2.zero;

        switch (e.type)
        {
            case EventType.MouseDown:
                if (e.button == 1)
                {
                    ProcessContextMenu(e.mousePosition);
                }
                break;
            case EventType.MouseDrag:
                if (e.button == 0)
                {
                    OnDrag(e.delta);
                }
                break;
        }
    }

    private void OnDrag(Vector2 delta)
    {
        drag = delta;

        if (nodes != null)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].Drag(delta);
            }
        }

        GUI.changed = true;
    }

    private void ProcessContextMenu(Vector2 mousePosition)
    {
        GenericMenu genericMenu = new GenericMenu();
        genericMenu.AddItem(new GUIContent("Add node"), false, () => OnClickAddNode(mousePosition));
        genericMenu.ShowAsContext();
    }

    private void ProcessNodeEvents(Event e)
    {
        if (nodes != null)
        {
            for (int i = nodes.Count - 1; i >= 0; i--)
            {
                bool guiChanged = nodes[i].ProcessEvents(e);
                if (guiChanged)
                    GUI.changed = true;
            }
        }
    }

    private void OnClickAddNode(Vector2 mousePosition)
    {
        if (nodes == null)
        {
            nodes = new List<EditorNode>();
        }

        Node node = new Node(GUID.Generate().ToString());
        nodes.Add(new EditorNode(node, mousePosition, 200, 50, nodeStyle, selectedStyle, executingStyle, executingSelectedStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode));
    }

    private void OnClickInPoint(EditorConnectionPoint inPoint)
    {
        selectedInPoint = inPoint;

        if (selectedOutPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickOutPoint(EditorConnectionPoint outPoint)
    {
        selectedOutPoint = outPoint;

        if (selectedInPoint != null)
        {
            if (selectedOutPoint.node != selectedInPoint.node)
            {
                CreateConnection();
                ClearConnectionSelection();
            }
            else
            {
                ClearConnectionSelection();
            }
        }
    }

    private void OnClickRemoveConnection(EditorConnection connection)
    {
        connections.Remove(connection);
    }

    private void CreateConnection()
    {
        if (connections == null)
        {
            connections = new List<EditorConnection>();
        }

        connections.Add(new EditorConnection(null, selectedInPoint, selectedOutPoint, OnClickRemoveConnection));
    }

    private void ClearConnectionSelection()
    {
        selectedInPoint = null;
        selectedOutPoint = null;
    }

    private void OnClickRemoveNode(EditorNode node)
    {
        if (connections != null)
        {
            List<EditorConnection> connectionsToRemove = new List<EditorConnection>();

            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint)
                {
                    connectionsToRemove.Add(connections[i]);
                }
            }

            for (int i = 0; i < connectionsToRemove.Count; i++)
            {
                connections.Remove(connectionsToRemove[i]);
            }
            connectionsToRemove = null;
        }
        nodes.Remove(node);
    }

    private void ClearAllNodes()
    {
        if (nodes != null)
        {
            if (connections != null)
            {
                foreach (EditorNode node in nodes)
                {
                    List<EditorConnection> connectionsToRemove = new List<EditorConnection>();

                    for (int i = 0; i < connections.Count; i++)
                    {
                        if (connections[i].inPoint == node.inPoint || connections[i].outPoint == node.outPoint)
                        {
                            connectionsToRemove.Add(connections[i]);
                        }
                    }

                    for (int i = 0; i < connectionsToRemove.Count; i++)
                    {
                        connections.Remove(connectionsToRemove[i]);
                    }
                    connectionsToRemove = null;
                }
            }
            nodes.Clear();
        }
    }

    private void ClearConnections()
    {
        if(connections != null)
        {
            connections.Clear();
        }

        selectedInPoint = null;
        selectedOutPoint = null;
    }

    private void LoadTree(NodeTree tree)
    {
        if (nodes == null)
        {
            nodes = new List<EditorNode>();
        }
        // Root
        if (tree.root != null)
        {
            Vector2 placement = new Vector2(0, 50 * (tree.root.children.Count / 2));
            EditorNode rootNode = new EditorNode(tree.root, placement, 200, 50, nodeStyle, selectedStyle, executingStyle, executingSelectedStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
            rootNode.actionId = tree.root.id;
            nodes.Add(rootNode);
            AddBranches(tree.root, placement);
        }
    }

    private void AddBranches(Node node, Vector2 placement, int child = 0)
    {
        Vector2 position = placement;
        int nodeWidth = 200;
        int nodeHeight = 50;
        position.x += (nodeWidth * 1.2f) + 20 * child;
        position.y = 0;

        // Add the nodes
        for (int i = 0; i < node.children.Count; i++)
        {
            // Todo fix positioning for Y coord
            position.y += nodeHeight * i;
            EditorNode editorNode = new EditorNode(node.children[i], position, nodeWidth, nodeHeight, nodeStyle, selectedStyle, executingStyle, executingSelectedStyle, inPointStyle, outPointStyle, OnClickInPoint, OnClickOutPoint, OnClickRemoveNode);
            editorNode.actionId = node.children[i].id;
            nodes.Add(editorNode);
        }

        // Create connections
        if(node.parent != null)
        {
            var editorNodeParent = nodes.Single(enode => enode.actionId.Equals(node.parent.id));
            var current = nodes.Single(enode => enode.actionId.Equals(node.id));

            var parentOut = editorNodeParent.outPoint;
            var currentIn = current.inPoint;

            if (connections == null)
            {
                connections = new List<EditorConnection>();
            }

            connections.Add(new EditorConnection(node, parentOut, currentIn, OnClickRemoveConnection));
        }

        // Run through the trees
        for (int i = 0; i < node.children.Count; i++)
        {
            AddBranches(node.children[i], position, child + 1);
        }
    }
}
