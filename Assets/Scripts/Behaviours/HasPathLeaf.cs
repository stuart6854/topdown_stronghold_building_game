using BehaviourTrees;
using UnityEngine;

public class HasPathLeaf : LeafNode {

    public override bool IsValid(Blackboard blackboard) {
        return true;
    }

    public override void OnStart(Blackboard blackboard) {
        Character character = (Character)blackboard.getTreeMemoryLocation("Character");

	    if(character.GetCurrentPath() == null) {
		    this.nodeStatus = NodeStatus.Failure;
//			Debug.Log("Dont have path.");
	    } else {
		    this.nodeStatus = NodeStatus.Success;
//			Debug.Log("Have path.");
		}
    }

    public override void Update(Blackboard blackboard) {  }

    public override void OnEnd(Blackboard blackboard) {  }

}
