using BehaviourTrees;
using UnityEngine;

public class RepeatTillHaveJobRequirements : DecoratorNode {

	public override bool IsValid(Blackboard blackboard) {
		return true;
	}

	public override void OnStart(Blackboard blackboard) {
		Character character = (Character)blackboard.getTreeMemoryLocation("Character");

		if(character.HasJobRequirements()) {
			this.nodeStatus = NodeStatus.Success;
//			Debug.Log("Has Requirements");
		}
	}

	public override void Update(Blackboard blackboard) {
		Character character = (Character)blackboard.getTreeMemoryLocation("Character");

		if(childNode.nodeStatus == NodeStatus.Failure) {
			//If Node Failed
			childNode.OnEnd(blackboard); //Call nodes OnEnd Method
			this.nodeStatus = NodeStatus.Failure;
		} else if(childNode.nodeStatus == NodeStatus.Success) {
			//If Node Succeeded
			childNode.OnEnd(blackboard); //Call nodes OnEnd Method
			childNode.nodeStatus = NodeStatus.Ready;
		} else if(childNode.nodeStatus == NodeStatus.Running) {
			//If Node is Running
			childNode.Update(blackboard); //Update Node
		} else if(childNode.nodeStatus == NodeStatus.Ready) {
			//If Node is Ready
			if(!childNode.IsValid(blackboard))
				return; //Exit Update Call

			childNode.OnStart(blackboard); //Call nodes OnStart Method
			childNode.nodeStatus = NodeStatus.Running; //Set Nodes status to Running to start updating it
		}


		if(character.HasJobRequirements()) {
			this.nodeStatus = NodeStatus.Success;
			Debug.Log("Has Requirements");
		} else {
			Debug.Log("Dont have requirements still");
		}
	}

	public override void OnEnd(Blackboard blackboard) { }

}
