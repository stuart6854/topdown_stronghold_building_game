using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InstalledObject : Constructable, IContextMenu, ITooltip{

	private Tile Tile;

	private int Width = -1;
	private int Height = -1;

	public InstalledObject PlaceInstance(string objectType, Tile tile, bool baseInstance = true) {
		if(baseInstance) { 
			this.ObjectType = objectType;
			this.WorldObjectType = WorldObjectType.InstalledObject;
			this.Tile = tile;
			this.X = tile.GetX();
			this.Y = tile.GetY();

			int width = this.GetWidth(this.ObjectType);
			int height = this.GetHeight(this.ObjectType);
			if(width != 1 || height != 1) {
				//Not 1x1

				for(int x = 1; x < width; x++) {
					for(int y = 1; y < height; y++) {
						Tile t = WorldController.GetTileAt((int) this.X + x, (int) this.Y + y);

						t.PlaceInstalledObject(this.ObjectType, this, false);
					}
				}
			}

			this.OnCreatedCB(this);
		}

		return this;

}

	public abstract void OnCreated();

	public abstract void OnUpdate();

	public abstract void OnDestroyed();

	public virtual void OnDismantled() {}

	public Tile GetTile() {
		return Tile;
	}

	public int GetMovementMultiplier() {
		string val = Defs.GetDef(this.ObjectType).Properties.GetValue("MovementMultiplier");
		return int.Parse(val);
	}

	public virtual Enterabilty GetEnterability() {
		return Enterabilty.Never; // Default. Can be overriden.
	}

	public virtual bool GetConnectsToNeighbours() {
		return false;
	}

	public int GetWidth(string objType) {
		//NOTE: This instances ObjectType variable wont have been set yet, hence the objType param
		if(this.Width > 0)
			return this.Width;

		DefinitionProperties.XMLTag widthTag = Defs.GetDef(objType).Properties.GetXMLData("Width");
		if(widthTag == null)
			return -1;

		this.Width = int.Parse(widthTag.Value);

		return this.Width;
	}

	public int GetHeight(string objType) {
		//NOTE: This instances ObjectType variable wont have been set yet, hence the objType param
		if(this.Height > 0)
			return this.Height;

		DefinitionProperties.XMLTag heightTag = Defs.GetDef(objType).Properties.GetXMLData("Height");
		if(heightTag == null)
			return -1;

		this.Height = int.Parse(heightTag.Value);

		return this.Height;
	}

	public override float GetZ() {
        return -0.1f;
    }

	public virtual RadialMenuItem[] GetContextMenuOptions() {
		List<RadialMenuItem> MenuItems = new List<RadialMenuItem>();

		Font font = Fonts.GetFont("Arial-Regular");
		if(font == null)
			Debug.LogError("InstalledObject::GetContextMenuOptions -> Font is null!");

		MenuItems.Add(new RadialMenuItem("Dismantle", font, () => {
			JobController.CreateDismantleJob(this.GetTile());
		}));

		return MenuItems.ToArray();
	}

	public abstract string Tooltip_GetTitle();

	public abstract string Tooltip_GetBodyText();

}
