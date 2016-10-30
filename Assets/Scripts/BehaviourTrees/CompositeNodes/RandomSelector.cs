namespace BehaviourTrees {

	/// <summary>
	/// Random Selector Nodes do the same as normal Selector Nodes,
	/// except their child nodes are randomised before running.
	/// Thus, running the child nodes in a randomised order.
	/// </summary>
	public class RandomSelector : SelectorNode {

		public override void OnStart(Blackboard blackboard) {
			childNodes.Shuffle();
		}

	}

}