using BehaviourTrees;
using UnityEngine;

public class MoveToDest : LeafNode {

    public override bool IsValid(Blackboard blackboard) {
        return true;
    }

	public override void OnStart(Blackboard blackboard) {
		Character character = (Character)blackboard.getTreeMemoryLocation("Character");

		if(character.GetCurrentTile() == character.GetDestinationTile()) {
			nodeStatus = NodeStatus.Success;
			character.SetPath(null);
//			Debug.Log("At dest.");
			return;
		}
	}

    public override void Update(Blackboard blackboard) {
        Character character = (Character)blackboard.getTreeMemoryLocation("Character");
//	    Tile curr = character.GetCurrentTile();
//	    Tile dest = character.GetDestinationTile();
//		Debug.Log("Moving - " + curr.GetX() + ", " + curr.GetY() + " | " + dest.GetX() + ", " + dest.GetY());
		character.Rotate();
		character.Move();

		if(character.GetCurrentTile() == character.GetDestinationTile()) {
            nodeStatus = NodeStatus.Success;
			character.SetPath(null);
//			Debug.Log("At dest.");
            return;
        }
    }

    public override void OnEnd(Blackboard blackboard) {  }

}
