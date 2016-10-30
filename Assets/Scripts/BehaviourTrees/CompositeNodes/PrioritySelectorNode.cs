using System.Linq;

namespace BehaviourTrees {

	/// <summary>
	/// PrioritySelectorNode, similar to SelectorNodes, are used to linearly call each of its child nodes.
	/// Except before the PrioritySelectorNode starts calling its children, the child nodes are sorted in priority order(small to big).
	///</summary>
	public class PrioritySelectorNode : SelectorNode {

		private readonly bool IgnoreZeroPriorityResults;

		public PrioritySelectorNode(bool ignoreZeroPriorityResults) {
			IgnoreZeroPriorityResults = ignoreZeroPriorityResults;
		}

		public override void OnStart(Blackboard blackboard) {
			childNodes = childNodes.OrderBy(o => o.Priority).ToList();
		}

		public override void Update(Blackboard blackboard) {
			BehaviourNode node = childNodes[currentNodeIndex]; //Get Current Node

			if(node.nodeStatus == NodeStatus.Failure) {
//If Node Failed
				node.OnEnd(blackboard); //Call nodes OnEnd Method

				currentNodeIndex += 1; //Increment Current Child Node Index
				if(currentNodeIndex >= childNodes.Count)
					nodeStatus = NodeStatus.Failure; //Set Status to Failure to inform parent node
			} else if(node.nodeStatus == NodeStatus.Success) {
//If Node Succeeded
				if((IgnoreZeroPriorityResults && (node.Priority > 0)) || (currentNodeIndex + 1 == childNodes.Count)) {
//If nodes priortity < 0 or it is the last node
					nodeStatus = NodeStatus.Success; //Set Status to Success to inform parent node
				} else {
					currentNodeIndex++;
					if(currentNodeIndex >= childNodes.Count)
						nodeStatus = NodeStatus.Success; //Set Status to Failure to inform parent node
				}
				node.OnEnd(blackboard); //Call nodes OnEnd Method
			} else if(node.nodeStatus == NodeStatus.Running) {
//If Node is Running
				node.Update(blackboard); //Update Node
			} else if(node.nodeStatus == NodeStatus.Ready) {
//If Node is Ready
				if(!node.IsValid(blackboard)) {
//If Node is NOT Valid
					node.nodeStatus = NodeStatus.Failure; //Set Status to Failure to inform parent node
				} else {
					//If Node is valid
					node.nodeStatus = NodeStatus.Running; //Set Nodes status to Running to start updating it
					node.OnStart(blackboard); //Call nodes OnStart Method
				}
			}
		}

	}

}