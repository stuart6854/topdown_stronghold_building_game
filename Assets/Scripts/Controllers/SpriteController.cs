using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour {

	public static SpriteController Instance;

	public Material spriteMaterial;

	public Transform tileParent;
	public Transform installedObjectParent;
	public Transform looseItemParent;
	public Transform characterParent;
	public Transform jobParent;

	private static Dictionary<string, Sprite> sprites = new Dictionary<string, Sprite>();

	private Dictionary<WorldObject, GameObject> worldObjectGameObjects;
	private Dictionary<Job, GameObject> jobGameObjects;

	void Awake() {
		Instance = this;

		LoadSprites();
	}

	void Start() {
		worldObjectGameObjects = new Dictionary<WorldObject, GameObject>();
		jobGameObjects = new Dictionary<Job, GameObject>();
	}

	private void LoadSprites() {
		Sprite[] sprites = Resources.LoadAll<Sprite>("sprites");
		foreach(Sprite sprite in sprites) {
			SpriteController.sprites.Add(sprite.name, sprite);
		}
	}

    public void SetSprite(string spriteName, WorldObject worldObject) {
        if(!worldObjectGameObjects.ContainsKey(worldObject)) {
            Debug.Log("SpriteController::SetSprite -> This worldobject doesn't have an associated gameobject!");
            return;
        }

        GameObject wo_go = worldObjectGameObjects[worldObject];

        if(wo_go == null) {
            Debug.Log("SpriteController::SetSprite -> This worldobjects associated gameobject is NULL!");
            return;
        }

        if(!sprites.ContainsKey(spriteName)) {
            Debug.Log("SpriteController::SetSprite -> The sprite " + spriteName + " does NOT Exist!");
            return;
        }

        SpriteRenderer sr = wo_go.GetComponent<SpriteRenderer>();
        sr.sprite = sprites[spriteName];
    }

	public void OnWorldObjectCreated(WorldObject worldObject) {
		if(worldObjectGameObjects.ContainsKey(worldObject)) {
			Debug.LogError("SpriteController::OnWorldObjectCreated -> A Sprite has already been created for this worldObject!");
			return;
		}

		float x = worldObject.GetX();
		float y = worldObject.GetY();
		float z = worldObject.GetZ();

		GameObject obj = new GameObject(worldObject.GetObjectType() + "_" + x + "_" + y);
		obj.transform.position = new Vector3(x, y, z);

		BoxCollider2D collider = obj.AddComponent<BoxCollider2D>();

		InstalledObject io = worldObject as InstalledObject;
		if(io != null) {
			int width = io.GetWidth(io.GetObjectType());
			int height = io.GetHeight(io.GetObjectType());
			collider.size = new Vector2(width, height);
			collider.offset = new Vector2((width - 1) * 0.5f, (height - 1) * 0.5f);
		}

		ObjectDataReference objDataRef = obj.AddComponent<ObjectDataReference>();
		objDataRef.WorldObject = worldObject;
		objDataRef.WorldObjectType = worldObject.GetWorldObjectType();

		SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
		sr.material = spriteMaterial;
		sr.sprite = GetSprite(worldObject);

		if(worldObject is Tile) {
			obj.transform.SetParent(tileParent);
			obj.layer = LayerMask.NameToLayer("Tile");
		} else if(worldObject is InstalledObject) {
			obj.transform.SetParent(installedObjectParent);
			obj.layer = LayerMask.NameToLayer("InstalledObject");
		} else if(worldObject is LooseItem) {
			obj.transform.SetParent(looseItemParent);
			obj.layer = LayerMask.NameToLayer("LooseItem");
		} else if(worldObject is Character) {
			obj.transform.SetParent(characterParent);
			obj.layer = LayerMask.NameToLayer("Character");
		}

		worldObjectGameObjects.Add(worldObject, obj);
	}

	public void OnWorldObjectChanged(WorldObject worldObject) {
		if(!worldObjectGameObjects.ContainsKey(worldObject)) {
			Debug.Log("SpriteController::OnWorldObjectChanged -> This worldobject doesn't have an associated gameobject!");
			return;
		}

		GameObject wo_go = worldObjectGameObjects[worldObject];

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

		if(worldObject.IsAnimated)
			return; //Dont change sprite as it is controlled by an animation

	    SpriteRenderer sr = wo_go.GetComponent<SpriteRenderer>();
		sr.sprite = GetSprite(worldObject);
	}

	public void OnWorldObjectDestroyed(WorldObject worldObject) {
		if(!worldObjectGameObjects.ContainsKey(worldObject)) {
			Debug.Log("SpriteController::OnWorldObjectDestroyed -> This worldobject doesn't have an associated gameobject!");
			return;
		}

		GameObject wo_go = worldObjectGameObjects[worldObject];

		worldObjectGameObjects.Remove(worldObject);

		if(wo_go == null) {
			Debug.Log("SpriteController::OnWorldObjectDestroyed -> This worldobjects associated gameobject is NULL!");
			return;
		}

		Destroy(wo_go);

//		Debug.Log("Destoyed WorldObjects GameObject");
	}

	private static Sprite GetSprite(WorldObject worldObject) {
//		InstalledObject installedObject = worldObject as InstalledObject;
//		if(installedObject != null) {
//			if(installedObject.GetConnectsToNeighbours()) {
//				int Bitmask = GetInstalledObjectBitmask(installedObject);
//
//				if(sprites.ContainsKey(installedObject.GetObjectType() + "_" + Bitmask))
//					return sprites[installedObject.GetObjectType() + "_" + Bitmask];
//			}
//		}

		string spriteName = worldObject.GetSpriteName();
		if(sprites.ContainsKey(spriteName))
			return sprites[spriteName];

		Debug.LogError("SpriteController::GetSprite(key) -> Sprite no found: " + spriteName);

		//If sprite NOT found
		if(sprites.ContainsKey("null_sprite"))
			return sprites["null_sprite"]; // Assign "Null" sprite if found

		Debug.LogError("SpriteController::GetSprite(key) -> Sprite no found: null_sprite");

		//Else return null
		return null;
	}

	private int GetInstalledObjectBitmask(InstalledObject io) {
		int x = (int)io.GetTile().GetX();
		int y = (int)io.GetTile().GetY();
		int bitmask = 0;

		Tile tile = WorldController.GetTileAt(x, y + 1); //North
		if(tile != null) {
			InstalledObject tileIO = tile.GetInstalledObject();
			if(tileIO != null && tileIO.GetObjectType() == io.GetObjectType())
				bitmask += 1;
		}

		tile = WorldController.GetTileAt(x + 1, y); //East
		if(tile != null) {
			InstalledObject tileIO = tile.GetInstalledObject();
			if(tileIO != null && tileIO.GetObjectType() == io.GetObjectType())
				bitmask += 2;
		}

		tile = WorldController.GetTileAt(x, y - 1); //South
		if(tile != null) {
			InstalledObject tileIO = tile.GetInstalledObject();
			if(tileIO != null && tileIO.GetObjectType() == io.GetObjectType())
				bitmask += 4;
		}

		tile = WorldController.GetTileAt(x - 1, y); //West
		if(tile != null) {
			InstalledObject tileIO = tile.GetInstalledObject();
			if(tileIO != null && tileIO.GetObjectType() == io.GetObjectType())
				bitmask += 8;
		}

		return bitmask;
	}

	public void OnJobCreated(Job _job) {
		if(jobGameObjects.ContainsKey(_job)) {
			Debug.LogError("SpriteController::OnJobCreated -> A Sprite has already been created for this Job!");
			return;
		}

		float x = _job.GetTile().GetX();
		float y = _job.GetTile().GetY();

		GameObject obj = new GameObject("job_" + x + "_" + y);
		obj.transform.position = new Vector3(x, y, -0.3f);
		obj.AddComponent<BoxCollider2D>();

		SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
		sr.material = spriteMaterial;
		sr.sprite = GetSprite("sprite_job");

		obj.transform.SetParent(jobParent);
		obj.layer = LayerMask.NameToLayer("Job");

		jobGameObjects.Add(_job, obj);
	}

	public void OnJobRemoved(Job _job) {
		
	}

	public static Sprite GetSprite(string key) {
		if(sprites.ContainsKey(key))
			return sprites[key];

		Debug.LogError("SpriteController::GetSprite(key) -> Sprite no found: " + key);

		//If sprite NOT found
		if(sprites.ContainsKey("null_sprite"))
			return sprites["null_sprite"]; // Assign "Null" sprite if found

		Debug.LogError("SpriteController::GetSprite(key) -> Sprite no found: null_sprite");

		//Else return null
		return null;
	}

	public GameObject GetGameObject(WorldObject worldObject) {
		if(!worldObjectGameObjects.ContainsKey(worldObject)) {
			Debug.LogError("SpriteController::GetGameObject -> No Gameobject attached to this WorldObject");
			return null;
		}

		return worldObjectGameObjects[worldObject];
	}

	public static void RegisterSprite(string key, Sprite sprite) {
		if(sprites.ContainsKey(key)) {
			Debug.LogError("SpriteManager::RegisterSprite -> Sprite has already been registered with key:" + key);
			return;
		}

		sprites.Add(key, sprite);
	}

}
