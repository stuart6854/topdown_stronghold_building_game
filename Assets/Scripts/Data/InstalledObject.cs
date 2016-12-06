using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InstalledObject : Constructable, IContextMenu{

	private Tile Tile;

	public InstalledObject PlaceInstance(string objectType, Tile tile) {
		this.ObjectType = objectType;
		this.WorldObjectType = WorldObjectType.InstalledObject;
		this.Tile = tile;
		this.X = tile.GetX();
		this.Y = tile.GetY();

		this.OnCreatedCB(this);

		return this;
	}

	public abstract void OnCreated();

	public abstract void OnUpdate();

	public abstract void OnDestroyed();

	public virtual void OnDismantled() {}

	public Tile GetTile() {
		return Tile;
	}

	public int GetMovementMultiplier() {
		string val = Defs.GetDef(this.ObjectType).Properties.GetValue("MovementMultiplier");
		return int.Parse(val);
	}

	public virtual Enterabilty GetEnterability() {
		return Enterabilty.Never; // Default. Can be overriden.
	}

	public virtual bool GetConnectsToNeighbours() {
		return false;
	}

	public override float GetZ() {
        return -0.1f;
    }

	public abstract RadialMenuItem[] MenuOptions_ContextMenu();
	
}
