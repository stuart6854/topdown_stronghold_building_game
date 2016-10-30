using System.Collections;
using System.Collections.Generic;
using BehaviourTrees;
using UnityEngine;

public class CharacterBehaviourRoot : SelectorNode {

	public override void Update(Blackboard blackboard) {
		base.Update(blackboard);

		if(base.nodeStatus == NodeStatus.Success || base.nodeStatus == NodeStatus.Failure) {
//			Debug.Log("RESETTING BEHAVIOUR TREE!");
			base.ResetNode();
		}
	}

	public override void ResetNode() {
//		Debug.Log("RESETTING BEHAVIOUR TREE!");
		base.ResetNode();
	}

}
