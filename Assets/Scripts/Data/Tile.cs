using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : WorldObject {

	//References
	private World world;

	private InstalledObject InstalledObject;
	private LooseItem LooseItem;

	//Data

	
	public Tile(int x, int y, string objectType, World world) {
		this.X = x;
		this.Y = y;
		this.Z = 0.0f;
		this.WorldObjectType = WorldObjectType.Tile;
		base.ObjectType = objectType;
		this.world = world;

		if(OnCreated != null)
			OnCreated(this);
	}

	public bool PlaceInstalledObject(InstalledObject prototype) {
		if(this.InstalledObject != null) {
			return false;
		}
		
		this.InstalledObject = prototype.PlaceInstance(this);

		//TODO: Update neighbours if installed object connects to them

		if(this.InstalledObject.GetOnCreated() != null)
			this.InstalledObject.GetOnCreated()(this.InstalledObject);

		return true;
	}

	public InstalledObject GetInstalledObject() {
		return InstalledObject;
	}

	public LooseItem GetLooseItem() {
		return LooseItem;
	}

	public void ChangeType(string type) {
		if(type == ObjectType)
			return; //We are already this type

		this.ObjectType = type;

		if(OnChanged != null)
			OnChanged(this);
	}

}
