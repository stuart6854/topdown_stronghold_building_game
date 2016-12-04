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
	    world.RegOnWOCreatedCB(SpriteController.Instance.OnWorldObjectCreated);
	    world.RegOnWOUpdatedCB(SpriteController.Instance.OnWorldObjectChanged);
	    world.RegOnWODestroyedCB(SpriteController.Instance.OnWorldObjectDestroyed);
	    world.InitialiseWorld();
	}
	
	void Update () {
		world.OnUpdate();
	}

	public World GetWorld() {
		return world;
	}

	public Tile GetTile(int x, int y) {
		return world.GetTile(x, y);
	}

	#region Static-Methods

	public static Tile GetTileAt(int x, int y) {
		return Instance.GetTile(x, y);
	}

	#endregion

}
