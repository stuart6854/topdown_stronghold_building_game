using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WorldObjectType { 
	Tile, InstalledObject, LooseItem
}

public abstract class WorldObject {

	protected WorldObjectType WorldObjectType;
	protected string ObjectType;

	protected int X, Y;
	protected float Z;

	protected float MovementCost;

	protected WorldObjectMethod methods;

	protected Action<WorldObject> OnCreated;
	protected Action<WorldObject> OnChanged;

	public abstract void OnUpdate();

	public WorldObjectType GetWorldObjectType() {
		return WorldObjectType;
	}

	public string GetObjectType() {
		return ObjectType;
	}

	public int GetX() {
		return X;
	}

	public int GetY() {
		return Y;
	}

	public float GetZ() {
		return Z;
	}

	public float GetMovementCost() {
		return MovementCost;
	}

	public WorldObjectMethod GetMethods() {
		return methods;
	}

	public Action<WorldObject> GetOnCreated() {
		return OnCreated;
	}

	public Action<WorldObject> GetOnChanged() {
		return OnChanged;
	}

	public void RegisterOnCreatedCallback(Action<WorldObject> callback) {
		OnCreated -= callback;
		OnCreated += callback;
	}

	public void UnregisterOnCreatedCallback(Action<WorldObject> callback) {
		OnCreated -= callback;
	}

	public void RegisterOnChangedCallback(Action<WorldObject> callback) {
		OnChanged -= callback;
		OnChanged += callback;
	}

	public void UnregisterOnChangedCallback(Action<WorldObject> callback) {
		OnChanged -= callback;
	}

}
