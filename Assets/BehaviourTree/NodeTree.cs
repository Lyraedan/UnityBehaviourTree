using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTree : MonoBehaviour
{
    private int currentChild = 0;
    public Node selected;

    public void ExecuteNodeTree()
    {
        // Execute our selected
        bool selectedExecuted = selected.Execute();

        while (selectedExecuted)
        {
            if (currentChild >= selected.children.Count)
            {
                Debug.LogError("No more child elements to check through on " + selected.id);
                selectedExecuted = false;
                break;
            }

            // Execute the current child
            bool childExecuted = selected.children[currentChild].Execute();
            if (childExecuted)
            {
                selected = selected.children[currentChild];
                currentChild = 0;
                selectedExecuted = true;
            }
            else
            {
                currentChild++;
            }
        }
        Debug.Log("Executed node tree");
    }
}
