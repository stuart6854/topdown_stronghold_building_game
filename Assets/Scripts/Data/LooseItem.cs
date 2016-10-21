using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseItem : WorldObject {

	private Tile Tile;

	public LooseItem(Tile tile, string objectType) {
		this.Tile = tile;
		base.ObjectType = objectType;
	}

}
