using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum JobTypes {
	//NOTE: Keep in order of importance (Left = More Important)
	None, MoveItem, Construct, Dismantle, Attack, 
}

public class Job {

	//TODO: Add a way for tasks to tell job that they have failed

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
	private Dictionary<string, int> requirements;

	public Job(JobTypes _jobType, Dictionary<string, int> _requirements, Tile _tile, long _creatorID, short _priority = 0) {
		jobType = _jobType;
		tile = _tile;
		creatorID = _creatorID;
		priority = _priority;

		tasks = new List<Task>();
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
		_task.job = this;
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

	protected void SetRequirements(Dictionary<string, int> _requirements) {
		this.requirements = _requirements;
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

	public Dictionary<string, int> GetRequirements() {
		return requirements;
	}

	#endregion

	//TODO: Stop duplicate jobs being put into job list

	public override bool Equals(object obj) {
		Job other = obj as Job;
		if(other == null)
			return false;

		if(this.tile != other.tile)
			return false;
		if(this.assignedCharacter != other.assignedCharacter)
			return false;
		if(this.tasks != other.tasks)
			return false;
		if(this.creatorID != other.creatorID)
			return false;
		if(this.hasFailedBefore != other.hasFailedBefore)
			return false;
		if(this.priority != other.priority)
			return false;
		if(this.jobType != other.jobType)
			return false;

		return true;
	}

}
