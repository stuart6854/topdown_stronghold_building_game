using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

	public static WorldController Instance;

	private World world;

	void Awake() {
		Instance = this;
	}
	
	void Start () {
		world = new World(100, 100);
		world.RegisterOnWorldObjectCreatedCallback(SpriteController.Instance.OnWorldObjectCreated);
		world.RegisterOnWorldObjectChangedCallback(SpriteController.Instance.OnWorldObjectChanged);
		world.InitialiseWorld();
	}
	
	void Update () {
		
	}

	public World GetWorld() {
		return world;
	}

	public Tile GetTileAt(int x, int y) {
		return world.GetTile(x, y);
	}

}
