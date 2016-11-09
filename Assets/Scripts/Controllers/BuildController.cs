using System.Collections.Generic;
using UnityEngine;

public enum BuildMethod {
    Single, Line, Grid
}

public enum BuildMode {
    None, Tile, InstalledObject, Character, Demolish
}

public class BuildController : MonoBehaviour {

    public static BuildController Instance;

    private static string InstaBuildVarName = "instabuild";
	private static bool InstaBuild {
		get { return bool.Parse(ConsoleController.Instance.GetSystemVariable(InstaBuildVarName)); }
	}

    public GameObject BuildDragPrefab;

    private BuildMode BuildMode;
    private string ObjectType;

    private List<GameObject> DragPreviewObjects;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        ConsoleController.Instance.RegisterSystemVariable(InstaBuildVarName, false);

        DragPreviewObjects = new List<GameObject>();
        SimplePool.Preload(BuildDragPrefab, 10, this.transform);
    }

    void Update() {
        if(Input.GetKeyUp(KeyCode.Escape)) {
            BuildMode = BuildMode.None;
        }

	    if(ConsoleController.Instance.IsVisble) {
		    ClearDragPreviews();
	    }
    }

    public void DoAction(Vector2 start, Vector2 end) {
        if(BuildMode == BuildMode.None)
            return;

        if(GetBuildMethod() == BuildMethod.Single || BuildMode == BuildMode.Character) {
            Tile tile = WorldController.Instance.GetTileAt(Mathf.RoundToInt(start.x), Mathf.RoundToInt(start.y));

            if(tile != null) {
                if(BuildMode == BuildMode.Character)
                    WorldController.Instance.GetWorld().PlaceCharacter(tile);
                else {
                    SetupJob(tile);
                }
            }

        } else {
            ClearDragPreviews();

            GetLoopCoords(start, end, out start, out end);

            for(int x = (int) start.x; x <= end.x; x++) {
                for(int y = (int) start.y; y <= end.y; y++) {
                    Tile tile = WorldController.Instance.GetTileAt(x, y);
                    if(tile != null)
                        SetupJob(tile);
                }
            }
        }
    }

    public void OnDragging(Vector2 start, Vector2 end) {
        ClearDragPreviews();

        if(BuildMode == BuildMode.None || BuildMode == BuildMode.Character)
            return;

        GetLoopCoords(start, end, out start, out end);

        for(int x = (int) start.x; x <= end.x; x++) {
            for(int y = (int) start.y; y <= end.y; y++) {
                Tile t = WorldController.Instance.GetTileAt(x, y);
                if(t == null) continue;

//	            if(BuildMode == BuildMode.Demolish)
//		            if(t.GetInstalledObject() == null)
//						continue;

                //Display the building hint on top of this tile
                GameObject go = SimplePool.Spawn(BuildDragPrefab, new Vector3(x, y, -2f), Quaternion.identity);
                go.name = "Drag_Preview_Object";
                go.transform.SetParent(this.transform, true);

                DragPreviewObjects.Add(go);

                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
                if(t.GetPendingJob() != null) {
                    sr.color = Color.red;
                } else if(BuildMode == BuildMode.Tile) {
                    sr.color = t.GetObjectType() != ObjectType ? Color.green : Color.red;
                } else if(BuildMode == BuildMode.InstalledObject) {
                    if(t.GetObjectType() == "null")
                        sr.color = Color.red;
                    else
                        sr.color = t.GetInstalledObject() == null ? Color.green : Color.red;
                }else if(BuildMode == BuildMode.Demolish) {
	                sr.color = t.GetInstalledObject() == null ? Color.white : Color.green;
                }
            }
        }
    }

    private void SetupJob(Tile tile) {
        string type = ObjectType;

	    Dictionary<string, int> requirements = null;
		if(!string.IsNullOrEmpty(type))
			requirements = WorldObjectMethod.Methods[type].GetConstructionRequirements();

        float jobTime = 1.0f;
        if(InstaBuild == true)
            jobTime = 0f;

	    Job job = null;
        if(BuildMode == BuildMode.Tile) {
            job = new Job(JobType.Construct, tile, j => tile.ChangeType(type), requirements, jobTime, 1);
        } else if(BuildMode == BuildMode.InstalledObject) {
            job = new Job(JobType.Construct, tile, j => WorldController.Instance.GetWorld().PlaceInstalledObject(type, tile), requirements, jobTime, 0);
            
        }else if(BuildMode == BuildMode.Demolish) {
	        job = new Job(JobType.Demolish, tile, j => WorldController.Instance.GetWorld().DemolishInstalledObject(tile), null, jobTime, 0);
        }

	    if(job != null) {
			if(tile.SetPendingJob(job))
				JobController.Instance.AddJob(job);
		}
    }

    public BuildMode GetBuildMode() {
        return BuildMode;
    }

    public void SetBuildMode(BuildMode buildMode) {
        BuildMode = buildMode;
    }

    public BuildMethod GetBuildMethod() {
		if(BuildMode == BuildMode.Demolish)
			return BuildMethod.Grid;

        if(!WorldObjectMethod.Methods.ContainsKey(ObjectType))
            return BuildMethod.Single;

        return WorldObjectMethod.Methods[ObjectType].GetBuildMethod();
    }

    public void PlaceTile(string type) {
        BuildMode = BuildMode.Tile;
        ObjectType = type;
    }

    public void PlaceInstalledObject(string type) {
        BuildMode = BuildMode.InstalledObject;
        ObjectType = type;
    }

    public void PlaceCharacter() {
        //TODO: Note that this is temporary, in final game players wont be able to place new characters
        BuildMode = BuildMode.Character;
        ObjectType = "character";
    }

	public void SetDemolish() {
		BuildMode = BuildMode.Demolish;
		ObjectType = null;
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