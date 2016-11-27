using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {

	//Data
	private int Width, Height;
	private Tile[,] Tiles;
    private List<Character> Characters;

	private Dictionary<string, WorldObject> WorldObjectPrototypes;

	//Callbacks
	private Action<WorldObject> OnWorldObjectCreated;
	private Action<WorldObject> OnWorldObjectChanged;
	private Action<WorldObject> OnWorldObjectDestroyed;

	public World(int width, int height) {
		this.Width = width;
		this.Height = height;
		this.WorldObjectPrototypes = new Dictionary<string, WorldObject>();
	    this.Characters = new List<Character>();
	}

	private void LoadInstalledObjectPrototypes() {
		foreach(KeyValuePair<string, Definition> installedObjectDef in Defs.InstalledObjectDefs) {
			Definition def = installedObjectDef.Value;
			Type type = def.GetType();
			InstalledObject io = (InstalledObject)def.CreateInstance();
			if(io == null)
				Debug.LogError("World::LoadInstalledObjectPrototypes -> Definition Instance created is Null!");
			WorldObjectPrototypes.Add(def.DefName, io);
		}
	}

	public void InitialiseWorld() {
		LoadInstalledObjectPrototypes();

		Tiles = new Tile[Width, Height];

		for(int x = 0; x < Width; x++) {
			for(int y = 0; y < Height; y++) {
				//Tile Constructior Params example
				//Activator.CreateInstance(type, constructorParam1, constructorParam2);
				//


				Tiles[x, y] = new Tile(x, y, this);
				Tiles[x, y].ChangeType("grass");
				Tiles[x, y].RegOnCreatedCB(OnWorldObjectCreated);
				Tiles[x, y].RegOnUpdateCB(OnWorldObjectChanged);
				Tiles[x, y].RegOnDestroyedCB(OnWorldObjectDestroyed);
				OnWorldObjectCreated(Tiles[x, y]);
			}
		}
	}

	public void OnUpdate() {
		for(int x = 0; x < Width; x++) {
			for(int y = 0; y < Height; y++) {
				Tiles[x, y].OnUpdate();
			}
		}

	    foreach(Character character in Characters) {
	        character.OnUpdate();
	    }
	}

    public void PlaceInstalledObject(string type, Tile tile) {
	    InstalledObject prototype = (InstalledObject)Defs.GetIODef(type).CreateInstance();
        tile.PlaceInstalledObject(type, prototype);
    }

	public void DemolishInstalledObject(Tile tile) {
//		tile.RemoveInstalledObject();
	}

    public void PlaceCharacter(Tile tile) {
        Character character = new Character(tile);
        Characters.Add(character);
        SpriteController.Instance.OnWorldObjectCreated(character);
    }

    public Tile GetTile(int x, int y) {
		if(x < 0 || x >= Width)
			return null;

		if(y < 0 || y >= Height)
			return null;

		return Tiles[x, y];
	}

    public int GetWidth() {
        return Width;
    }

    public int GetHeight() {
        return Height;
    }

	public WorldObject GetWorldObjectPrototype(string name) {
		if(!WorldObjectPrototypes.ContainsKey(name)) {
			Debug.LogError("World::GetWorldObjectPrototype -> There is NO prototype for the object '" + name + "'");
			return null;
		}

		WorldObject wo_proto = WorldObjectPrototypes[name];
		if(wo_proto == null) {
			Debug.LogError("World::GetWorldObjectPrototype -> The prototype for object '" + name + "' is NULL!");
			return null;
		}

		return wo_proto;
	}

    public void RegOnWOCreatedCB(Action<WorldObject> callback) {
		OnWorldObjectCreated -= callback;
		OnWorldObjectCreated += callback;

        if(Tiles == null)
            return;

        for(int x = 0; x < Width; x++) {
            for(int y = 0; y < Height; y++) {
                Tiles[x, y].RegOnCreatedCB(callback);
            }
        }
    }

	public void DeregOnWOCreatedCB(Action<WorldObject> callback) {
		OnWorldObjectCreated -= callback;
	}

	public void RegOnWOUpdatedCB(Action<WorldObject> callback) {
		OnWorldObjectChanged -= callback;
		OnWorldObjectChanged += callback;

	    if(Tiles == null)
	        return;

	    for(int x = 0; x < Width; x++) {
	        for(int y = 0; y < Height; y++) {
	            Tiles[x, y].RegOnUpdateCB(callback);
	        }
	    }
	}

	public void DeregOnWOUpdatedCB(Action<WorldObject> callback) {
		OnWorldObjectChanged -= callback;
	}

	public void RegOnWODestroyedCB(Action<WorldObject> callback) {
		OnWorldObjectDestroyed -= callback;
		OnWorldObjectDestroyed += callback;

		if(Tiles == null)
			return;

		for(int x = 0; x < Width; x++) {
			for(int y = 0; y < Height; y++) {
				Tiles[x, y].RegOnDestroyedCB(callback);
			}
		}
	}

	public void DeregOnWODestroyedCB(Action<WorldObject> callback) {
		OnWorldObjectDestroyed -= callback;
	}

}
