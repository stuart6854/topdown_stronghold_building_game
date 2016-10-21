using System.Collections.Generic;
using UnityEngine;

public class BuildController : MonoBehaviour {

	public static BuildController Instance;

	public GameObject BuildDragPrefab;

	private bool InBuildMode = false;

	private WorldObjectType BuildMode;
	private string ObjectType;

	private List<GameObject> DragPreviewObjects;

	private void Awake() {
		Instance = this;
	}

	private void Start() {
		DragPreviewObjects = new List<GameObject>();
		SimplePool.Preload(BuildDragPrefab, 10, this.transform);
	}

	void Update () {
		if(Input.GetKeyUp(KeyCode.Escape)) {
			InBuildMode = false;
		}
	}

	public void DoAction(Vector2 start, Vector2 end) {
		GetLoopCoords(start, end, out start, out end);
		
		ClearDragPreviews();
		//TODO: Place object, queue build order, destroy object, etc.
		for(int x = (int) start.x; x <= end.x; x++) {
			for(int y = (int) start.y; y <= end.y; y++) {
				Tile t = WorldController.Instance.GetTileAt(x, y);
				if(t != null) {
					if(BuildMode == WorldObjectType.Tile) {
						t.ChangeType(ObjectType);
					} else if(BuildMode == WorldObjectType.InstalledObject) {
						//Place InstalledObject
					}
				}
			}
		}
	}

	public void OnDragging(Vector2 start, Vector2 end) {
		ClearDragPreviews();

		GetLoopCoords(start, end, out start, out end);

		for(int x = (int)start.x; x <= end.x; x++) {
			for(int y = (int)start.y; y <= end.y; y++) {
				Tile t = WorldController.Instance.GetTileAt(x, y);
				if(t != null) {
					//Display the building hint on top of this tile
					GameObject go = SimplePool.Spawn(BuildDragPrefab, new Vector3(x, y, -0.1f), Quaternion.identity);
					go.name = "Drag_Preview_Object";
					go.transform.SetParent(this.transform, true);

					DragPreviewObjects.Add(go);

					SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
					if(BuildMode == WorldObjectType.Tile) {
						sr.color = t.GetObjectType() == "null" ? Color.green : Color.red;
					} else {
						if(t.GetObjectType() == "null")
							sr.color = Color.red;
						else
							sr.color = t.GetInstalledObject() == null ? Color.green : Color.red;
					}
				}
			}
		}
	}

	public bool IsInBuildMode() {
		return InBuildMode;
	}

	public void SetInBuildMode(bool inBuildMode) {
		InBuildMode = inBuildMode;
	}

	public void PlaceTile(string type) {
		InBuildMode = true;
		BuildMode = WorldObjectType.Tile;;
		ObjectType = type;
	}

	public void PlaceInstalledObject(string type) {
		InBuildMode = true;
		BuildMode = WorldObjectType.InstalledObject;
		ObjectType = type;
	}

	private void ClearDragPreviews() {
		//Despawn Drag Preview Objects, if any
		while(DragPreviewObjects.Count > 0) {
			GameObject go = DragPreviewObjects[0];
			DragPreviewObjects.Remove(go);
			SimplePool.Despawn(go);
		}
	}

	private void GetLoopCoords(Vector2 start, Vector2 end, out Vector2 outStart, out Vector2 outEnd) {
		int startX = Mathf.RoundToInt(start.x);
		int startY = Mathf.RoundToInt(start.y);
		int endX = Mathf.RoundToInt(end.x);
		int endY = Mathf.RoundToInt(end.y);

		//Swap X coords if end.x < start.x
		if(endX < startX) {
			int tmp = startX;
			startX = endX;
			endX = tmp;
		}
		//Swap Y coords if end.y < start.y
		if(endY < startY) {
			int tmp = startY;
			startY = endY;
			endY = tmp;
		}
		outStart = new Vector2(startX, startY);
		outEnd = new Vector2(endX, endY);
	}

}
