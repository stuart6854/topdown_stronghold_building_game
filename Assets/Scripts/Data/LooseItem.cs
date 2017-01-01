using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LooseItem : WorldObject, ITooltip {

	private Tile Tile;

    //Data
    private int MaxStackSize;
    private int StackSize;

	public LooseItem(LooseItem other) : this(other.ObjectType, other.StackSize){
		
	}

	public LooseItem(string type, int amount) : base() {
		this.ObjectType = type;
	    this.StackSize = amount;
		this.WorldObjectType = WorldObjectType.LooseItem;
	}

	public LooseItem Clone() {
        return new LooseItem(this);
    }

	public void AddToStack(int amnt) {
        StackSize += amnt;
    }

	/// <summary>
	/// Tries to remove the requested amount from the stack.
	/// </summary>
	/// <param name="amnt"></param>
	/// <returns>The amount that it managed to remove. This could be less than requested!</returns>
	public int RemoveFromStack(int amnt) {
		int removedAmnt = 0;
		if(StackSize - amnt >= 0) {//Their is enough for whole request
			StackSize -= amnt;
			removedAmnt = amnt;
		} else { //Not enough, remove what we can
			removedAmnt = amnt - StackSize;
			StackSize -= removedAmnt;
		}

        if(StackSize <= 0 && Tile != null)
            Tile.PlaceLooseItem(null);

		return removedAmnt;
	}

	public void SetStackSize(int size) {
        this.StackSize = size;
    }

	public void SetTile(Tile tile) {
		this.Tile = tile;
		this.X = tile.GetX();
		this.Y = tile.GetY();
	}

	public int GetMaxStackSize() {
		return MaxStackSize;
	}

	public Tile GetTile() {
		return Tile;
	}

	public int GetStackSize() {
        return StackSize;
    }

	public override float GetZ() {
		return -0.2f;
	}

	public override string GetSpriteName() {
		return "looseitem_" + ObjectType;
	}

	public string Tooltip_GetTitle() {
		return this.ObjectType;
	}

	public string Tooltip_GetBodyText() {
		return "LooseItem Body Text.";
	}

}
