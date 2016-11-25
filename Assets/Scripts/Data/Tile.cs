using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : WorldObject {

	//References
	private World World;

	private InstalledObject InstalledObject;
	private LooseItem LooseItem;

	//Data
	protected float MovementCost = 1.0f; // Multiplier

	public Tile(int x, int y, string objectType, World world) {
		this.X = x;
		this.Y = y;
//		this.WorldObjectType = WorldObjectType.Tile;
//		this.ObjectType = objectType;
		this.World = world;

//		if(OnCreatedCB != null)
//			OnCreatedCB(this);
	}

	public void OnUpdate() {
		if(InstalledObject != null)
			InstalledObject.OnUpdate();
	}

//	public InstalledObject PlaceInstalledObject(InstalledObject prototype) {
//		if(this.InstalledObject != null)
//			return null;
//
//		this.InstalledObject = prototype.PlaceInstance(this);
//		this.InstalledObject.RegisterOnCreatedCallback(this.OnCreatedCB);
//		this.InstalledObject.RegisterOnChangedCallback(this.OnChangedCB);
//		this.InstalledObject.RegisterOnDestroyedCallback(this.OnDestroyedCB);
//
//		if(this.InstalledObject.GetConnectsToNeighbours()) {
//			foreach(Tile tile in GetNeighbourTiles()) {
//				if(tile.InstalledObject == null)
//					continue;
//
//				if(tile.InstalledObject.GetObjectType() != this.InstalledObject.GetObjectType())
//					continue;
//
//				if(tile.InstalledObject.GetOnChanged() != null)
//					tile.InstalledObject.GetOnChanged()(tile.InstalledObject);
//			}
//		}
//
//		if(this.InstalledObject.GetOnCreated() != null)
//			this.InstalledObject.GetOnCreated()(this.InstalledObject);
//
//	    if(OnChangedCB != null)
//	        OnChangedCB(this);
//
//	    return this.InstalledObject;
//	}

//	public void RemoveInstalledObject() {
//		if(this.InstalledObject == null)
//			return;
//
//		InstalledObject io = this.InstalledObject;
//		this.InstalledObject = null;
//
//		if(io.GetOnDestroyed() != null)
//			io.GetOnDestroyed()(io);
//
//		if(io.GetConnectsToNeighbours()) {
//			foreach(Tile tile in GetNeighbourTiles()) {
//				if(tile.InstalledObject == null)
//					continue;
//
//				if(tile.InstalledObject.GetObjectType() != io.GetObjectType())
//					continue;
//
//				if(tile.InstalledObject.GetOnChanged() != null)
//					tile.InstalledObject.GetOnChanged()(tile.InstalledObject);
//			}
//		}
//
//		if(OnChangedCB != null)
//			OnChangedCB(this);
//	}

//    public LooseItem PlaceLooseItem(LooseItem looseItem) {
//        if(looseItem == null) {
//	        if(this.LooseItem != null)
//				if(this.LooseItem.GetOnDestroyed() != null)
//					this.LooseItem.GetOnDestroyed()(this.LooseItem);
//
//            this.LooseItem = null;
//            return null;
//        }
//
//        if(this.MovementCost == 0)
//            return null;
//
//        if(this.InstalledObject != null && this.InstalledObject.GetMovementCost() == 0)
//            return null;
//
//        if(this.LooseItem != null) {
//            //TODO: Try combine stack
//
//            return this.LooseItem;
//        }
//
//        this.LooseItem = looseItem.Clone();
//        this.LooseItem.SetTile(this);
//		this.LooseItem.RegisterOnCreatedCallback(this.OnCreatedCB);
//		this.LooseItem.RegisterOnChangedCallback(this.OnChangedCB);
//		this.LooseItem.RegisterOnDestroyedCallback(this.OnDestroyedCB);
//
//		looseItem.SetStackSize(0); //Incase we are passed a reference
//		
//        if(OnCreatedCB != null)
//            OnCreatedCB(this.LooseItem);
//
//        return this.LooseItem;
//    }

//	public void ChangeType(string type) {
//		if(type == ObjectType)
//			return; //We are already this type
//
//		this.ObjectType = type;
//
//	    PlaceLooseItem(new LooseItem("stone", 5)); //TODO: Temporary! For testing looseitems and job requirements
//
//	    if(OnChangedCB != null)
//			OnChangedCB(this);
//	}

	public InstalledObject GetInstalledObject() {
		return InstalledObject;
	}

	public LooseItem GetLooseItem() {
		return LooseItem;
	}

	public Tile[] GetNeighbourTiles() {
		List<Tile> neighbours = new List<Tile>();
		for(int x = (int)X - 1; x <= X + 1; x++) {
			for(int y = (int)Y - 1; y <= Y + 1; y++) {

				if(x == this.X && y == this.Y)
					continue;

				Tile tile = World.GetTile(x, y);
				if(tile == null)
					continue;

				neighbours.Add(tile);
			}
		}

		return neighbours.ToArray();
	}

	public float GetMovementCost() {
		return MovementCost;
	}

	public Enterabilty GetEnterability() {
		if(MovementCost == 0)
			return Enterabilty.Never;

		if(InstalledObject != null)
			return InstalledObject.GetEnterability();

		return Enterabilty.Enterable;
	}

}
