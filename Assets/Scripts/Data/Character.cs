using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Character {

    //References
    private Tile CurrentTile, DestinationTile;
    private float PercentageBetweenTiles;

    private Job CurrentJob;

    //Data
    private float Rotation;
    private const float MoveSpeed = 3.0f;
    private const float LookSpeed = 5.0f;

    private float JobSearchCooldown;

    //Callbacks
    private Action<Character> OnChanged;


    public Character(Tile tile) {
        this.CurrentTile = this.DestinationTile = tile;
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
                DestinationTile = CurrentTile;
                return;
            }
            return;
        }

        if(CurrentTile == CurrentJob.GetTile()) {
            CurrentJob.DoJob(Time.deltaTime);
        }
    }

    private void Move() {
        if(DestinationTile == CurrentTile)
            return;

        float x = Mathf.Pow(CurrentTile.GetX() - DestinationTile.GetX(), 2);
        float y = Mathf.Pow(CurrentTile.GetY() - DestinationTile.GetY(), 2);
        float distToTravel = Mathf.Sqrt(x + y);

        float distThisFrame = MoveSpeed * Time.deltaTime;

        float percentageThisFrame = distThisFrame / distToTravel;

        PercentageBetweenTiles += percentageThisFrame;

        if(PercentageBetweenTiles >= 1.0f) {
            CurrentTile = DestinationTile;
            PercentageBetweenTiles = 0;
        }
    }

    private void Rotate() {
        if(DestinationTile == CurrentTile)
            return;

        Vector2 vecToDest = new Vector2(DestinationTile.GetX() - CurrentTile.GetX(), DestinationTile.GetY() - CurrentTile.GetY());
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
        //TODO: Try and get path at this point to check if job is accessible
    }

    private void OnJobComplete(Job job) {
        CurrentJob = null;
        DestinationTile = CurrentTile;
        JobSearchCooldown = UnityEngine.Random.Range(0.1f, 0.5f);
    }

    public float GetX() {
        return Mathf.Lerp(CurrentTile.GetX(), DestinationTile.GetX(), PercentageBetweenTiles);
    }

    public float GetY() {
        return Mathf.Lerp(CurrentTile.GetY(), DestinationTile.GetY(), PercentageBetweenTiles);
    }

    public float GetZ() {
        return 0.0f;
    }

    public float GetRotation() {
        return Rotation;
    }

    public Job GetCurrentJob() {
        return CurrentJob;
    }

}
