using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character {

    //References
    private Tile CurrentTile, NextTile, DestinationTile;
    private float PercentageBetweenTiles;

    private Job CurrentJob;
    private Tile[] CurrentPath;
    private int PathIndex;

    //Data
    private float Rotation;
    private const float MoveSpeed = 3.0f;
    private const float LookSpeed = 5.0f;

    private float JobSearchCooldown;

    //Callbacks
    private Action<Character> OnChanged;


    public Character(Tile tile) {
        this.CurrentTile = this.NextTile = this.DestinationTile = tile;
        this.OnChanged += CharacterSpriteController.Instance.OnCharacterChanged;

        this.DestinationTile = WorldController.Instance.GetTileAt(0, 0);
    }

    public void OnUpdate() {
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
            return;
        }

        if(CurrentPath == null)
            return; //Still waiting on our path

        if(CurrentTile == CurrentJob.GetTile()) {
            CurrentJob.DoJob(Time.deltaTime);
        }
    }

    private void Move() {
        if(CurrentTile == DestinationTile)
            return;

        float x = Mathf.Pow(CurrentTile.GetX() - NextTile.GetX(), 2);
        float y = Mathf.Pow(CurrentTile.GetY() - NextTile.GetY(), 2);
        float distToTravel = Mathf.Sqrt(x + y);

        float distThisFrame = MoveSpeed * Time.deltaTime;

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
        PathfindingController.Instance.RequestPath(CurrentTile, DestinationTile, OnPathRecieved);
    }

    private void OnPathRecieved(Tile[] path, bool success) {
        if(!success || path.Length == 0 || path[path.Length - 1] != DestinationTile) {
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

    public float GetX() {
        return Mathf.Lerp(CurrentTile.GetX(), NextTile.GetX(), PercentageBetweenTiles);
    }

    public float GetY() {
        return Mathf.Lerp(CurrentTile.GetY(), NextTile.GetY(), PercentageBetweenTiles);
    }

    public float GetZ() {
        return -0.1f;
    }

    public float GetRotation() {
        return Rotation;
    }

    public Job GetCurrentJob() {
        return CurrentJob;
    }

}
