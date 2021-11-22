using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Node
{

    public Node parent = null;
    public List<Node> children = new List<Node>();
    public string id = "Node";
    public List<NodeAction> actions = new List<NodeAction>();
    public bool hasExecuted = false;

    public Node(string id, params NodeAction[] actions)
    {
        this.id = id;
        this.actions = actions.OfType<NodeAction>().ToList();
    }

    public Node(Node parent, string id, params NodeAction[] actions)
    {
        this.parent = parent;
        parent.children.Add(this);
        this.id = id;
        this.actions = actions.OfType<NodeAction>().ToList();
    }

    public bool Execute()
    {
        // If there is no parent skip this
        if (parent != null)
        {
            if (!parent.hasExecuted)
            {
                return false;
            }
        }

        int count = 0;
        for(int i = 0; i < actions.Count; i++) {
            bool success = actions[i].Execute();
            if (success)
                count++;
        }
        hasExecuted = count >= actions.Count;
        return hasExecuted;
    }
}
