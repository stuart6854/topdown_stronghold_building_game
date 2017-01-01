using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Task {

	//References
	public Job job;
	protected Character assignedCharacter;

	//Properties
	private bool hasStarted;
	private bool hasEnded;

	protected Task() {

	}

	public void AssignCharacter(Character _character) {
		assignedCharacter = _character;
	}

	public virtual void OnStart() {
		hasStarted = true;
	}

	public abstract void OnUpdate();

	public virtual void OnEnd() {
		hasEnded = true;
	}

	public bool HasStarted() {
		return hasStarted;
	}

	public abstract bool IsCompleted();

	public bool HasEnded() {
		return hasEnded;
	}

}
