using System;
using System.Collections.Generic;
using UnityEngine;

public class Character : WorldObject{

    //TODO: Add LooseItem Inventory(separate class or built into this class?)

    //References
    private Tile CurrentTile, NextTile, DestinationTile;
    private float PercentageBetweenTiles;

    private Job CurrentJob;
    private Tile[] CurrentPath;
    private int PathIndex;

    //Data
    private const float MoveSpeed = 3.0f;
    private const float LookSpeed = 5.0f;

    private float JobSearchCooldown;

    private Inventory Inventory;

    private Dictionary<string, int> JobRequirements;
    private string CurrentRequirement;


    public Character(Tile tile) {
        this.WorldObjectType = WorldObjectType.Character;
        this.ObjectType = "character";
        this.CurrentTile = this.NextTile = this.DestinationTile = tile;

        this.OnChanged += SpriteController.Instance.OnWorldObjectChanged;
        this.DestinationTile = WorldController.Instance.GetTileAt(0, 0);

        this.Inventory = new Inventory(4);
    }

    public override void OnUpdate() {
        UpdateJob();
        Move();
        Rotate();

        if(OnChanged != null)
            OnChanged(this);
    }

    private void UpdateJob() {
        if(CurrentJob == null) {
            JobSearchCooldown -= Time.deltaTime;
            if(JobSearchCooldown > 0)
                return;

            GetJob();

            if(CurrentJob == null) {
                JobSearchCooldown = UnityEngine.Random.Range(0.1f, 0.5f);
                DestinationTile = NextTile = CurrentTile;
                return;
            }

        } else {
            //TODO: Have required materials/resources
            if(!HasJobRequirements()) {
                if(CurrentPath == null) {
                    KeyValuePair<string, int> requirement = GetUnfulfilledJobRequirement();

                    Tile[] path = PathfindingController.Instance.RequestPathToObject(CurrentTile, requirement.Key);
                    if(path == null) {
                        //We could find one of the requirements so this job is unable to be completed just now.
                        //So lets abandon and requeue the job
                        Debug.Log("Couldnt find requirement! Abandoning Job!");
                        AbandonJob();
                        return;
                    }
                    //We have found a tile that contains something that we need.
                    //Lets go get it.
                    Debug.Log("Got path to requirement: " + requirement.Key);
                    Debug.Log("LooseItem has " + path[path.Length - 1].GetLooseItem().GetStackSize() + " " + path[path.Length - 1].GetLooseItem().GetObjectType());
                    CurrentRequirement = requirement.Key;
                    CurrentPath = path;
                    PathIndex = 0;
                    NextTile = CurrentPath[0];
                    DestinationTile = CurrentPath[CurrentPath.Length - 1];
                } else {
                    if(CurrentTile == DestinationTile) {
                        //We have reached a requirements location, hopefully.
                        //TODO: Pickup Requirement
                        Debug.Log("At Requirement!");
                        LooseItem item = CurrentTile.GetLooseItem();
                        int amnt = item.GetStackSize();
                        if(amnt < JobRequirements[CurrentRequirement]) {
                            Inventory.Add(new LooseItem(CurrentRequirement, amnt));
                            CurrentTile.GetLooseItem().RemoveFromStack(amnt);
                            JobRequirements[CurrentRequirement] -= amnt;
                            Debug.Log("Added " + amnt + " " + CurrentRequirement);
                            CurrentRequirement = null;
                            CurrentPath = null; //Resets us to try find more of requirement as we have not fullfilled the required amount
                        } else {
                            Debug.Log("CurrentRequirement: " + CurrentRequirement);
                            //We can fulfill the whole requirement here
                            Debug.Log("Added " + JobRequirements[CurrentRequirement] + " " + CurrentRequirement);
                            Inventory.Add(new LooseItem(CurrentRequirement, JobRequirements[CurrentRequirement]));
                            CurrentTile.GetLooseItem().RemoveFromStack(JobRequirements[CurrentRequirement]);
                            JobRequirements.Remove(CurrentRequirement); //We dont require any more of this type
                            CurrentPath = null;
                            CurrentRequirement = null;
                        }
                    }
                }

            } else {
                DestinationTile = CurrentJob.GetTile();

                if(CurrentPath == null) {
                    Tile[] path = PathfindingController.Instance.RequestPath(CurrentTile, DestinationTile);
                    if(path == null) {
                        AbandonJob();
                        return;
                    }

                    CurrentPath = path;
                    PathIndex = 0;
                    NextTile = CurrentPath[0];
                }else if(!PathfindingController.Instance.PathStillValid(CurrentPath, PathIndex)) {
                    AbandonJob();
                    return;
                }

                if(CurrentTile == CurrentJob.GetTile()) {
                    CurrentJob.DoJob(Time.deltaTime);
                }
            }

        }
    }

