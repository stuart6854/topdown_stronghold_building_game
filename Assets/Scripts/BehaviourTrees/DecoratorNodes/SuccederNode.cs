namespace BehaviourTrees {

	/// <summary>
	/// Succeeder Nodes ALWAYS return SUCCESS, no matter what their child returns.
	/// </summary>
	public class SucceederNode : DecoratorNode {

		public override bool IsValid(Blackboard blackboard) {
			return true;
		}

		public override void OnStart(Blackboard blackboard) {}

		public override void Update(Blackboard blackboard) {
			if(childNode.nodeStatus == NodeStatus.Failure) {
				//If Node Failed
				childNode.OnEnd(blackboard); //Call nodes OnEnd Method
				nodeStatus = NodeStatus.Success; //Set Status to Success to inform parent node
			} else if(childNode.nodeStatus == NodeStatus.Success) {
				//If Node Succeeded
				childNode.OnEnd(blackboard); //Call nodes OnEnd Method
				nodeStatus = NodeStatus.Success; //Set Status to Success to inform parent node
			} else if(childNode.nodeStatus == NodeStatus.Running) {
				//If Node is Running
				childNode.Update(blackboard); //Update Node
			} else if(childNode.nodeStatus == NodeStatus.Ready) {
				//If Node is Ready
				if(!childNode.IsValid(blackboard)) {
					//If Node is NOT Valid
					nodeStatus = NodeStatus.Success; //Set Status to Success to inform parent node
					return; //Exit Update Call
				}

				childNode.OnStart(blackboard); //Call nodes OnStart Method
				childNode.nodeStatus = NodeStatus.Running; //Set Nodes status to Running to start updating it
			}
		}

		public override void OnEnd(Blackboard blackboard) {}

	}

}