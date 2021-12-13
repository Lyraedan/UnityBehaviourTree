using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestScript : MonoBehaviour
{
    public NodeTree tree;

    public enum State
    {
        DECIDE, IDLE, WALKING, HUNTING
    }

    public State state = State.DECIDE;
    public int i = 0;

    // Start is called before the first frame update
    void Start()
    {

        // Setup the entry node
        NodeAction masterAction = new NodeAction("Init", Decide, () => state.Equals(State.DECIDE));
        tree.root = new Node("Make decision", masterAction);

        // Create 3 nodes that branch of the entry node
        NodeAction actionA = new NodeAction("Go for walk", MakeDecision, () => state.Equals(State.IDLE));
        NodeAction actionB = new NodeAction("Start hunting", MakeDecision, () => state.Equals(State.HUNTING));
        NodeAction actionC = new NodeAction("Do nothing", Fallback); // NodeAction with no condition

        Node childA = new Node(tree.root, "Walk", actionA);
        Node childB = new Node(tree.root, "Hunt", actionB);
        Node childC = new Node(tree.root, "Nothing", actionC);

        // Create another node that branches off the first node we created past entry
        Node innerChild = new Node(childA, "End walk", new NodeAction("Finish walk", FinishWalk, () => state.Equals(State.WALKING)));
        /*
                Entry
                  V
              A   B   C
              v 
             ic
         */
        tree.selected = tree.root;
        tree.ExecuteNodeTree();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            i = 0;
            state = State.DECIDE;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            i = 1;
            state = State.DECIDE;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            i = 2;
            state = State.DECIDE;
        }

        tree.ExecuteNodeTree();
    }

    void Decide(string id)
    {
        Debug.Log("Making decision");
        if(i == 0)
        {
            state = State.IDLE;
        } else if(i == 1)
        {
            state = State.HUNTING;
        }
    }

    void MakeDecision(string id)
    {
        if(state.Equals(State.IDLE))
        {
            Debug.Log("Going for a walk");
            state = State.WALKING;
        } else
        {
            Debug.Log("Hunting");
        }
    }

    void FinishWalk(string id)
    {
        Debug.Log("Finished walk");
    }

    void Fallback(string id)
    {
        Debug.Log("All other node trees failed. Using fallback with " + id);
    }

}
