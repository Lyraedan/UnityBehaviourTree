# UnityBehaviourTree
Node based behaviour tree in unity

<b>The behaviour tree itself is not Unity specific. The nodes can be taken out and used in any C# project.</b>

<b>Unity specific classes</b>
`NodeEditor.cs`,
`TestScript.cs`

<b>Non Unity specific classes</b>
`Node.cs`,
`NodeTree.cs`,
`NodeAction.cs`,

To use NodeTree outside of Unity, remove `using UnityEngine;` and remove the inheritence of `MonoBehaviour`

Written by Luke Rapkin
