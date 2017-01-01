using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_DoAction : Task {

	//References

	
	//Properties
	private bool doneAction;
	private float waitTime;
	private Action action;

	private float timePassed;

	public Task_DoAction(Action _action, float _waitTime = 0f) : base() {
		action = _action;
		waitTime = _waitTime;
	}

	public override void OnStart() {
		base.OnStart();
	}

	public override void OnUpdate() {
		if(!doneAction && timePassed >= waitTime) {
			action();
			doneAction = true;
		}

		timePassed += Time.deltaTime;
	}

	public override void OnEnd() {
		base.OnEnd();
	}

	public override bool IsCompleted() {
		return doneAction;
	}

}
