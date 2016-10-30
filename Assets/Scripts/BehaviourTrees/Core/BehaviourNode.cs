namespace BehaviourTrees {

	public abstract class BehaviourNode {

		public NodeStatus nodeStatus = NodeStatus.Ready;

		public int Priority = 1;

		public abstract bool IsValid(Blackboard blackboard);

		public abstract void OnStart(Blackboard blackboard);

		public abstract void Update(Blackboard blackboard);

		public abstract void OnEnd(Blackboard blackboard);

		public abstract void ResetNode();

		public override bool Equals(object obj) {
			BehaviourNode node = (BehaviourNode) obj;

			if(node == null)
				return false;

			if(node.Priority != this.Priority) 
				return false;

			return true;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

	}

}