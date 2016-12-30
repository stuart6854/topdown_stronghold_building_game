using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_MoveTo : Task {

	private Tile Destination;

	public Task_MoveTo(Tile _destination) : base() {
		this.Destination = _destination;
	}

	public override void OnStart() {
		base.OnStart();
	}

	public override void OnUpdate() {
		if(assignedCharacter.GetDestinationTile() != Destination) {
			assignedCharacter.SetDestination(Destination);
		}
	}

	public override void OnEnd() {
		base.OnEnd();
	}

	public override bool IsCompleted() {
		return (assignedCharacter.GetCurrentTile() == Destination);
	}

	public void SetDestination(Tile _destination) {
		this.Destination = _destination;
	}

}
