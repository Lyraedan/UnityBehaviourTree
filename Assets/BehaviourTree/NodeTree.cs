using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTree : MonoBehaviour
{
    private int currentChild = 0;
    public Node root;
    public Node selected;

    public void ExecuteNodeTree()
    {
        // Execute our selected
        bool selectedExecuted = selected.Execute();

        while (selectedExecuted)
        {
            if (currentChild >= selected.children.Count)
            {
                Debug.Log("<color=#ff0000>No more child elements to check through on " + selected.id + " -> Exiting node tree</color>");
                currentChild = 0;
                selectedExecuted = false;
                break;
            }

            // Execute the current child
            Node child = selected.children[currentChild];
            bool childExecuted = child.Execute();
            if (childExecuted)
            {
                Debug.Log("<color=#00ff00>Child " + currentChild + " successfully executed</color>");
                selected = selected.children[currentChild];
                currentChild = 0;
                selectedExecuted = true;
            }
            else
            {
                currentChild++;
                Debug.Log("Moving onto child " + currentChild);
            }
        }

        if (currentChild > 0)
        {
            for (int i = 0; i < currentChild; i++)
            {
                // Flag nodes in prev tree as non-executed
                selected.hasExecuted = false;

                if (selected.parent != null)
                    selected = selected.parent;
            }
        }
        selected = root;
        currentChild = 0;
        Debug.Log("Executed node tree");
    }
}
