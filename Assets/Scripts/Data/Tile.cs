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

		if(OnCreated != null)
			OnCreated(this);
	}

	public override void OnUpdate() {
		if(Methods != null)
			Methods.OnUpdate(this);

		if(InstalledObject != null)
			InstalledObject.OnUpdate();
	}

	public InstalledObject PlaceInstalledObject(InstalledObject prototype) {
		if(this.InstalledObject != null)
			return null;
		
		this.InstalledObject = prototype.PlaceInstance(this);
		this.InstalledObject.RegisterOnCreatedCallback(this.OnCreated);
		this.InstalledObject.RegisterOnChangedCallback(this.OnChanged);
		this.InstalledObject.RegisterOnDestroyedCallback(this.OnDestroyed);

		if(this.InstalledObject.DoesConnectToNeighbours()) {
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

	    if(OnChanged != null)
	        OnChanged(this);

	    return this.InstalledObject;
	}

	public void RemoveInstalledObject() {
		if(this.InstalledObject == null)
			return;

		InstalledObject io = this.InstalledObject;
		this.InstalledObject = null;

		if(io.GetOnDestroyed() != null)
			io.GetOnDestroyed()(io);

		if(io.DoesConnectToNeighbours()) {
			foreach(Tile tile in GetNeighbourTiles()) {
				if(tile.InstalledObject == null)
					continue;

				if(tile.InstalledObject.GetObjectType() != io.GetObjectType())
					continue;

				if(tile.InstalledObject.GetOnChanged() != null)
					tile.InstalledObject.GetOnChanged()(tile.InstalledObject);
			}
		}

		if(OnChanged != null)
			OnChanged(this);
	}

    public LooseItem PlaceLooseItem(LooseItem looseItem) {
        if(looseItem == null) {
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
        looseItem.SetStackSize(0); //Incase we are passed a reference

        if(OnCreated != null)
            OnCreated(this.LooseItem);

        return this.LooseItem;
    }

	public void ChangeType(string type) {
		if(type == ObjectType)
			return; //We are already this type

		this.ObjectType = type;

	    PlaceLooseItem(new LooseItem("stone", 5)); //TODO: Temporary! For testing looseitems and job requirements

	    if(OnChanged != null)
			OnChanged(this);
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
