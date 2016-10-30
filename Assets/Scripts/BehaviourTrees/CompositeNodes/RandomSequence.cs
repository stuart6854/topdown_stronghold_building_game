namespace BehaviourTrees {

	/// <summary>
	/// Random Sequence Nodes do the same as normal Sequence Nodes,
	/// except their child nodes are randomised before running.
	/// Thus, running the child nodes in a randomised order.
	/// </summary>
	public class RandomSequence : SequenceNode {

		public override void OnStart(Blackboard blackboard) {
			childNodes.Shuffle();
		}

	}

}