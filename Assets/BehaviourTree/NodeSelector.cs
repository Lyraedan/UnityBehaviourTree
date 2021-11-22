using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeSelector : MonoBehaviour
{
    public Node root;

    private int currentChild = 0;
    private Node selected;

    int i = 0;

    // Start is called before the first frame update
    void Start()
    {
        NodeAction masterAction = new NodeAction("Init", id => {
            Debug.Log("Node action executed for " + id + " :D");
            i++;
        }, i == 0);
        root = new Node("Start", masterAction);
        Debug.Log("Master assigned to node: " + root.id);

        NodeAction sharedAction = new NodeAction("Shared", SharedAction, i == 1);

        Node childA = new Node(root, "Test A", sharedAction);
        Node childB = new Node(root, "Test B", sharedAction);

        Node childC = new Node(childA, "Inner child", new NodeAction("Inner child", id => {
            Debug.Log("Child from within child (Child C) executed");
        }, i == 3));

        selected = root;
    }

    void SharedAction(string id)
    {
        Debug.Log("Child node shared action executed for " + id);
        i++;
    }

    // Update is called once per frame
    void Update()
    {
        // Execute our selected
        selected.Execute();

        if (selected.children.Count > 0)
        {
            if(currentChild >= selected.children.Count)
            {
                Debug.LogError("No more child elements to check through.");
                return;
            }
            // Execute the current child
            bool childExecuted = selected.children[currentChild].Execute();
            if (childExecuted)
            {
                selected = selected.children[currentChild];
                currentChild = 0;
            } else
            {
                currentChild++;
            }
        }
    }
}
