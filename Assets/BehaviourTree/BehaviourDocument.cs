using SimpleJSON;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class BehaviourDocument
{
    public List<BehaviourNode> nodes = new List<BehaviourNode>();

    public NodeTree GenerateTree(Type t)
    {
        NodeTree tree = new NodeTree();

        //Create root
        var root = nodes.FirstOrDefault(node => node.parentId.Equals(string.Empty));
        Node rootNode = new Node(root.id, GetActions(root, t));
        UnityEngine.Debug.Log("Root node conditions: " + rootNode.actions.Count);

        var children = nodes.Where(node => nodes.All(n => !n.parentId.Equals(string.Empty))).ToList();
        UnityEngine.Debug.Log("Found: " + children.Count + " children");
        List<Node> nodeChildren = new List<Node>();

        // Generate children
        for(int i = 0; i < children.Count; i++)
        {
            // Missing parents right now
            nodeChildren.Add(new Node(children[i].parentId, GetActions(children[i], t)));
            UnityEngine.Debug.Log("Added child");
        }

        // Assign parents
        for (int i = 0; i < children.Count; i++)
        {
            var parent = rootNode.children.FirstOrDefault(node => !node.id.Equals(children[i].parentId));
            nodeChildren[i].SetParent(parent);
            UnityEngine.Debug.Log("Child node: " + children[i].id + " set its parent to " + parent.id);
        }

        return tree;
    }

    NodeAction[] GetActions(BehaviourNode node, Type t)
    {
        List<NodeAction> actions = new List<NodeAction>();
        for (int i = 0; i < node.actions.Count; i++)
        {
            NodeAction action = new NodeAction(node.actions[i].id, id => t.GetMethod(node.actions[i].callback).Invoke(t.GetType(), new object[] { id }), () => CheckConditions(t, node.actions[i].conditions));
            actions.Add(action);
        }
        return actions.ToArray();
    }

    bool CheckConditions(Type t, List<BehaviourCondition> conditions)
    {
        List<bool> checkList = new List<bool>();
        // Parse conditions here
        for (int i = 0; i < conditions.Count; i++)
        {
            BehaviourCondition.BehaviourCheck check = conditions[i].RunCheck();
            var field = t.GetField(conditions[i].variable);
            var val = field.GetValue(t);
            var checkVal = conditions[i].value;

            switch (check)
            {
                case BehaviourCondition.BehaviourCheck.EQUALS:
                    checkList.Add(val.Equals(checkVal));
                    break;
                case BehaviourCondition.BehaviourCheck.GREATER_THAN:
                    checkList.Add((float)val > (float)checkVal);
                    break;
                case BehaviourCondition.BehaviourCheck.GREATER_THAN_EQUAL_TO:
                    checkList.Add((float)val >= (float)checkVal);
                    break;
                case BehaviourCondition.BehaviourCheck.LESS_THAN:
                    checkList.Add((float)val < (float)checkVal);
                    break;
                case BehaviourCondition.BehaviourCheck.LESS_THAN_EQUAL_TO:
                    checkList.Add((float)val <= (float)checkVal);
                    break;
                default:
                    break;
            }
        }
        return checkList.TrueForAll(e => e == true);
    }

    public static BehaviourDocument Parse(JSONNode node)
    {
        if (node.Count <= 0)
            return null;

        BehaviourDocument document = new BehaviourDocument();
        for (int i = 0; i < node.Count; i++)
        {
            BehaviourNode behaviourNode = new BehaviourNode();
            var id = node[i]["id"];
            var parent = node[i]["parent"];
            behaviourNode.id = id;
            behaviourNode.parentId = parent;

            var action = node[i]["action"];

            if (action.Count > 1)
                throw new ArgumentOutOfRangeException($"Too many actions on node {id} | Only 1 is supported.");

            BehaviourAction actions = new BehaviourAction();
            var actionId = action[0]["id"];
            var actionCallback = action[0]["callback"];
            var conditions = action[0]["conditions"];

            actions.id = actionId;
            actions.callback = actionCallback;
            for (int k = 0; k < conditions.Count; k++)
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
            behaviourNode.actions.Add(actions);
            document.nodes.Add(behaviourNode);

        }
        return document;
    }
}

public class BehaviourNode
{
    public string id = string.Empty;
    public string parentId = string.Empty;
    public List<BehaviourAction> actions = new List<BehaviourAction>();
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
    public object value = -1;

    public enum BehaviourCheck
    {
        UNDEFINED, EQUALS, GREATER_THAN, LESS_THAN, GREATER_THAN_EQUAL_TO, LESS_THAN_EQUAL_TO
    }

    public BehaviourCheck RunCheck()
    {
        switch (check.ToLower())
        {
            case "equals":
                return BehaviourCheck.EQUALS;
            case "greater_than":
                return BehaviourCheck.GREATER_THAN;
            case "less_than":
                return BehaviourCheck.LESS_THAN;
            case "greater_than_equal_to":
                return BehaviourCheck.GREATER_THAN_EQUAL_TO;
            case "less_than_equal_to":
                return BehaviourCheck.LESS_THAN_EQUAL_TO;
            default:
                return BehaviourCheck.UNDEFINED;
        }
    }
}
