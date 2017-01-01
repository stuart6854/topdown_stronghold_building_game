using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_GetRequirement : Task {

	private string req_Name;
	private int req_Amount;

	private Tile req_Tile; //A tile containing the requirement

	public Task_GetRequirement(string _reqName, int _reqAmount) : base() {
		req_Name = _reqName;
		req_Amount = _reqAmount;
	}

	public override void OnUpdate() {
		if(req_Tile == null)
			req_Tile = FindRequirement();

		if(req_Tile == null)
			return; //TODO: Tell job we have failed!

		if(assignedCharacter.GetCurrentTile() == req_Tile) {
			//Pickup requirement
			req_Amount -= assignedCharacter.PickupLooseItem(req_Amount);
			return;
		}

		if(assignedCharacter.GetDestinationTile() != req_Tile)
			assignedCharacter.SetDestination(req_Tile);
	}

	public override bool IsCompleted() {
		return assignedCharacter.GetInventory().Contains(req_Name, req_Amount);

	}

	//Utility Methods

	private Tile FindRequirement() {
		return PathfindingController.Instance.FindAccessibleObject(assignedCharacter.GetCurrentTile(), req_Name);
	}

}