    private void Move() {
        if(CurrentTile == DestinationTile)
            return;

        if(NextTile.GetEnterabilty() == Enterabilty.Soon)
            return;

        if(CurrentPath == null)
            return;

        float x = Mathf.Pow(CurrentTile.GetX() - NextTile.GetX(), 2);
        float y = Mathf.Pow(CurrentTile.GetY() - NextTile.GetY(), 2);
        float distToTravel = Mathf.Sqrt(x + y);

        float distThisFrame = MoveSpeed * Time.deltaTime;

        //Apply MovementCost
        if(PercentageBetweenTiles < 0.5f)
            distThisFrame *= CurrentTile.GetMovementCost();
        else
            distThisFrame *= NextTile.GetMovementCost();

        float percentageThisFrame = distThisFrame / distToTravel;

        PercentageBetweenTiles += percentageThisFrame;

        if(PercentageBetweenTiles >= 1.0f) {
            PathIndex++;
            CurrentTile = NextTile;
            PercentageBetweenTiles = 0;
            if(PathIndex < CurrentPath.Length)
                NextTile = CurrentPath[PathIndex];
        }
    }

    private void Rotate() {
        if(CurrentTile == NextTile)
            return;

        Vector2 vecToDest = new Vector2(NextTile.GetX() - CurrentTile.GetX(), NextTile.GetY() - CurrentTile.GetY());
        float angle = Mathf.Atan2(vecToDest.y, vecToDest.x) * Mathf.Rad2Deg;
        Rotation = Mathf.LerpAngle(Rotation, angle, Time.deltaTime * LookSpeed);
    }

    private void GetJob() {
        CurrentJob = JobController.Instance.GetJob();
        if(CurrentJob == null)
            return;

        CurrentJob.RegisterOnCompleteCallback(OnJobComplete);
        CurrentJob.RegisterOnAbortedCallback(OnJobComplete);
        DestinationTile = CurrentJob.GetTile();

        //Check if job is accessible
        Tile[] path = PathfindingController.Instance.RequestPath(CurrentTile, DestinationTile);
        if(path == null || path.Length == 0 || path[path.Length - 1] != DestinationTile)
            AbandonJob();

        if(CurrentJob.GetRequirements() != null)
            JobRequirements = new Dictionary<string, int>(CurrentJob.GetRequirements());
    }

    private bool HasJobRequirements() {
        if(JobRequirements == null)
            return true;

        foreach(KeyValuePair<string,int> pair in JobRequirements) {
            if(!Inventory.Contains(pair.Key, pair.Value))
                return false;
        }

        return true;
    }

    private KeyValuePair<string,int> GetUnfulfilledJobRequirement() {
        foreach(KeyValuePair<string,int> pair in JobRequirements) {
            if(!Inventory.Contains(pair.Key, pair.Value))
                return pair;
        }

        return new KeyValuePair<string, int>();
    }

    private void AbandonJob() {
        JobController.Instance.AddJob(CurrentJob);
        CurrentJob = null;
        CurrentPath = null;
        JobSearchCooldown = UnityEngine.Random.Range(0.1f, 0.5f);
        DestinationTile = NextTile = CurrentTile;
    }

    private void OnJobComplete(Job job) {
        foreach(KeyValuePair<string,int> pair in job.GetRequirements()) {
            Inventory.Remove(pair.Key, pair.Value); //Remove the jobs required items. NOTE:They shouldn't have disapeared since we started the job
            //In the future we may want required items to be used up during the job instead of after
        }

        CurrentJob = null;
	    CurrentPath = null;
        DestinationTile = NextTile = CurrentTile;
        JobSearchCooldown = UnityEngine.Random.Range(0.1f, 0.5f);
    }

    public override float GetX() {
        return Mathf.Lerp(CurrentTile.GetX(), NextTile.GetX(), PercentageBetweenTiles);
    }

    public override float GetY() {
        return Mathf.Lerp(CurrentTile.GetY(), NextTile.GetY(), PercentageBetweenTiles);
    }

    public override float GetZ() {
        return -0.3f;
    }

    public Job GetCurrentJob() {
        return CurrentJob;
    }

}
