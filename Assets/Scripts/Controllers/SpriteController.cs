using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteController : MonoBehaviour {

	public static SpriteController Instance;

	public Material SpriteMaterial;


	public Transform TileParent;
	public Transform InstalledObjectParent;
	public Transform LooseItemParent;

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

	public void OnWorldObjectCreated(WorldObject worldObject) {
		int x = worldObject.GetX();
		int y = worldObject.GetY();
		float z = worldObject.GetZ();

		GameObject obj = new GameObject(worldObject.GetObjectType() + "_" + x + "_" + y);
		obj.transform.position = new Vector3(x, y, z);

		SpriteRenderer sr = obj.AddComponent<SpriteRenderer>();
		sr.material = SpriteMaterial;
		sr.sprite = GetSprite(worldObject.GetObjectType());

		if(worldObject.GetWorldObjectType() == WorldObjectType.Tile)
			obj.transform.SetParent(TileParent);
		else if(worldObject.GetWorldObjectType() == WorldObjectType.InstalledObject)
			obj.transform.SetParent(InstalledObjectParent);
		else if(worldObject.GetWorldObjectType() == WorldObjectType.LooseItem)
			obj.transform.SetParent(LooseItemParent);
	}

	public void OnWorldObjectChanged(WorldObject oldObject) {
		
	}

	private Sprite GetSprite(string objectType) {
		if(ObjectSprites.ContainsKey(objectType + "_0"))
			return ObjectSprites[objectType + "_0"];

		return null;
	}

}
