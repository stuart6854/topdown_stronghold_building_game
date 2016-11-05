using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour {

	public static SpriteController Instance;

	public Material SpriteMaterial;

	public Transform TileParent;
	public Transform InstalledObjectParent;
	public Transform LooseItemParent;
	public Transform CharacterParent;

	private Dictionary<string, Sprite> ObjectSprites;

	private Dictionary<WorldObject, GameObject> WorldObjectGameObjects;

	void Awake() {
		Instance = this;

		LoadSprites();
	}

	void Start() {
		WorldObjectGameObjects = new Dictionary<WorldObject, GameObject>();
	}

	private void LoadSprites() {
		ObjectSprites = new Dictionary<string, Sprite>();

		Sprite[] sprites = Resources.LoadAll<Sprite>("Sprites");
		foreach(Sprite sprite in sprites) {
			ObjectSprites.Add(sprite.name, sprite);
		}
	}

    public void SetSprite(string spriteName, WorldObject worldObject) {
        if(!WorldObjectGameObjects.ContainsKey(worldObject)) {
            Debug.Log("SpriteController::SetSprite -> This worldobject doesn't have an associated gameobject!");
            return;
        }

        GameObject wo_go = WorldObjectGameObjects[worldObject];

        if(wo_go == null) {
            Debug.Log("SpriteController::SetSprite -> This worldobjects associated gameobject is NULL!");
            return;
        }

        if(!ObjectSprites.ContainsKey(spriteName)) {
            Debug.Log("SpriteController::SetSprite -> The sprite " + spriteName + " does NOT Exist!");
            return;
        }

        SpriteRenderer sr = wo_go.GetComponent<SpriteRenderer>();
        sr.sprite = ObjectSprites[spriteName];
    }

	public void OnWorldObjectCreated(WorldObject worldObject) {
		float x = worldObject.GetX();
		float y = worldObject.GetY();
		float z = worldObject.GetZ();

		GameObject obj = new GameObject(worldObject.GetObjectType() + "_" + x + "_" + y);
		obj.transform.position = new Vector3(x, y, z);

		obj.AddComponent<BoxCollider2D>();

		ObjectDataReference objDataRef = obj.AddComponent<ObjectDataReference>();
		objDataRef.X = (int)x;
		objDataRef.Y = (int)y;
		objDataRef.ObjectType = worldObject.GetWorldObjectType();

		SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
		sr.material = SpriteMaterial;
		sr.sprite = GetSprite(worldObject);

		if(worldObject.GetWorldObjectType() == WorldObjectType.Tile) {
			obj.transform.SetParent(TileParent);
			obj.layer = LayerMask.NameToLayer("Tile");
		} else if(worldObject.GetWorldObjectType() == WorldObjectType.InstalledObject) {
			obj.transform.SetParent(InstalledObjectParent);
			obj.layer = LayerMask.NameToLayer("InstalledObject");
		} else if(worldObject.GetWorldObjectType() == WorldObjectType.LooseItem) {
			obj.transform.SetParent(LooseItemParent);
			obj.layer = LayerMask.NameToLayer("LooseItem");
		} else if(worldObject.GetWorldObjectType() == WorldObjectType.Character) {
			obj.transform.SetParent(CharacterParent);
			obj.layer = LayerMask.NameToLayer("Character");
		}

		WorldObjectGameObjects.Add(worldObject, obj);
	}

	public void OnWorldObjectChanged(WorldObject worldObject) {
		if(!WorldObjectGameObjects.ContainsKey(worldObject)) {
			Debug.Log("SpriteController::OnWorldObjectChanged -> This worldobject doesn't have an associated gameobject!");
			return;
		}

		GameObject wo_go = WorldObjectGameObjects[worldObject];

		if(wo_go == null) {
			Debug.Log("SpriteController::OnWorldObjectChanged -> This worldobjects associated gameobject is NULL!");
			return;
		}

	    float x = worldObject.GetX();
	    float y = worldObject.GetY();
	    float z = worldObject.GetZ();
	    float rot = worldObject.GetRotation();

	    wo_go.transform.position = new Vector3(x, y, z);
	    wo_go.transform.eulerAngles = new Vector3(0, 0, rot);

	    SpriteRenderer sr = wo_go.GetComponent<SpriteRenderer>();
		sr.sprite = GetSprite(worldObject);
	}

	public void OnWorldObjectDestroyed(WorldObject worldObject) {
		if(!WorldObjectGameObjects.ContainsKey(worldObject)) {
			Debug.Log("SpriteController::OnWorldObjectDestroyed -> This worldobject doesn't have an associated gameobject!");
			return;
		}

		GameObject wo_go = WorldObjectGameObjects[worldObject];

		WorldObjectGameObjects.Remove(worldObject);

		if(wo_go == null) {
			Debug.Log("SpriteController::OnWorldObjectDestroyed -> This worldobjects associated gameobject is NULL!");
			return;
		}

		Destroy(wo_go);

//		Debug.Log("Destoyed WorldObjects GameObject");
	}

	private Sprite GetSprite(WorldObject obj) {
		InstalledObject installedObject = obj as InstalledObject;
		if(installedObject != null) {
			if(installedObject.DoesConnectToNeighbours()) {
				int Bitmask = GetInstalledObjectBitmask(installedObject);

				if(ObjectSprites.ContainsKey(installedObject.GetObjectType() + "_" + Bitmask))
					return ObjectSprites[installedObject.GetObjectType() + "_" + Bitmask];
			}
		}


		if(ObjectSprites.ContainsKey(obj.GetObjectType() + "_0"))
			return ObjectSprites[obj.GetObjectType() + "_0"];

		return null;
	}

	private int GetInstalledObjectBitmask(InstalledObject io) {
		int x = (int)io.GetTile().GetX();
		int y = (int)io.GetTile().GetY();
		int bitmask = 0;

		Tile tile = WorldController.Instance.GetTileAt(x, y + 1); //North
		if(tile != null) {
			InstalledObject tileIO = tile.GetInstalledObject();
			if(tileIO != null && tileIO.GetObjectType() == io.GetObjectType())
				bitmask += 1;
		}

		tile = WorldController.Instance.GetTileAt(x + 1, y); //East
		if(tile != null) {
			InstalledObject tileIO = tile.GetInstalledObject();
			if(tileIO != null && tileIO.GetObjectType() == io.GetObjectType())
				bitmask += 2;
		}

		tile = WorldController.Instance.GetTileAt(x, y - 1); //South
		if(tile != null) {
			InstalledObject tileIO = tile.GetInstalledObject();
			if(tileIO != null && tileIO.GetObjectType() == io.GetObjectType())
				bitmask += 4;
		}

		tile = WorldController.Instance.GetTileAt(x - 1, y); //West
		if(tile != null) {
			InstalledObject tileIO = tile.GetInstalledObject();
			if(tileIO != null && tileIO.GetObjectType() == io.GetObjectType())
				bitmask += 8;
		}

		return bitmask;
	}

}
