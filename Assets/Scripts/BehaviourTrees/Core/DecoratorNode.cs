namespace BehaviourTrees {

	/// <summary>
	/// Decorator Nodes are used to either transform their child's return status,
	/// terminate their child or repeat their child behaviour multiple times
	/// </summary>
	public abstract class DecoratorNode : BehaviourNode {

		protected BehaviourNode childNode;

		public abstract override bool IsValid(Blackboard blackboard);

		public abstract override void OnStart(Blackboard blackboard);

		public abstract override void Update(Blackboard blackboard);

		public abstract override void OnEnd(Blackboard blackboard);

		public override void ResetNode() {
			nodeStatus = NodeStatus.Ready;

			childNode.ResetNode();
		}

		public void SetChildNode(BehaviourNode node) {
			this.childNode = node;
		}

		public bool equals(object obj) {
			bool equals = Equals(obj);
			DecoratorNode node = (DecoratorNode) obj;

			if(childNode.Equals(node)) return false;

			return equals;
		}

	}

}