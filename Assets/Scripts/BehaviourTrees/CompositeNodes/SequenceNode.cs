namespace BehaviourTrees {

	/// <summary>
	/// Sequence Nodes are used to linearly call each child nodes.
	/// If the child node returns a SUCCESS, it will move onto the next node.
	/// If it successfuly calls each child node the Sequence Node will signal a SUCCESS status to its parent node.
	/// If the child node fails the Sequence Node will signal a FAILURE status the its parent node.
	/// </summary>
	public class SequenceNode : CompositeNode {

		private int currentNodeIndex;

		public override bool IsValid(Blackboard blackboard) {
			if(childNodes == null) return false;
			if(childNodes.Count == 0) return false;

			return true;
		}

		public override void OnStart(Blackboard blackboard) {}

		public override void Update(Blackboard blackboard) {
			BehaviourNode node = childNodes[currentNodeIndex]; //Get Current Node

			if(node.nodeStatus == NodeStatus.Failure) {
				//If Node Failed
				nodeStatus = NodeStatus.Failure; //Set Status to Failure to inform parent node
				node.OnEnd(blackboard); //Call nodes OnEnd Method
			} else if(node.nodeStatus == NodeStatus.Success) {
				//If Node Succeeded
				node.OnEnd(blackboard); //Call nodes OnEnd Method

				currentNodeIndex++; //Increment Current Child Node Index
				if(currentNodeIndex >= childNodes.Count)
					nodeStatus = NodeStatus.Success; //Set Status to Success to inform parent node
			} else if(node.nodeStatus == NodeStatus.Running) {
				//If Node is Running
				node.Update(blackboard); //Update Node
			} else if(node.nodeStatus == NodeStatus.Ready) {
				//If Node is Ready
				if(!node.IsValid(blackboard)) {
					//If Node is NOT Valid
					nodeStatus = NodeStatus.Failure; //Set Status to Failure to inform parent node
					return; //Exit Update Call
				}

				node.nodeStatus = NodeStatus.Running; //Set Nodes status to Running to start updating it
				node.OnStart(blackboard); //Call nodes OnStart Method
			}
		}

		public override void OnEnd(Blackboard blackboard) {}

		public override void ResetNode() {
			currentNodeIndex = 0;
			base.ResetNode();
		}

	}

}