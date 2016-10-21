using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World {

	//Data
	private int Width, Height;
	private Tile[,] Tiles;

	//Callbacks
	private Action<WorldObject> OnWorldObjectCreated;
	private Action<WorldObject> OnWorldObjectChanged;

	public World(int width, int height, Action<WorldObject> onWorldObjectCreated) {
		this.Width = width;
		this.Height = height;
		RegisterOnWorldObjectCreatedCallback(onWorldObjectCreated);
		SetupInitalTiles();
	}

	private void SetupInitalTiles() {
		Tiles = new Tile[Width, Height];

		for(int x = 0; x < Width; x++) {
			for(int y = 0; y < Height; y++) {
				Tiles[x, y] = new Tile(x, y, "null", this);
				Tiles[x, y].RegisterOnCreatedCallback(OnWorldObjectCreated);
				Tiles[x, y].RegisterOnChangedCallback(OnWorldObjectChanged);
				OnWorldObjectCreated(Tiles[x, y]);
			}
		}
	}

	public Tile GetTile(int x, int y) {
		if(x < 0 || x > Width)
			return null;

		if(y < 0 || y > Height)
			return null;

		return Tiles[x, y];
	}

	public void RegisterOnWorldObjectCreatedCallback(Action<WorldObject> callback) {
		OnWorldObjectCreated -= callback;
		OnWorldObjectCreated += callback;
	}

	public void UnregisterOnWorldObjectCreatedCallback(Action<WorldObject> callback) {
		OnWorldObjectCreated -= callback;
	}

	public void RegisterOnWorldObjectChangedCallback(Action<WorldObject> callback) {
		OnWorldObjectChanged -= callback;
		OnWorldObjectChanged += callback;
	}

	public void UnregisterOnWorldObjectChangedCallback(Action<WorldObject> callback) {
		OnWorldObjectChanged -= callback;
	}

}
