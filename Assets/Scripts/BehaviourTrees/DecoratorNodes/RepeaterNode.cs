namespace BehaviourTrees {

	/// <summary>
	/// Repeater Nodes will re-run their child node each time it returns a result.	
	/// Note: Repeater Nodes can optionally run their child a set number of times.
	/// </summary>
	public class RepeaterNode : DecoratorNode {

		private int RepeatCounter; //Current Repeat Count

		private int RepeatNum = -1; //Amount of Times to Repeat Child Node - (<= 0 means repeat infinitely)

		public RepeaterNode(int repeatCount) {
			RepeatNum = repeatCount;
		}

		public RepeaterNode() {}

		public override bool IsValid(Blackboard blackboard) {
			return true;
		}

		public override void OnStart(Blackboard blackboard) {}

		public override void Update(Blackboard blackboard) {
			if(childNode.nodeStatus == NodeStatus.Failure) {
				//If Node Failed
				childNode.OnEnd(blackboard); //Call nodes OnEnd Method
				childNode.nodeStatus = NodeStatus.Ready;
				RepeatCounter++;
				return; //Exit Update Call
			}
			if(childNode.nodeStatus == NodeStatus.Success) {
				//If Node Succeeded
				childNode.OnEnd(blackboard); //Call nodes OnEnd Method
				childNode.nodeStatus = NodeStatus.Ready;
				RepeatCounter++;
				return; //Exit Update Call
			}
			if(childNode.nodeStatus == NodeStatus.Running) {
				//If Node is Running
				childNode.Update(blackboard); //Update Node
			} else if(childNode.nodeStatus == NodeStatus.Ready) {
				//If Node is Ready
				if(!childNode.IsValid(blackboard)) return; //Exit Update Call

				childNode.OnStart(blackboard); //Call nodes OnStart Method
				childNode.nodeStatus = NodeStatus.Running; //Set Nodes status to Running to start updating it
			}

			if(RepeatNum > 0)
				if(RepeatCounter == RepeatNum) nodeStatus = NodeStatus.Success;
		}

		public override void OnEnd(Blackboard blackboard) {}

	}

}