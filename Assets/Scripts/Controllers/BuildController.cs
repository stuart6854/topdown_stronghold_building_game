using System.Collections.Generic;
using UnityEngine;

public enum BuildMethod {

    Single,
    Line,
    Grid

}

public enum BuildMode {

    None,
    Tile,
    InstalledObject,
    Character

}

public class BuildController : MonoBehaviour {

    public static BuildController Instance;

    public GameObject BuildDragPrefab;

    private BuildMode BuildMode;
    private string ObjectType;

    private List<GameObject> DragPreviewObjects;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        DragPreviewObjects = new List<GameObject>();
        SimplePool.Preload(BuildDragPrefab, 10, this.transform);
    }

    void Update() {
        if(Input.GetKeyUp(KeyCode.Escape)) {
            BuildMode = BuildMode.None;
        }

    }

    public void DoAction(Vector2 start, Vector2 end) {
        if(BuildMode == BuildMode.None)
            return;

        if(BuildMode == BuildMode.Character) {
            Tile tile = WorldController.Instance.GetTileAt(Mathf.RoundToInt(start.x), Mathf.RoundToInt(start.y));

            if(tile != null)
                WorldController.Instance.GetWorld().PlaceCharacter(tile);

        } else {
            ClearDragPreviews();

            GetLoopCoords(start, end, out start, out end);

            for(int x = (int) start.x; x <= end.x; x++) {
                for(int y = (int) start.y; y <= end.y; y++) {
                    Tile t = WorldController.Instance.GetTileAt(x, y);
                    if(t == null) continue;

                    string type = ObjectType;

                    if(BuildMode == BuildMode.Tile) {
                        Job job = new Job(t, j => t.ChangeType(type), 1f, 0);
                        JobController.Instance.AddJob(job, t);
                    } else if(BuildMode == BuildMode.InstalledObject) {
                        Job job = new Job(t,
                            j => WorldController.Instance.GetWorld().PlaceInstalledObject(type, t), 1f, 0);
                        JobController.Instance.AddJob(job, t);
                    }
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

                //Display the building hint on top of this tile
                GameObject go = SimplePool.Spawn(BuildDragPrefab, new Vector3(x, y, -0.1f), Quaternion.identity);
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
                }
            }
        }
    }

    public BuildMode GetBuildMode() {
        return BuildMode;
    }

    public void SetBuildMode(BuildMode buildMode) {
        BuildMode = buildMode;
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
        BuildMode = BuildMode.Character;
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