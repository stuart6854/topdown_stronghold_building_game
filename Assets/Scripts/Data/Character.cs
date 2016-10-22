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
    private const float Speed = 3.0f;


    //Callbacks
    private Action<Character> OnChanged;


    public Character(Tile tile) {
        this.CurrentTile = this.DestinationTile = tile;
        this.OnChanged += CharacterSpriteController.Instance.OnCharacterChanged;

        this.DestinationTile = WorldController.Instance.GetTileAt(0, 0);
    }

    public void OnUpdate() {
        Move();
        Rotate();

        if(OnChanged != null)
            OnChanged(this);
    }

    private void Move() {
        if(DestinationTile == CurrentTile)
            return;

        float x = Mathf.Pow(CurrentTile.GetX() - DestinationTile.GetX(), 2);
        float y = Mathf.Pow(CurrentTile.GetY() - DestinationTile.GetY(), 2);
        float distToTravel = Mathf.Sqrt(x + y);

        float distThisFrame = Speed * Time.deltaTime;

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
        Rotation = angle;
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
