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

//	List<RadialMenuGenerator.RadialMenuItem> MenuItems = new List<RadialMenuGenerator.RadialMenuItem>();
//
//		Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");
//
//		MenuItems.Add(new RadialMenuGenerator.RadialMenuItem("Dismantle", font, this.OnDismantled));
//		MenuItems.Add(new RadialMenuGenerator.RadialMenuItem("Cancel", font, null)); //TODO: Allow player to cancel current order, eg. Cancel Order
//
//		return MenuItems.ToArray();
	public abstract RadialMenuGenerator.RadialMenuItem[] MenuOptions_ContextMenu();
	
}
