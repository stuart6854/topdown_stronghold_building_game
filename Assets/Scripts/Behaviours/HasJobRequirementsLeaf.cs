using BehaviourTrees;
using UnityEngine;

public class HasJobRequirementsLeaf : LeafNode {

    public override bool IsValid(Blackboard blackboard) {
        return true;
    }

    public override void OnStart(Blackboard blackboard) {
        Character character = (Character)blackboard.getTreeMemoryLocation("Character");

	    if(!character.HasJobRequirements())
		    this.nodeStatus = NodeStatus.Failure;
	    else {
		    this.nodeStatus = NodeStatus.Success;
	    }
    }

    public override void Update(Blackboard blackboard) {  }

    public override void OnEnd(Blackboard blackboard) {  }

}
