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
            int met = 0;
            for(int i = 0; i < conditions.Count; i++)
            {
                if(conditions[i])
                {
                    met++;
                }
            }
            return met >= conditions.Count;
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
            UnityEngine.Debug.LogError("Conditions not met for " + id);
            return false;
        }
    }
}
