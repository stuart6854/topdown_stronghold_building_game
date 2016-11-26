using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldController : MonoBehaviour {

	public static WorldController Instance;

    public const int WIDTH = 100, HEIGHT = 100;

	private World world;

	void Awake() {
		if(!ModManager.ModsLoaded)
			ModManager.LoadMods();
		Defs.LoadDefs(ModManager.Mods.ToArray());

		Instance = this;
	}
	
	void Start () {
	    world = new World(WIDTH, HEIGHT);
	    world.RegisterOnWorldObjectCreatedCallback(SpriteController.Instance.OnWorldObjectCreated);
	    world.RegisterOnWorldObjectChangedCallback(SpriteController.Instance.OnWorldObjectChanged);
	    world.RegisterOnWorldObjectDestroyedCallback(SpriteController.Instance.OnWorldObjectDestroyed);
	    world.InitialiseWorld();
	}
	
	void Update () {
		world.OnUpdate();
	}

	public World GetWorld() {
		return world;
	}

	public Tile GetTileAt(int x, int y) {
		return world.GetTile(x, y);
	}

}
