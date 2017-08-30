using System.Collections.Generic;
using UnityEngine;

public class Character : WorldObject{

    //TODO: Add LooseItem Inventory(separate class or built into this class?)

    //References
    private Tile CurrentTile, NextTile, DestinationTile;
    private float PercentageBetweenTiles;

	private Job currentJob;
    private List<Tile> currentPath;
    private int PathIndex;

    //Data
    private const float MoveSpeed = 3.0f;
    private const float LookSpeed = 5.0f;

    private float JobSearchCooldown;

    private Inventory Inventory;

	private AnimHandler Animation;

    public Character(Tile tile) : base() {
        this.ObjectType = "character";
		this.WorldObjectType = WorldObjectType.Character;
        this.CurrentTile = this.NextTile = this.DestinationTile = tile;
        this.Inventory = new Inventory(4);

		this.Animation = new AnimHandler("character_idle_front_anim", this);

        this.OnUpdateCB += SpriteController.Instance.OnWorldObjectChanged;
    }

	public void OnUpdate() {
		if(currentJob == null) 
			TryGetJob();

		if(currentJob != null) {
			currentJob.OnUpdate();
			if(currentJob.IsComplete()) {
				currentJob.OnEnd();
				currentJob = null;
			}
		}

		Move();

//		Animation.TickAnim(Time.deltaTime);

        if(OnUpdateCB != null)
			OnUpdateCB(this);
    }

    public void Move() {
        if(CurrentTile == DestinationTile)
            return;

        if(NextTile.GetEnterability() == Enterabilty.Soon)
            return;

        if(currentPath == null)
            return;

        float x = Mathf.Pow(CurrentTile.GetX() - NextTile.GetX(), 2);
        float y = Mathf.Pow(CurrentTile.GetY() - NextTile.GetY(), 2);
        float distToTravel = Mathf.Sqrt(x + y);

        float distThisFrame = (MoveSpeed * CurrentTile.GetMovementMultiplier()) * Time.deltaTime;

        //Apply MovementCost
        if(PercentageBetweenTiles < 0.5f)
            distThisFrame *= CurrentTile.GetMovementMultiplier();
        else
            distThisFrame *= NextTile.GetMovementMultiplier();

        float percentageThisFrame = distThisFrame / distToTravel;

        PercentageBetweenTiles += percentageThisFrame;

        if(PercentageBetweenTiles >= 1.0f) {
            CurrentTile = NextTile;
            PercentageBetweenTiles = 0;

	        if(currentPath.Count > 0) 
		        currentPath.RemoveAt(0);

		    if(currentPath.Count > 0) //Path still has Tiles
			    NextTile = currentPath[0];
        }
    }

//    public void Rotate() {
//	    Tile targetTile = NextTile;
//		if(CurrentTile == NextTile && CurrentTile == DestinationTile)
//			return;
//
//	    if(CurrentTile == NextTile && CurrentTile != DestinationTile)
//		    targetTile = DestinationTile;
//
//
//        Vector2 vecToDest = new Vector2(targetTile.GetX() - CurrentTile.GetX(), targetTile.GetY() - CurrentTile.GetY());
//        float angle = Mathf.Atan2(vecToDest.y, vecToDest.x) * Mathf.Rad2Deg;
//        Rotation = Mathf.LerpAngle(Rotation, angle, Time.deltaTime * LookSpeed);
//    }

	private bool TryGetPath() {
		List<Tile> path = PathfindingController.Instance.RequestPath(this.CurrentTile, this.DestinationTile);
		if(path == null || path.Count == 0) {
			return false;
		}

		SetPath(path);
		return true;
	}

	public void SetPath(List<Tile> path) {
		currentPath = path;
		PathIndex = 0;
		NextTile = (currentPath != null ? currentPath[PathIndex] : CurrentTile);
		DestinationTile = (currentPath != null ? currentPath[currentPath.Count - 1] : CurrentTile);
	}

	public bool TryGetJob() {
	    currentJob = JobHandler.GetBestJob(this);
        if(currentJob == null)
            return false;

//		currentJob.RegisterOnCompleteCallback(OnJobComplete);
//		currentJob.RegisterOnAbortedCallback(OnJobComplete);
		
		currentJob.AssignCharacter(this);
		currentJob.OnStart();

	    return true;
    }

	/// <summary>
	/// 
	/// </summary>
	/// <param name="amnt">The amount the character is to try pick up.</param>
	/// <returns>The amount picked up.</returns>
	public int PickupLooseItem(int amnt) {
		if(CurrentTile.GetLooseItem() == null)
			return 0;

		LooseItem req = CurrentTile.GetLooseItem();
		int amntTaken = req.RemoveFromStack(amnt);

		Inventory.Add(new LooseItem(req.GetObjectType(), amntTaken));
		return amntTaken;
	}

    public void AbandonJob() {
		//		JobController.Instance.AddFailedJob(CurrentJob);
		currentJob.OnEnd ();
		currentJob = null;
        currentPath = null;
		DestinationTile = NextTile = CurrentTile;
    }

    public void JobComplete(Job job) {
        foreach(KeyValuePair<string,int> pair in job.GetRequirements()) {
            Inventory.Remove(pair.Key, pair.Value); //Remove the jobs required items. NOTE:They shouldn't have disapeared since we started the job
            //In the future we may want required items to be used up during the job instead of afterwards
        }

        currentJob.OnEnd();
		currentJob = null;
	    currentPath = null;
        DestinationTile = NextTile = CurrentTile;
        JobSearchCooldown = Random.Range(0.1f, 0.5f);
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

	public override string GetSpriteName() {
		return "character_sprite";
	}

	public Inventory GetInventory() {
        return Inventory;
    }

    public Tile GetCurrentTile() {
        return CurrentTile;
    }

    public Tile GetDestinationTile() {
        return DestinationTile;
    }

	public void SetDestination(Tile tile) {
		this.DestinationTile = tile;
		if(this.DestinationTile != null) {
			if(!TryGetPath()) {
				this.DestinationTile = null;
				//Couldnt find path!
			}
		}
	}

    public List<Tile> GetCurrentPath() {
        return currentPath;
    }

    public Job GetCurrentJob() {
        return currentJob;
    }

}
