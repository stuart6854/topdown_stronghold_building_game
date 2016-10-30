using System.Collections.Generic;
using BehaviourTrees;
using UnityEngine;

public class PickupRequirement : LeafNode {

    public override bool IsValid(Blackboard blackboard) {
        return true;
    }

    public override void OnStart(Blackboard blackboard) {
        Character character = (Character)blackboard.getTreeMemoryLocation("Character");

        Tile currTile = character.GetCurrentTile();

        Dictionary<string, int> JobRequirements = character.GetJobRequirements();
        string currJobReq = character.GetCurrentJobRequirement();

        LooseItem item = currTile.GetLooseItem();
        int amnt = item.GetStackSize();
        if(amnt < JobRequirements[currJobReq]) {
            character.GetInventory().Add(new LooseItem(currJobReq, amnt));
            currTile.GetLooseItem().RemoveFromStack(amnt);
            character.GetJobRequirements()[currJobReq] -= amnt;

            character.SetCurrentJobRequirement(null);
            character.SetPath(null); //Resets us to try find more of requirement as we have not fullfilled the required amount
        } else {
            //We can fulfill the whole requirement here
            character.GetInventory().Add(new LooseItem(currJobReq, JobRequirements[currJobReq]));
            currTile.GetLooseItem().RemoveFromStack(JobRequirements[currJobReq]);
            JobRequirements.Remove(currJobReq); //We dont require any more of this type

            character.SetPath(null);
            character.SetCurrentJobRequirement(null);
        }

//		Debug.Log("Picked up requirements");
        this.nodeStatus = NodeStatus.Success;
    }

    public override void Update(Blackboard blackboard) {  }

    public override void OnEnd(Blackboard blackboard) {  }

}
