using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum WorldObjectType {
	Tile, InstalledObject, LooseItem, Character
}

public abstract class WorldObject {

	protected static long Next_ID = 0; //The next ID to be assigned to a new worldobject

	public long ID { get; protected set; }

	protected string ObjectType;
	protected WorldObjectType WorldObjectType;

	protected float X, Y;
	protected float Rotation; // In Degrees

	public bool IsAnimated = false;

	protected Action<WorldObject> OnCreatedCB;
	protected Action<WorldObject> OnUpdateCB;
	protected Action<WorldObject> OnDestroyedCB;

	protected WorldObject() {
		AssignID(this);
	}

	public abstract string GetSpriteName();

	public string GetObjectType() {
		return this.ObjectType;
	}

	public WorldObjectType GetWorldObjectType() {
		return this.WorldObjectType;
	}

	public virtual float GetX() {
		return X;
	}

	public virtual float GetY() {
		return Y;
	}

	public virtual float GetZ() {
		return 0.0f;
	}

	public float GetRotation() {
		return Rotation;
	}

	public Action<WorldObject> GetOnCreatedCB() {
		return OnCreatedCB;
	}

	public Action<WorldObject> GetOnUpdatedCB() {
		return OnUpdateCB;
	}

	public Action<WorldObject> GetOnDestroyedCB() {
		return OnDestroyedCB;
	}

	public void RegOnCreatedCB(Action<WorldObject> callback) {
		OnCreatedCB -= callback;
		OnCreatedCB += callback;
	}

	public void DeregOnCreatedCB(Action<WorldObject> callback) {
		OnCreatedCB -= callback;
	}

	public void RegOnUpdateCB(Action<WorldObject> callback) {
		OnUpdateCB -= callback;
		OnUpdateCB += callback;
	}

	public void DeregOnUpdatedCB(Action<WorldObject> callback) {
		OnUpdateCB -= callback;
	}

	public void RegOnDestroyedCB(Action<WorldObject> callback) {
		OnDestroyedCB -= callback;
		OnDestroyedCB += callback;
	}

	public void DeregOnDestroyedCB(Action<WorldObject> callback) {
		OnDestroyedCB -= callback;
	}

	private static void AssignID(WorldObject worldObject) {
		worldObject.ID = Next_ID;
		Next_ID++;
	}

}
