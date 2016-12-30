using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JobTypes {
	//NOTE: Keep in order of importance (Left = More Important)
	None, MoveItem, Construct, Dismantle, Attack, 
}

public class Job {

	//References
	private Tile tile;
	private Character assignedCharacter;
	private List<Task> tasks;

	//Identifying Properties
	private long creatorID; // WorldObject ID
	private bool hasFailedBefore; // Has a character failed to do this job before
	private short priority; // Higher = More Important
	private JobTypes jobType;
	
	//Practical Properties
	private bool hasStarted;

	public Job(JobTypes _jobType, Tile _tile, short _priority = 0) {
		this.jobType = _jobType;
		this.tile = _tile;
		this.priority = _priority;

		this.tasks = new List<Task>();
	}

	public void AssignCharacter(Character _character) {
		this.assignedCharacter = _character;
		foreach(Task task in tasks) {
			task.AssignCharacter(_character);
		}
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

	#region Getters

	public Tile GetTile() {
		return this.tile;
	}

	public long GetCreatorID() {
		return this.creatorID;
	}

	public bool GetHasFailedBefore() {
		return hasFailedBefore;
	}

	public short GetPriority() {
		return this.priority;
	}

	public JobTypes GetJobType() {
		return this.jobType;
	}

	#endregion

}
