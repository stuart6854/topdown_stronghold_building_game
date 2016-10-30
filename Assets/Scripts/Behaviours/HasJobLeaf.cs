using BehaviourTrees;
using UnityEngine;

public class HasJobLeaf : LeafNode {

    public override bool IsValid(Blackboard blackboard) {
        return true;
    }

    public override void OnStart(Blackboard blackboard) {
        Character character = (Character)blackboard.getTreeMemoryLocation("Character");

	    if(character.GetCurrentJob() == null) {
		    this.nodeStatus = NodeStatus.Failure;
//			Debug.Log("Dont have job.");
	    } else {
		    this.nodeStatus = NodeStatus.Success;
//			Debug.Log("Have job.");
		}
    }

    public override void Update(Blackboard blackboard) {  }

    public override void OnEnd(Blackboard blackboard) {  }

}
