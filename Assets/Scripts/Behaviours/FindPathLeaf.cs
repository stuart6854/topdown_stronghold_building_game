using BehaviourTrees;
using UnityEngine;

public class FindPathLeaf : LeafNode {

    public override bool IsValid(Blackboard blackboard) {
        return true;
    }

    public override void OnStart(Blackboard blackboard) {
        Character character = (Character)blackboard.getTreeMemoryLocation("Character");

        Tile[] path = PathfindingController.Instance.RequestPath(character.GetCurrentTile(), character.GetDestinationTile());
        if(path == null) {
            character.AbandonJob();
            this.nodeStatus = NodeStatus.Failure;
//			Debug.Log("couldnt find path.");
			return;
        }

        character.SetPath(path);
//		Debug.Log("Found path.");
		this.nodeStatus = NodeStatus.Success;
    }

    public override void Update(Blackboard blackboard) {  }

    public override void OnEnd(Blackboard blackboard) {  }

}
