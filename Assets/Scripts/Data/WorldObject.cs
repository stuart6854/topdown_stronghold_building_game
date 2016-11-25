using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldObject {

	
	protected float X, Y;
    protected float Rotation; // In Degrees

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

}
