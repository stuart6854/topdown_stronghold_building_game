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


            if(CurrentPath == null)
                return; //Still waiting on our path

            if(CurrentTile == CurrentJob.GetTile()) {
                CurrentJob.DoJob(Time.deltaTime);
            }
        }
    }

    private void Move() {
        if(CurrentTile == DestinationTile)
            return;

        if(NextTile.GetEnterabilty() == Enterabilty.Soon)
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

        Tile[] path = PathfindingController.Instance.RequestPath(CurrentTile, DestinationTile);
        if(path == null || path.Length == 0 || path[path.Length - 1] != DestinationTile)  {
            JobController.Instance.AddJob(CurrentJob);
            CurrentJob = null;
            JobSearchCooldown = UnityEngine.Random.Range(0.1f, 0.5f);
            DestinationTile = NextTile = CurrentTile;
            return;
        }

        CurrentPath = path;
        PathIndex = 0;
        NextTile = CurrentPath[0];
    }

    private void OnJobComplete(Job job) {
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
