using BehaviourTrees;
using UnityEngine;

public class SetJobDest : LeafNode {

	public override bool IsValid(Blackboard blackboard) {
		return true;
	}

	public override void OnStart(Blackboard blackboard) {
		Character character = (Character)blackboard.getTreeMemoryLocation("Character");

		//We have job requirements so we ensure that are next destination is the job tile
//		Debug.Log("I have requirements. Setting dest to jobs tile!");
		character.SetDestination(character.GetCurrentJob().GetTile());
		nodeStatus = NodeStatus.Success;
	}

	public override void Update(Blackboard blackboard) { }

	public override void OnEnd(Blackboard blackboard) { }

}


