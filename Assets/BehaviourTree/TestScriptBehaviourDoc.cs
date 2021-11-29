using SimpleJSON;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScriptBehaviourDoc : MonoBehaviour
{
    public TextAsset behaviourDoc;
    public BehaviourDocument behaviour;

    // Start is called before the first frame update
    void Start()
    {
        var json = JSON.Parse(behaviourDoc.text);
        behaviour = BehaviourDocument.Parse(json);
        behaviour.GenerateTree(GetType());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
