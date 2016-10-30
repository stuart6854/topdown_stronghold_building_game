using BehaviourTrees;
using UnityEngine;

public class DoJob : LeafNode {

    public override bool IsValid(Blackboard blackboard) {
        return true;
    }

    public override void OnStart(Blackboard blackboard) {  }

    public override void Update(Blackboard blackboard) {
        Character character = (Character)blackboard.getTreeMemoryLocation("Character");

	    if(character.GetCurrentJob() == null) {
		    this.nodeStatus = NodeStatus.Failure;
			return;
	    }


	    if(character.GetCurrentTile() != character.GetCurrentJob().GetTile()) {
		    this.nodeStatus = NodeStatus.Failure;
			Debug.LogError("Character::BehaviourTree::DoJob(Leaf) -> I am trying to do my job without being at its tile!");
			return;
	    }


//		Debug.Log("Doing job.");
		if(character.GetCurrentJob().DoJob(Time.deltaTime))
            this.nodeStatus = NodeStatus.Success;
    }

    public override void OnEnd(Blackboard blackboard) {  }

}
