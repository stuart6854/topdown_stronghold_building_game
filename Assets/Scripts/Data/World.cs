using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class World {

	//Data
	private int Width, Height;
	private Tile[,] Tiles;
    private List<Character> Characters;

	//Callbacks
	private Action<WorldObject> OnWorldObjectCreated;
	private Action<WorldObject> OnWorldObjectChanged;
	private Action<WorldObject> OnWorldObjectDestroyed;

	public World(int width, int height) {
		this.Width = width;
		this.Height = height;
	    this.Characters = new List<Character>();
	}

	public void InitialiseWorld() {
		Tiles = new Tile[Width, Height];

		for(int x = 0; x < Width; x++) {
			for(int y = 0; y < Height; y++) {
				Tiles[x, y] = new Tile(x, y, "grass", this);
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
	    InstalledObject instance = (InstalledObject)Defs.GetDef(type).Properties.CreateAssemblyClassInstance();
        tile.PlaceInstalledObject(type, instance);
    }

	public void DemolishInstalledObject(Tile tile) {
		InstalledObject io = tile.GetInstalledObject();
		tile.RemoveInstalledObject();
		Dictionary<string, int> drops = io.GetDismantledDrops(io.GetObjectType());
		WorldController.Instance.StartCoroutine(DropLooseItems(tile, drops));
	}

	public void DestroyInstalledObject(Tile tile) {
		InstalledObject io = tile.GetInstalledObject();
		tile.RemoveInstalledObject();
		Dictionary<string, int> drops = io.GetDestroyedDrops(io.GetObjectType());
		WorldController.Instance.StartCoroutine(DropLooseItems(tile, drops));
	}

    public void PlaceCharacter(Tile tile) {
        Character character = new Character(tile);
		character.RegOnCreatedCB(OnWorldObjectCreated);
		character.RegOnUpdateCB(OnWorldObjectChanged);
		character.RegOnDestroyedCB(OnWorldObjectDestroyed);
		Characters.Add(character);
        SpriteController.Instance.OnWorldObjectCreated(character);
    }

	private IEnumerator DropLooseItems(Tile initialTile, Dictionary<string, int> drops) {
		Queue<Tile> QueuedTiles = new Queue<Tile>();
		QueuedTiles.Enqueue(initialTile);

		List<Tile> ProcessedTiled = new List<Tile>();

		while(drops.Count > 0 && QueuedTiles.Count > 0 ) {
			Tile tile = QueuedTiles.Dequeue();
			if(tile == null)
				continue;

			if(ProcessedTiled.Contains(tile))
				continue;

			InstalledObject io = tile.GetInstalledObject();
			if(io != null && io.GetMovementMultiplier() == 0f)
				continue;

			if(tile.GetLooseItem() == null) {
				KeyValuePair<string, int> pair = drops.First();
				string type = pair.Key;
				int amnt = pair.Value;

				tile.PlaceLooseItem(new LooseItem(type, amnt));
				drops.Remove(type);
			} else {
				LooseItem looseItem = tile.GetLooseItem();
				foreach(KeyValuePair<string, int> pair in drops) {
					string type = pair.Key;
					int amnt = pair.Value;

					if(looseItem.GetObjectType() == type) {
						looseItem.AddToStack(amnt);
						drops.Remove(type);
						break;
					}
				}
			}

			//Get Neighbours
			foreach(Tile neighbourTile in tile.GetNeighbourTiles()) {
				if(ProcessedTiled.Contains(neighbourTile))
					continue;

				QueuedTiles.Enqueue(neighbourTile);
			}

			ProcessedTiled.Add(tile);

			yield return null;
		}

		yield return null;
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
