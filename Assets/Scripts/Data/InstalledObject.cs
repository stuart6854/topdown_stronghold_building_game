using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstalledObject : WorldObject {

	private Tile Tile;

	public InstalledObject(Tile tile, string objectType) {
		this.Tile = tile;
		base.ObjectType = objectType;
	}

}
