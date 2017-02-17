using System;
using System.Collections.Generic;
using UnityEngine;

public enum BuildMethod {
    Single, Line, Grid
}

public enum ActionMode {
    None, Tile, InstalledObject, Character, Dismantle, Destroy
}

public class BuildController : MonoBehaviour {

    public static BuildController Instance;

	private static string InstaBuildVarName = "instabuild";
	public static bool InstaBuild {
		get { return bool.Parse(ConsoleController.Instance.GetSystemVariable(InstaBuildVarName)); }
	}

    public GameObject BuildDragPrefab;

    private ActionMode ActionMode;
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
            ActionMode = ActionMode.None;
        }

	    if(ConsoleController.Instance.IsVisble) {
		    ClearDragPreviews();
	    }
    }

    public void DoAction(Vector2 start, Vector2 end) {
        if(ActionMode == ActionMode.None)
            return;

        if(GetBuildMethod() == BuildMethod.Single || ActionMode == ActionMode.Character) {
            Tile tile = WorldController.GetTileAt(Mathf.RoundToInt(start.x), Mathf.RoundToInt(start.y));

            if(tile != null) {
                if(ActionMode == ActionMode.Character)
                    WorldController.Instance.GetWorld().PlaceCharacter(tile);
                else {
					if(ActionMode == ActionMode.Dismantle)
						SetupDismantleOrder(tile);
					else
						SetupBuildOrder(tile);
				}
            }

        } else {
            ClearDragPreviews();

            GetLoopCoords(start, end, out start, out end);

            for(int x = (int) start.x; x <= end.x; x++) {
                for(int y = (int) start.y; y <= end.y; y++) {
                    Tile tile = WorldController.GetTileAt(x, y);
	                if(tile != null) {
						if(ActionMode == ActionMode.Dismantle)
							SetupDismantleOrder(tile);
						else
							SetupBuildOrder(tile);
	                }
                }
            }
        }
    }

    public void OnDragging(Vector2 start, Vector2 end) {
        ClearDragPreviews();

        if(ActionMode == ActionMode.None || ActionMode == ActionMode.Character)
            return;

        GetLoopCoords(start, end, out start, out end);

        for(int x = (int) start.x; x <= end.x; x++) {
            for(int y = (int) start.y; y <= end.y; y++) {
                Tile t = WorldController.GetTileAt(x, y);
                if(t == null) continue;

	            if(ActionMode == ActionMode.Dismantle)
		            if(t.GetInstalledObject() == null)
						continue;

                //Display the building hint on top of this tile
                GameObject go = SimplePool.Spawn(BuildDragPrefab, new Vector3(x, y, -2f), Quaternion.identity);
                go.name = "Drag_Preview_Object";
                go.transform.SetParent(this.transform, true);

                DragPreviewObjects.Add(go);

                SpriteRenderer sr = go.GetComponent<SpriteRenderer>();
				if(t.GetPendingJob() != null) {
				    sr.color = Color.red;
				} else if(ActionMode == ActionMode.Tile) {
                    sr.color = (t.GetObjectType() != ObjectType ? Color.green : Color.red);
                } else if(ActionMode == ActionMode.InstalledObject) {
                    if(t.GetObjectType() == "null")
                        sr.color = Color.red;
                    else
                        sr.color = (t.GetInstalledObject() == null ? Color.green : Color.red);
                }else if(ActionMode == ActionMode.Dismantle) {
	                sr.color = (t.GetInstalledObject() == null ? Color.white : Color.green);
                }
            }
        }
    }

    private void SetupBuildOrder(Tile tile) {
		if(ActionMode == ActionMode.Tile && tile.GetObjectType() == ObjectType)
			return;
		else if(ActionMode == ActionMode.InstalledObject && tile.GetInstalledObject() != null)
			return;

		string type = ObjectType;
		ActionMode mode = Defs.GetDef(type).Properties.DefCategory.ToActionMode();

	    float buildTime = GetJobTime(type, mode);

		Job_Construct job = new Job_Construct(type, tile, buildTime);

//		Job job = new Job(JobTypes.Construct, tile, -1, 0);
//		job.AddTask(new Task_MoveTo(tile));
//		if(ActionMode == ActionMode.Tile)
//			job.AddTask(new Task_DoAction(() => tile.ChangeType(type), buildTime));
//		else if(ActionMode == ActionMode.InstalledObject)
//			job.AddTask(new Task_DoAction(() => WorldController.Instance.GetWorld().PlaceInstalledObject(type, tile), buildTime));
//
		JobHandler.AddJob(job);
	}

	private void SetupDismantleOrder(Tile tile) {
		if(tile.GetInstalledObject() == null)
			return;

		string type = tile.GetInstalledObject().GetObjectType();
		float dismantleTime = GetJobTime(type, ActionMode.Dismantle);

		Job job = new Job(JobTypes.Dismantle, null, tile, -1, 1);
		job.AddTask(new Task_MoveTo(tile));
		job.AddTask(new Task_DoAction(() => WorldController.Instance.GetWorld().DemolishInstalledObject(tile), dismantleTime));

		JobHandler.AddJob(job);
	}

	public static float GetJobTime(string type, ActionMode mode) {
		float time = 0f;
		if(!InstaBuild) {
			string val = Defs.GetDef(type).Properties.GetValue((mode == ActionMode.Dismantle) ? "DismantleTime" : "ConstructionTime");
			if(val != string.Empty)
				time = float.Parse(val);
		}

		return time;
	}

	public ActionMode GetBuildMode() {
        return ActionMode;
    }

    public void SetBuildMode(ActionMode actionMode) {
        ActionMode = actionMode;
		ObjectType = string.Empty;
    }

    public BuildMethod GetBuildMethod() {
		if(ActionMode == ActionMode.Dismantle)
			return BuildMethod.Grid;

		if(ActionMode == ActionMode.Tile)
			return BuildMethod.Grid;

	    if(ActionMode == ActionMode.InstalledObject) {
		    InstalledObject io = (InstalledObject) Defs.GetDef(this.ObjectType).Properties.Prototype;
		    if(io != null)
			    return io.GetBuildMethod(this.ObjectType);
	    }

		return BuildMethod.Single;
    }

    public void PlaceTile(string type) {
        ActionMode = ActionMode.Tile;
        ObjectType = type;
    }

    public void PlaceInstalledObject(string type) {
        ActionMode = ActionMode.InstalledObject;
        ObjectType = type;
    }

    public void PlaceCharacter() {
        //TODO: Note that this is temporary, in final game players wont be able to place new characters
        ActionMode = ActionMode.Character;
        ObjectType = "character";
    }

	public void SetDemolish() {
		ActionMode = ActionMode.Dismantle;
		ObjectType = string.Empty;
	}

	public void SetDestroy() {
		ActionMode = ActionMode.Destroy;
		ObjectType = string.Empty;
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