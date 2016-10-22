using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstalledObject : WorldObject {

	private Tile Tile;

	private WorldObjectMethod methods;

	public static InstalledObject CreatePrototype(string type, WorldObjectMethod methods) {
		InstalledObject io = new InstalledObject();
		io.ObjectType = type;
		io.WorldObjectType = WorldObjectType.InstalledObject;
		io.methods = methods;

		return io;
	}

	public InstalledObject PlaceInstance(Tile tile) {
		InstalledObject io = new InstalledObject();
		io.ObjectType = this.ObjectType;
		io.WorldObjectType = this.WorldObjectType;
		io.Tile = tile;
		io.X = tile.GetX();
		io.Y = tile.GetY();
		io.OnCreated = this.OnCreated;
		io.OnChanged = this.OnChanged;
		io.methods = this.methods;

		return io;
	}

}
