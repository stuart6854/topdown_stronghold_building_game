using System.Collections.Generic;
using BehaviourTrees;
using UnityEngine;

public class GetJobLeaf : LeafNode {

	private float JobSearchCooldown;

	public override bool IsValid(Blackboard blackboard) {
		return true;
	}

	public override void OnStart(Blackboard blackboard) {  }

	public override void Update(Blackboard blackboard) {
		Character character = (Character)blackboard.getTreeMemoryLocation("Character");
		
		JobSearchCooldown -= Time.deltaTime;
		if(JobSearchCooldown >= 0) {
//			Debug.Log("Job Search Cooldown: " + JobSearchCooldown);
			return;
		}
		
		if(character.GetJob()) {
			this.nodeStatus = NodeStatus.Success;
			JobSearchCooldown = UnityEngine.Random.Range(0.1f, 0.5f);
//			Debug.Log("Got job.");
		} else {
			this.nodeStatus = NodeStatus.Failure;
//			Debug.Log("Didnt get job.");
		}
	}

	public override void OnEnd(Blackboard blackboard) { }

}
