using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseItem : WorldObject {

	private Tile Tile;

    //Data
    private int MaxStackSize;
    private int StackSize;

	public LooseItem(string type, int amount) {
	    this.WorldObjectType = WorldObjectType.LooseItem;
		this.ObjectType = type;
	}

    public LooseItem(LooseItem other) {
        this.WorldObjectType = WorldObjectType.LooseItem;
        this.ObjectType = other.ObjectType;
        this.StackSize = other.StackSize;
    }

    public override void OnUpdate(){  }

    public LooseItem Clone() {
        return new LooseItem(this);
    }

    public void SetTile(Tile tile) {
        this.Tile = tile;
        this.X = tile.GetX();
        this.Y = tile.GetY();
    }

    public Tile GetTile() {
        return Tile;
    }

    public override float GetZ() {
        return -0.2f;
    }

    public void AddToStack(int amnt) {
        StackSize += amnt;
    }

    public void RemoveFromStack(int amnt) {
        StackSize -= amnt;
    }

    public int GetMaxStackSize() {
        return MaxStackSize;
    }

    public void SetStackSize(int size) {
        this.StackSize = size;
    }

    public int GetStackSize() {
        return StackSize;
    }
}
