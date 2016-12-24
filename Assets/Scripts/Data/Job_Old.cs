using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public enum JobType {
	None, Construct, Dismantle, Mine
}

public class Job_Old : IComparable<Job_Old> {

	private long CreatorID; // Should contain the ID of a WorldObject

    private readonly int Priority;
	private readonly JobType JobType;
	private float TimeCreated;

    private Tile JobTile;

    private Dictionary<string, int> Requirements;

    private float CompletionTime;
    private float PassedTime;

    private Action<Job_Old> OnComplete;
    private Action<Job_Old> OnAborted;

    public Job_Old(JobType type, Tile jobTile, Action<Job_Old> JobAction, Dictionary<string, int> requirements, float completionTime = -1, int priority = 0) {
	    this.JobType = type;
        this.JobTile = jobTile;
        this.Requirements = requirements;
        this.CompletionTime = completionTime;
        this.Priority = priority;
        ResetTimeCreated();

        RegisterOnCompleteCallback(JobAction);
    }

    public bool DoJob(float deltaTime) {
        //NOTE: AI should check if they have the requirements before calling this
        PassedTime += deltaTime;

        if(PassedTime < CompletionTime)
            return false;

        //Job is complete
        if(OnComplete != null)
            OnComplete(this);

        return true;
    }

    public int GetPriority() {
        return Priority;
    }

	public JobType GetJobType() {
		return JobType;
	}

    public Tile GetTile() {
        return JobTile;
    }

    public Dictionary<string, int> GetRequirements() {
        return Requirements ?? new Dictionary<string, int>();//TODO: Might cause issues if dictionary is passed as reference
    }

    public float GetCompletionTime() {
        return CompletionTime;
    }

    public float GetPassedTime() {
        return PassedTime;
    }

    public bool IsComplete() {
        return (PassedTime >= CompletionTime);
    }

	public void ResetTimeCreated() {
		this.TimeCreated = Time.realtimeSinceStartup;
	}

    public void RegisterOnCompleteCallback(Action<Job_Old> callback) {
        OnComplete -= callback;
        OnComplete += callback;
    }

    public void UnregisterOnJobCompleteCallback(Action<Job_Old> callback) {
        OnComplete -= callback;
    }

    public void RegisterOnAbortedCallback(Action<Job_Old> callback) {
        OnAborted -= callback;
        OnAborted += callback;
    }

    public void UnregisterOnJobAbortedCallback(Action<Job_Old> callback) {
        OnAborted -= callback;
    }

    public int CompareTo(Job_Old other) {
        int result = other.Priority.CompareTo(this.Priority);
        if(result == 0)
            result = other.TimeCreated.CompareTo(this.TimeCreated);

        return result;
    }

    public override bool Equals(object obj) {
        Job_Old otherJob = (Job_Old) obj;
        if(otherJob == null)
            return false;

        if(otherJob.JobTile != this.JobTile)
            return false;

        if(otherJob.Priority != this.Priority)
            return false;

	    if(otherJob.JobType != this.JobType)
		    return false;

        if(otherJob.TimeCreated != this.TimeCreated)
            return false;

        return true;
    }

    public override int GetHashCode() {
	    int hash = 13;
	    hash = (hash * 7) + Priority.GetHashCode();
	    hash = (hash * 7) + JobType.GetHashCode();
	    hash = (hash * 7) + TimeCreated.GetHashCode();

	    return hash;
    }

}