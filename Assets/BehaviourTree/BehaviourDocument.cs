using SimpleJSON;
using System;
using System.Collections;
using System.Collections.Generic;

public class BehaviourDocument
{
    public string id = string.Empty;
    public List<BehaviourAction> actions = new List<BehaviourAction>();

    public bool Execute()
    {
        return false;
    }

    public static BehaviourDocument Parse(JSONNode node)
    {
        if (node.Count <= 0)
            return null;

        BehaviourDocument document = new BehaviourDocument();
        for (int i = 0; i < node.Count; i++)
        {
            var id = node[i]["id"];
            document.id = id;

            var action = node[i]["action"];
            for (int j = 0; j < action.Count; i++)
            {
                BehaviourAction actions = new BehaviourAction();

                var actionId = action[j]["id"];
                var actionCallback = action[j]["callback"];
                var conditions = action[j]["conditions"];

                actions.id = actionId;
                actions.callback = actionCallback;
                for(int k = 0; k < conditions.Count; k++)
                {
                    BehaviourCondition condition = new BehaviourCondition();
                    var variable = conditions[k]["variable"];
                    var check = conditions[k]["check"];
                    var value = conditions[k]["value"];

                    condition.variable = variable;
                    condition.check = check;
                    condition.value = value;

                    actions.conditions.Add(condition);
                }

                document.actions.Add(actions);
            }
        }
        return document;
    }
}

public class BehaviourAction
{
    public string id = string.Empty;
    public string callback = string.Empty;
    public List<BehaviourCondition> conditions = new List<BehaviourCondition>();
}

public class BehaviourCondition
{
    public string variable = string.Empty;
    public string check = string.Empty;
    public object value;
}
