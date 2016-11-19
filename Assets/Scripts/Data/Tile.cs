using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : WorldObject {

	//References
	private World world;

	private InstalledObject InstalledObject;
	private LooseItem LooseItem;

    private Job PendingJob;

	//Data

	
	public Tile(int x, int y, string objectType, World world) {
		this.X = x;
		this.Y = y;
		this.Z = 0.0f;
		this.WorldObjectType = WorldObjectType.Tile;
		this.ObjectType = objectType;
		this.world = world;

		if(OnCreatedCB != null)
			OnCreatedCB(this);
	}

	public override void OnCreated() {
		throw new System.NotImplementedException();
	}

	public override void OnUpdate() {
		if(Methods != null)
			Methods.OnUpdate(this);

		if(InstalledObject != null)
			InstalledObject.OnUpdate();
	}

	public override void OnDestroyed() {
		throw new System.NotImplementedException();
	}

	public InstalledObject PlaceInstalledObject(InstalledObject prototype) {
		if(this.InstalledObject != null)
			return null;
		
//		this.InstalledObject = prototype.PlaceInstance(this);
		this.InstalledObject.RegisterOnCreatedCallback(this.OnCreatedCB);
		this.InstalledObject.RegisterOnChangedCallback(this.OnChangedCB);
		this.InstalledObject.RegisterOnDestroyedCallback(this.OnDestroyedCB);

		if(this.InstalledObject.GetConnectsToNeighbours()) {
			foreach(Tile tile in GetNeighbourTiles()) {
				if(tile.InstalledObject == null)
					continue;

				if(tile.InstalledObject.GetObjectType() != this.InstalledObject.GetObjectType())
					continue;

				if(tile.InstalledObject.GetOnChanged() != null)
					tile.InstalledObject.GetOnChanged()(tile.InstalledObject);
			}
		}

		if(this.InstalledObject.GetOnCreated() != null)
			this.InstalledObject.GetOnCreated()(this.InstalledObject);

	    if(OnChangedCB != null)
	        OnChangedCB(this);

	    return this.InstalledObject;
	}

	public void RemoveInstalledObject() {
		if(this.InstalledObject == null)
			return;

		InstalledObject io = this.InstalledObject;
		this.InstalledObject = null;

		if(io.GetOnDestroyed() != null)
			io.GetOnDestroyed()(io);

		if(io.GetConnectsToNeighbours()) {
			foreach(Tile tile in GetNeighbourTiles()) {
				if(tile.InstalledObject == null)
					continue;

				if(tile.InstalledObject.GetObjectType() != io.GetObjectType())
					continue;

				if(tile.InstalledObject.GetOnChanged() != null)
					tile.InstalledObject.GetOnChanged()(tile.InstalledObject);
			}
		}

		if(OnChangedCB != null)
			OnChangedCB(this);
	}

    public LooseItem PlaceLooseItem(LooseItem looseItem) {
        if(looseItem == null) {
	        if(this.LooseItem != null)
				if(this.LooseItem.GetOnDestroyed() != null)
					this.LooseItem.GetOnDestroyed()(this.LooseItem);

            this.LooseItem = null;
            return null;
        }

        if(this.MovementCost == 0)
            return null;

        if(this.InstalledObject != null && this.InstalledObject.GetMovementCost() == 0)
            return null;

        if(this.LooseItem != null) {
            //TODO: Try combine stack

            return this.LooseItem;
        }

        this.LooseItem = looseItem.Clone();
        this.LooseItem.SetTile(this);
		this.LooseItem.RegisterOnCreatedCallback(this.OnCreatedCB);
		this.LooseItem.RegisterOnChangedCallback(this.OnChangedCB);
		this.LooseItem.RegisterOnDestroyedCallback(this.OnDestroyedCB);

		looseItem.SetStackSize(0); //Incase we are passed a reference
		
        if(OnCreatedCB != null)
            OnCreatedCB(this.LooseItem);

        return this.LooseItem;
    }

	public void ChangeType(string type) {
		if(type == ObjectType)
			return; //We are already this type

		this.ObjectType = type;

	    PlaceLooseItem(new LooseItem("stone", 5)); //TODO: Temporary! For testing looseitems and job requirements

	    if(OnChangedCB != null)
			OnChangedCB(this);
	}

	public InstalledObject GetInstalledObject() {
		return InstalledObject;
	}

	public LooseItem GetLooseItem() {
		return LooseItem;
	}

    public bool SetPendingJob(Job job) {
        if(this.PendingJob != null) {
            return false;
        }

        this.PendingJob = job;
        this.PendingJob.RegisterOnCompleteCallback(OnJobComplete);
        this.PendingJob.RegisterOnAbortedCallback(OnJobAborted);

        return true;
    }

    public Job GetPendingJob() {
        return PendingJob;
    }

	public Tile[] GetNeighbourTiles() {
		List<Tile> neighbours = new List<Tile>();
		for(int x = (int)X - 1; x <= X + 1; x++) {
			for(int y = (int)Y - 1; y <= Y + 1; y++) {

				if(x == this.X && y == this.Y)
					continue;

				Tile tile = world.GetTile(x, y);
				if(tile == null)
					continue;

				neighbours.Add(tile);
			}
		}

		return neighbours.ToArray();
	}

	public Enterabilty GetEnterabilty() {
		if(MovementCost == 0)
			return Enterabilty.Never;

		if(InstalledObject != null)
			return InstalledObject.GetEnterability();

		return Enterabilty.Enterable;
	}

    void OnJobComplete(Job job) {
        this.PendingJob = null;
    }

    void OnJobAborted(Job job) {
        this.PendingJob = null;
    }

}
