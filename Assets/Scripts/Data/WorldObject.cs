using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WorldObjectType { 
	Tile, InstalledObject, LooseItem, Character
}

public abstract class WorldObject {

	protected WorldObjectType WorldObjectType;
	protected string ObjectType;

	protected float X, Y;
	protected float Z;
    protected float Rotation; //In Degrees

	protected float MovementCost = 1.0f; //Multiplier - 2 = Double Speed, 0.5 = Half Speed

	protected WorldObjectMethod Methods;
    protected Dictionary<string, object> Parameters;

	protected Action<WorldObject> OnCreated;
	protected Action<WorldObject> OnChanged;
	protected Action<WorldObject> OnDestroyed;

	public abstract void OnUpdate();

	public WorldObjectType GetWorldObjectType() {
		return WorldObjectType;
	}

	public string GetObjectType() {
		return ObjectType;
	}

	public virtual float GetX() {
		return X;
	}

	public virtual float GetY() {
		return Y;
	}

	public virtual float GetZ() {
		return Z;
	}

    public float GetRotation() {
        return Rotation;
    }

	public float GetMovementCost() {
		return MovementCost;
	}

	public WorldObjectMethod GetMethods() {
		return Methods;
	}

    public void SetParameter(string key, object value) {
        if(Parameters.ContainsKey(key))
            Parameters[key] = value;
        else
            Parameters.Add(key, value);
    }

    public object GetParameter(string key) {
        return Parameters[key];
    }

	public Action<WorldObject> GetOnCreated() {
		return OnCreated;
	}

	public Action<WorldObject> GetOnChanged() {
		return OnChanged;
	}

	public Action<WorldObject> GetOnDestroyed() {
		return OnDestroyed;
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

	public void RegisterOnDestroyedCallback(Action<WorldObject> callback) {
		OnDestroyed -= callback;
		OnDestroyed += callback;
	}

	public void UnregisterOnDestroyedCallback(Action<WorldObject> callback) {
		OnDestroyed -= callback;
	}

}
