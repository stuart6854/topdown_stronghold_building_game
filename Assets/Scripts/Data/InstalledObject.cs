using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstalledObject : WorldObject, IContextMenu {

	private Tile Tile;

	private bool ConnectsToNeighbours;

	public static InstalledObject CreatePrototype(string type, WorldObjectMethod methods, float movementCost = 1, bool connectToNeighbours = false) {
		InstalledObject io = new InstalledObject();
		io.ObjectType = type;
		io.WorldObjectType = WorldObjectType.InstalledObject;
		io.Methods = methods;
		io.MovementCost = movementCost;
		io.ConnectsToNeighbours = connectToNeighbours;

		return io;
	}

	public InstalledObject PlaceInstance(Tile tile) {
		InstalledObject io = new InstalledObject();
		io.ObjectType = this.ObjectType;
		io.WorldObjectType = this.WorldObjectType;
		io.Tile = tile;
		io.X = tile.GetX();
		io.Y = tile.GetY();
		io.MovementCost = this.MovementCost;
		io.ConnectsToNeighbours = this.ConnectsToNeighbours;
		io.OnCreated = this.OnCreated;
		io.OnChanged = this.OnChanged;
		io.OnDestroyed = this.OnDestroyed;
		io.Methods = this.Methods;
	    io.Parameters = new Dictionary<string, object>();

	    io.Methods.OnCreated(io);

		return io;
	}

	public override void OnUpdate() {
		if(Methods != null)
			Methods.OnUpdate(this);
	}

	public Tile GetTile() {
		return Tile;
	}

	public Enterabilty GetEnterability() {
		if(MovementCost == 0)
			return Enterabilty.Never;

		else return Methods.GetEnterabilty(this);
	}

	public bool DoesConnectToNeighbours() {
		return ConnectsToNeighbours;
	}

    public override float GetZ() {
        return -0.1f;
    }

	public RadialMenuGenerator.RadialMenuItem[] MenuOptions_ContextMenu() {
		List<RadialMenuGenerator.RadialMenuItem> MenuItems = new List<RadialMenuGenerator.RadialMenuItem>();

		Font font = Resources.GetBuiltinResource<Font>("Arial.ttf");

		MenuItems.Add(new RadialMenuGenerator.RadialMenuItem("Dismantle", font, () => WorldController.Instance.GetWorld().DemolishInstalledObject(this.Tile)));
		MenuItems.Add(new RadialMenuGenerator.RadialMenuItem("Cancel", font, null));

		return MenuItems.ToArray();
	}

}
