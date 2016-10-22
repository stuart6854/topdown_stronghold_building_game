using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstalledObject : WorldObject {

	private Tile Tile;

	private bool ConnectsToNeighbours;

	public static InstalledObject CreatePrototype(string type, WorldObjectMethod methods, float movementCost = 1, bool connectToNeighbours = false) {
		InstalledObject io = new InstalledObject();
		io.ObjectType = type;
		io.WorldObjectType = WorldObjectType.InstalledObject;
		io.methods = methods;
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
		io.methods = this.methods;

		return io;
	}

	public override void OnUpdate() {
		if(methods != null)
			methods.OnUpdate(this);
	}

	public Tile GetTile() {
		return Tile;
	}

	public Enterabilty GetEnterability() {
		if(MovementCost == 0)
			return Enterabilty.Never;

		else return methods.GetEnterabilty(this);
	}

	public bool GetConnectToNeighbours() {
		return ConnectsToNeighbours;
	}

}
