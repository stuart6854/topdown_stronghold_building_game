namespace BehaviourTrees {

	/// <summary>
	/// Leaf Nodes are used to carry Game Specific Tests or Actions
	/// </summary>
	public abstract class LeafNode : BehaviourNode {

		public abstract override bool IsValid(Blackboard blackboard);

		public abstract override void OnStart(Blackboard blackboard);

		public abstract override void Update(Blackboard blackboard);

		public abstract override void OnEnd(Blackboard blackboard);

		public override void ResetNode() {
			nodeStatus = NodeStatus.Ready;
		}

	}

}