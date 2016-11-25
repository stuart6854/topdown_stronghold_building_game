using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InstalledObject : WorldObject, IContextMenu {

	private Tile Tile;

//	public static InstalledObject CreatePrototype(string type, WorldObjectMethod methods, float movementCost = 1, bool connectToNeighbours = false) {
//		InstalledObject io = new InstalledObject();
//		io.ObjectType = type;
//		io.WorldObjectType = WorldObjectType.InstalledObject;
//		io.Methods = methods;
//		io.MovementCost = movementCost;
//		io.ConnectsToNeighbours = connectToNeighbours;
//
//		return io;
//	}

	public InstalledObject PlaceInstance(Tile tile) {
//		InstalledObject io = new InstalledObject();
//		io.ObjectType = this.ObjectType;
//		io.WorldObjectType = this.WorldObjectType;
//		io.Tile = tile;
//		io.X = tile.GetX();
//		io.Y = tile.GetY();
//		io.MovementCost = this.MovementCost;
////		io.ConnectsToNeighbours = this.ConnectsToNeighbours;
//		io.OnCreatedCB = this.OnCreatedCB;
//		io.OnChangedCB = this.OnChangedCB;
//		io.OnDestroyedCB = this.OnDestroyedCB;
//		io.Methods = this.Methods;
//	    io.Parameters = new Dictionary<string, object>();
//
//	    io.Methods.OnCreated(io);

//		return io;

		return null;
	}

	public abstract void OnCreated();

	public abstract void OnUpdate();

	public abstract void OnDestroyed();

	public virtual void OnDismantled() {}

	public abstract Dictionary<string, int> GetConstructionRequirements();

	public Tile GetTile() {
		return Tile;
	}

	public virtual BuildMethod GetBuildMethod() {
		return BuildMethod.Single; // Default. Can be overriden.
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
