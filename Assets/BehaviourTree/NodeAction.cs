using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NodeAction
{
    public string id = "Node action";
    public Func<bool> predicts;
    public Action<string> callback;

    bool ConditionsMet
    {
        get
        {
            // If there are no conditions auto meet
            if (predicts == null)
                return true;

            // Conditions need refreshing after each node execution
            return predicts();
        }
    }

    public NodeAction(string id, Action<string> callback, Func<bool> predicts = null)
    {
        this.id = id;
        this.callback = callback;
        this.predicts = predicts;
    }

    public NodeAction(string id, Func<bool> predicts)
    {
        this.id = id;
        this.predicts = predicts;
    }

    public bool Execute()
    {
        if(ConditionsMet)
        {
            callback?.Invoke(id);
            return true;
        } else
        {
            UnityEngine.Debug.LogError("Conditions not met for " + id);
            return false;
        }
    }
}
