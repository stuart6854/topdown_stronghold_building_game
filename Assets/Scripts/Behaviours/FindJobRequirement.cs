using System.Collections.Generic;
using BehaviourTrees;
using UnityEngine;

public class FindJobRequirement : LeafNode {

    public override bool IsValid(Blackboard blackboard) {
        return true;
    }

    public override void OnStart(Blackboard blackboard) {
        Character character = (Character)blackboard.getTreeMemoryLocation("Character");

        KeyValuePair<string, int> requirement = character.GetUnfulfilledJobRequirement();

//		Debug.Log("Looking for requirement: " + requirement.Key + " | " + requirement.Value);

	    Tile[] path = PathfindingController.Instance.RequestPathToObject(character.GetCurrentTile(), requirement.Key);
        if(path == null) {
            //We could find one of the requirements so this job is unable to be completed just now.
            //So lets abandon and requeue the job
            character.AbandonJob();
            this.nodeStatus = NodeStatus.Failure;
//			Debug.Log("Didnt find job requirement. Abandoned Job");
			return;
        }
        //We have found a tile that contains something that we need.
        //Lets go get it.
        character.SetCurrentJobRequirement(requirement.Key);
        character.SetPath(path);
//		Debug.Log("Found job requirement.");
        this.nodeStatus = NodeStatus.Success;
    }

    public override void Update(Blackboard blackboard) {  }

    public override void OnEnd(Blackboard blackboard) {  }

}