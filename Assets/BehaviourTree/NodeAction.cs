using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class NodeAction
{
    public string id = "Node action";
    public List<bool> conditions = new List<bool>();
    public Action<string> callback;

    bool ConditionsMet
    {
        get
        {
            // If there are no conditions auto meet
            if (conditions.Count == 0)
                return true;

            return conditions.TrueForAll(i => i.Equals(true));
        }
    }

    public NodeAction(string id, Action<string> callback, params bool[] conditions)
    {
        this.id = id;
        this.callback = callback;
        this.conditions = conditions.OfType<bool>().ToList();
    }

    public NodeAction(string id, params bool[] conditions)
    {
        this.id = id;
        this.conditions = conditions.OfType<bool>().ToList();
    }

    public bool Execute()
    {
        if(ConditionsMet)
        {
            callback?.Invoke(id);
            return true;
        } else
        {
            UnityEngine.Debug.LogError("Conditions not met for " + id + " [" + conditions.Count + "]");
            return false;
        }
    }
}
