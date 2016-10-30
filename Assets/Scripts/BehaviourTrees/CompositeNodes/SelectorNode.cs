using UnityEngine;

namespace BehaviourTrees {

	/// <summary>
	/// Selector Nodes, similar to Sequence Nodes, are used to linearly call each of its child node.
	/// But the difference is that if one of its child nodes succeed, it will instantly signal a
    /// Success Status to its parent node ignoring the remaining child nodes.
    /// Also if a child node returns a failure status it will carry onto the next child node.
	/// </summary>
	public class SelectorNode : CompositeNode {

		protected int currentNodeIndex;

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
				node.OnEnd(blackboard); //Call nodes OnEnd Method

				currentNodeIndex++; //Increment Current Child Node Index
				if(currentNodeIndex >= childNodes.Count)
					nodeStatus = NodeStatus.Failure; //Set Status to Failure to inform parent node
			} else if(node.nodeStatus == NodeStatus.Success) {
				//If Node Succeeded
				nodeStatus = NodeStatus.Success; //Set Status to Success to inform parent node
				node.OnEnd(blackboard); //Call nodes OnEnd Method
			} else if(node.nodeStatus == NodeStatus.Running) {
				//If Node is Running
				node.Update(blackboard); //Update Node
			} else if(node.nodeStatus == NodeStatus.Ready) {
				//If Node is Ready
				if(!node.IsValid(blackboard)) {
					//If Node is NOT Valid
					node.nodeStatus = NodeStatus.Failure; //Set Nodes status to Failure
				} else {
					//If Node is valid
					node.nodeStatus = NodeStatus.Running; //Set Nodes status to Running to start updating it
					node.OnStart(blackboard); //Call nodes OnStart Method
				}
			}
		}

		public override void OnEnd(Blackboard blackboard) {}

		public override void ResetNode() {
			currentNodeIndex = 0;
			base.ResetNode();
		}

	}

}