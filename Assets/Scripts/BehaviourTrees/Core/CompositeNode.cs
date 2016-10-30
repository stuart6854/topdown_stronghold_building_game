using System.Collections.Generic;

namespace BehaviourTrees {
	/// <summary>
	/// Composite Nodes are used to process each of their children
	/// either linearly or in a random order (Order could even be customly defined by the node).
	/// </summary>
	public abstract class CompositeNode : BehaviourNode {

		protected List<BehaviourNode> childNodes;

		public CompositeNode() {
			childNodes = new List<BehaviourNode>();
		}

		public abstract override bool IsValid(Blackboard blackboard);

		public abstract override void OnStart(Blackboard blackboard);

		public abstract override void Update(Blackboard blackboard);

		public abstract override void OnEnd(Blackboard blackboard);

		public override void ResetNode() {
			nodeStatus = NodeStatus.Ready;

			foreach(BehaviourNode behaviourNode in childNodes) behaviourNode.ResetNode();
		}

		public CompositeNode AddChild(BehaviourNode node) {
			this.childNodes.Add(node);
			return this;
		}

		public override bool Equals(object obj) {
			bool equals = base.Equals(obj);
			CompositeNode node = (CompositeNode) obj;

			if(childNodes.Equals(node.childNodes)) return false;

			return equals;
		}

		public override int GetHashCode() {
			return base.GetHashCode();
		}

	}

}