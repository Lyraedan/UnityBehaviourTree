using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using UnityEngine;

public class NodeCompiler : MonoBehaviour
{
    private StringBuilder script = new StringBuilder();
    private List<NodeCode> lines = new List<NodeCode>();

    private void Start()
    {
        Compile();
    }

    int i = 0;

    public void Compile()
    {
        script.Clear();

        object actionA = new NodeAction("Test action", id => { }, () => i == 0);
        object childA = new Node(null, "Test node", (NodeAction) actionA);

        AddCode(ref childA, "id");
        AddCode(ref actionA, "predicts");
    }

    public void AddCode(ref object obj, string variable)
    {
        NodeCode code = new NodeCode();

        Type type = obj.GetType();
        FieldInfo info = type.GetField(variable);

        code.variable = info;
        code.value = info.GetValue(obj);


        Debug.Log("Added code " + code.variable + " -> " + code.value);
        lines.Add(code);
    }
}

public class NodeCode
{
    public object variable;
    public object value;
}
