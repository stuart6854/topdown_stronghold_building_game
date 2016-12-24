using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : Constructable, ITooltip {

	//References
	private World World;

	private InstalledObject InstalledObject;
	private LooseItem LooseItem;

	public Tile(int x, int y, string type, World world) : base() {
		this.X = x;
		this.Y = y;
		this.ObjectType = type;
		this.World = world;
		this.WorldObjectType = WorldObjectType.Tile;
	}

	public void OnUpdate() {
		if(InstalledObject != null)
			InstalledObject.OnUpdate();
	}

	public InstalledObject PlaceInstalledObject(string type, InstalledObject instance, bool baseInstance = true) {
		if(this.InstalledObject != null)
			return null;

		if(baseInstance) {
			instance.RegOnCreatedCB(this.OnCreatedCB);
			instance.RegOnUpdateCB(this.OnUpdateCB);
			instance.RegOnDestroyedCB(this.OnDestroyedCB);
			instance.OnCreated();
		}

		this.InstalledObject = instance.PlaceInstance(type, this, baseInstance);

		if(this.InstalledObject.GetConnectsToNeighbours()) {
			foreach(Tile tile in GetNeighbourTiles()) {
				if(tile.InstalledObject == null)
					continue;

				if(tile.InstalledObject.GetObjectType() != this.InstalledObject.GetObjectType())
					continue;

				if(tile.InstalledObject.GetOnUpdatedCB() != null)
					tile.InstalledObject.GetOnUpdatedCB()(tile.InstalledObject);
			}
		}

	    if(OnUpdateCB != null)
			OnUpdateCB(this);

	    return this.InstalledObject;
	}

	public void RemoveInstalledObject() {
		if(this.InstalledObject == null)
			return;

		InstalledObject io = this.InstalledObject;
		this.InstalledObject = null;

		io.OnDestroyed();

		if(io.GetOnDestroyedCB() != null)
			io.GetOnDestroyedCB()(io);

		if(io.GetConnectsToNeighbours()) {
			foreach(Tile tile in GetNeighbourTiles()) {
				if(tile.InstalledObject == null)
					continue;

				if(tile.InstalledObject.GetObjectType() != io.GetObjectType())
					continue;

				if(tile.InstalledObject.GetOnUpdatedCB() != null)
					tile.InstalledObject.GetOnUpdatedCB()(tile.InstalledObject);
			}
		}

		if(OnUpdateCB != null)
			OnUpdateCB(this);
	}

    public LooseItem PlaceLooseItem(LooseItem looseItem) {
        if(looseItem == null) {
	        if(this.LooseItem != null)
				if(this.LooseItem.GetOnDestroyedCB() != null)
					this.LooseItem.GetOnDestroyedCB()(this.LooseItem);

            this.LooseItem = null;
            return null;
        }

        if(this.GetMovementMultiplier() == 0)
            return null;

        if(this.InstalledObject != null && this.InstalledObject.GetMovementMultiplier() == 0)
            return null;

        if(this.LooseItem != null) {
            //TODO: Try combine stack

            return this.LooseItem;
        }

        this.LooseItem = looseItem.Clone();
        this.LooseItem.SetTile(this);
		this.LooseItem.RegOnCreatedCB(this.OnCreatedCB);
		this.LooseItem.RegOnUpdateCB(this.OnUpdateCB);
		this.LooseItem.RegOnDestroyedCB(this.OnDestroyedCB);

		looseItem.SetStackSize(0); //Incase we are passed a reference
		
        if(OnCreatedCB != null)
            OnCreatedCB(this.LooseItem);

        return this.LooseItem;
    }

	public void ChangeType(string type) {
		if(type == ObjectType)
			return; //We are already this type

		this.ObjectType = type;

	    if(OnUpdateCB != null)
			OnUpdateCB(this);
	}

	public override string GetSpriteName() {
		return "tile_" + ObjectType;
	}

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

	public float GetMovementMultiplier() {
		string val = Defs.GetDef(this.ObjectType).Properties.GetValue("MovementMultiplier");
		return float.Parse(val);
	}

	public Enterabilty GetEnterability() {
		if(GetMovementMultiplier() == 0)
			return Enterabilty.Never;

		if(InstalledObject != null)
			return InstalledObject.GetEnterability();

		return Enterabilty.Enterable;
	}

	public Dictionary<string, int> GetConstructionRequirements() {
		return null;
	}

	public Dictionary<string, int> GetDismantledDrops() {
		return null;
	}

	public string Tooltip_GetTitle() {
		return this.ObjectType;
	}

	public string Tooltip_GetBodyText() {
		return "Tile Body Text.";
	}

}
