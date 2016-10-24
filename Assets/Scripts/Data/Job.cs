using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

public class Job : IComparable<Job> {

    private int Priority;
    private float TimeCreated;

    private Tile Tile;

    private Dictionary<string, int> Requirements;

    private float CompletionTime;
    private float PassedTime;

    private Action<Job> OnComplete;
    private Action<Job> OnAborted;

    public Job(Tile tile, Action<Job> JobAction, Dictionary<string, int> requirements , float completionTime = -1, int priority = 0) {
        this.Tile = tile;
        this.Requirements = requirements;
        this.CompletionTime = completionTime;
        this.Priority = priority;
        this.TimeCreated = Time.realtimeSinceStartup;

        RegisterOnCompleteCallback(JobAction);
    }

    public void DoJob(float deltaTime) {
        //NOTE: AI should check if they have the requirements before calling this
        PassedTime += deltaTime;

        if(PassedTime < CompletionTime)
            return;

        //Job is complete

        //TODO: Remove the required objects from characters inventory

        if(OnComplete != null)
            OnComplete(this);
    }

    public int GetPriority() {
        return Priority;
    }

    public Tile GetTile() {
        return Tile;
    }

    public Dictionary<string, int> GetRequirements() {
        return Requirements;//TODO: Might cause issues if dictionary is passed as reference
    }

    public float GetCompletionTime() {
        return CompletionTime;
    }

    public float GetPassedTime() {
        return PassedTime;
    }

    public void RegisterOnCompleteCallback(Action<Job> callback) {
        OnComplete -= callback;
        OnComplete += callback;
    }

    public void UnregisterOnJobCompleteCallback(Action<Job> callback) {
        OnComplete -= callback;
    }

    public void RegisterOnAbortedCallback(Action<Job> callback) {
        OnAborted -= callback;
        OnAborted += callback;
    }

    public void UnregisterOnJobAbortedCallback(Action<Job> callback) {
        OnAborted -= callback;
    }

    public int CompareTo(Job other) {
        int result = other.Priority.CompareTo(this.Priority);
        if(result == 0)
            result = other.TimeCreated.CompareTo(this.TimeCreated);

        return result;
    }

    public override bool Equals(object obj) {
        Job otherJob = (Job) obj;
        if(otherJob == null)
            return false;

        if(otherJob.Tile != this.Tile)
            return false;

        if(otherJob.Priority != this.Priority)
            return false;

        if(otherJob.TimeCreated != this.TimeCreated)
            return false;

        return true;
    }

    public override int GetHashCode() {
        return base.GetHashCode();
    }

}