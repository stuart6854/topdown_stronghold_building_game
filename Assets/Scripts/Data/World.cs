using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {

	//Data
	private int Width, Height;
	private Tile[,] Tiles;
    private List<Character> Characters;

	private Dictionary<string, InstalledObject> InstalledObjectPrototypes;

	//Callbacks
	private Action<WorldObject> OnWorldObjectCreated;
	private Action<WorldObject> OnWorldObjectChanged;

	public World(int width, int height) {
		this.Width = width;
		this.Height = height;
		this.InstalledObjectPrototypes = new Dictionary<string, InstalledObject>();
	    this.Characters = new List<Character>();
	}

	private void LoadInstalledObjectPrototypes() {
		InstalledObject io;

		io = InstalledObject.CreatePrototype("wall", new Wall(), 0, true);
		io.RegisterOnCreatedCallback(OnWorldObjectCreated);
		io.RegisterOnChangedCallback(OnWorldObjectChanged);
		InstalledObjectPrototypes.Add(io.GetObjectType(), io);

		io = InstalledObject.CreatePrototype("door", new Door(), 1, false);
		io.RegisterOnCreatedCallback(OnWorldObjectCreated);
		io.RegisterOnChangedCallback(OnWorldObjectChanged);
		InstalledObjectPrototypes.Add(io.GetObjectType(), io);
	}

	public void InitialiseWorld() {
		LoadInstalledObjectPrototypes();

		Tiles = new Tile[Width, Height];

		for(int x = 0; x < Width; x++) {
			for(int y = 0; y < Height; y++) {
				Tiles[x, y] = new Tile(x, y, "grass", this);
				Tiles[x, y].RegisterOnCreatedCallback(OnWorldObjectCreated);
				Tiles[x, y].RegisterOnChangedCallback(OnWorldObjectChanged);
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
        InstalledObject prototype = InstalledObjectPrototypes[type];
        tile.PlaceInstalledObject(prototype);
    }

    public void PlaceCharacter(Tile tile) {
        Character character = new Character(tile);
        Characters.Add(character);
        CharacterSpriteController.Instance.OnCharacterCreated(character);
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

    public void RegisterOnWorldObjectCreatedCallback(Action<WorldObject> callback) {
		OnWorldObjectCreated -= callback;
		OnWorldObjectCreated += callback;

        if(Tiles == null)
            return;

        for(int x = 0; x < Width; x++) {
            for(int y = 0; y < Height; y++) {
                Tiles[x, y].RegisterOnCreatedCallback(callback);
            }
        }
    }

	public void UnregisterOnWorldObjectCreatedCallback(Action<WorldObject> callback) {
		OnWorldObjectCreated -= callback;
	}

	public void RegisterOnWorldObjectChangedCallback(Action<WorldObject> callback) {
		OnWorldObjectChanged -= callback;
		OnWorldObjectChanged += callback;

	    if(Tiles == null)
	        return;

	    for(int x = 0; x < Width; x++) {
	        for(int y = 0; y < Height; y++) {
	            Tiles[x, y].RegisterOnChangedCallback(callback);
	        }
	    }
	}

	public void UnregisterOnWorldObjectChangedCallback(Action<WorldObject> callback) {
		OnWorldObjectChanged -= callback;
	}

}
