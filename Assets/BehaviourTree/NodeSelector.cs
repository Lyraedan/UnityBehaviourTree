using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSelector : MonoBehaviour
{
    private int currentChild = 0;
    private Node selected;

    int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        NodeAction masterAction = new NodeAction("Init", Init, i == 0);
        Node root = new Node("Start", masterAction);

        NodeAction actionA = new NodeAction("Action A", SharedAction, i == 1);
        NodeAction actionB = new NodeAction("Action B", SharedAction, i == 2);
        NodeAction actionC = new NodeAction("Fallback", Fallback);

        Node childA = new Node(root, "Test A", actionA);
        Node childB = new Node(root, "Test B", actionB);
        Node childC = new Node(root, "Test C", actionC);

        Node innerChild = new Node(childA, "Inner child", new NodeAction("Inner child", InnerChild));

        selected = root;
        ExecuteNodeTree();
    }

    void Init(string id)
    {
        Debug.Log("Node action executed for " + id + " :D");
        i++;
        Debug.Log("i = " + i);
    }

    void SharedAction(string id)
    {
        Debug.Log("Child node shared action executed for " + id);
        i++;
    }

    void InnerChild(string id)
    {
        Debug.Log("Child from within child (Child C) executed");
    }

    void Fallback(string id)
    {
        Debug.Log("All other node trees failed. Using fallback with " + id);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExecuteNodeTree()
    {
        // Execute our selected
        bool selectedExecuted = selected.Execute();

        while (selectedExecuted)
        {
            if (currentChild >= selected.children.Count)
            {
                Debug.LogError("No more child elements to check through on " + selected.id);
                break;
            }

            // Execute the current child
            bool childExecuted = selected.children[currentChild].Execute();
            if (childExecuted)
            {
                selected = selected.children[currentChild];
                currentChild = 0;
                selectedExecuted = selected.Execute();
            }
            else
            {
                currentChild++;
            }
        }
        Debug.Log("Executed node tree");
    }
}
