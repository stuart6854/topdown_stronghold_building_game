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

	protected Action<WorldObject> OnCreatedCB;
	protected Action<WorldObject> OnChangedCB;
	protected Action<WorldObject> OnDestroyedCB;

	public abstract void OnCreated();

	public abstract void OnUpdate();

	public abstract void OnDestroyed();

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
		return OnCreatedCB;
	}

	public Action<WorldObject> GetOnChanged() {
		return OnChangedCB;
	}

	public Action<WorldObject> GetOnDestroyed() {
		return OnDestroyedCB;
	}

	public void RegisterOnCreatedCallback(Action<WorldObject> callback) {
		OnCreatedCB -= callback;
		OnCreatedCB += callback;
	}

	public void UnregisterOnCreatedCallback(Action<WorldObject> callback) {
		OnCreatedCB -= callback;
	}

	public void RegisterOnChangedCallback(Action<WorldObject> callback) {
		OnChangedCB -= callback;
		OnChangedCB += callback;
	}

	public void UnregisterOnChangedCallback(Action<WorldObject> callback) {
		OnChangedCB -= callback;
	}

	public void RegisterOnDestroyedCallback(Action<WorldObject> callback) {
		OnDestroyedCB -= callback;
		OnDestroyedCB += callback;
	}

	public void UnregisterOnDestroyedCallback(Action<WorldObject> callback) {
		OnDestroyedCB -= callback;
	}

}
