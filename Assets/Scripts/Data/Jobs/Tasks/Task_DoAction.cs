using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Task_DoAction : Task {

	//References

	
	//Properties
	private bool doneAction;
	private Action action;

	public Task_DoAction(Action _action) : base() {
		action = _action;
	}

	public override void OnStart() {
		base.OnStart();
	}

	public override void OnUpdate() {
		if(!doneAction) {
			action();
			doneAction = true;
		}
	}

	public override void OnEnd() {
		base.OnEnd();
	}

	public override bool IsCompleted() {
		return doneAction;
	}

}
