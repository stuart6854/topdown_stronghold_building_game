using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Job {

	//References
	private Character assignedCharacter;
	public List<Task> tasks;

	//Properties
	private bool hasStarted;

	public Job() {
		this.tasks = new List<Task>();
	}

	public void AssignCharacter(Character _character) {
		this.assignedCharacter = _character;
	}

	public void OnStart() {
		hasStarted = true;
	}

	public void OnUpdate() {
		foreach(Task task in tasks) {
			if(task.IsCompleted()) //Don't run this task if we (still) meet completion conditions
				continue;

			if(!task.HasStarted()) //If this is the first time
				task.OnStart(); 

			task.OnUpdate(); //Update Task

			if(task.IsCompleted() && !task.HasEnded()) //If task is complete & it hasnt ended yet
				task.OnEnd();

			break; //Stop here. We must complete a task before moving to the next one
		}
	}

	public void OnEnd() {
		
	}

	public void AddTask(Task _task) {
		_task.AssignCharacter(assignedCharacter);
		tasks.Add(_task);
	}

	public bool HasStarted() {
		return hasStarted;
	}

	public bool IsComplete() {
		foreach(Task task in tasks) {
			if(!task.IsCompleted())
				return false;
		}

		return true;
	}

}
