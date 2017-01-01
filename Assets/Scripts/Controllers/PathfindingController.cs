using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Pathfinding))]
public class PathfindingController : MonoBehaviour {

    //TODO: Use 2d raycasting to optimse path (eg. if we can raycast to two tiles, with one inbetween, in path then remove the path tiles inbetween)

    public static PathfindingController Instance;

    private Pathfinding Pathfinding;

    void Awake() {
        Instance = this;
        Pathfinding = GetComponent<Pathfinding>();
    }

    public List<Tile> RequestPath(Tile start, Tile end) {
        return Pathfinding.FindPathInstant(start, end);
    }

    public Tile FindAccessibleObject(Tile start, string objectType) {
		List<Tile> path = Pathfinding.FindPathInstant(start, null, objectType);
	    return path[path.Count - 1];
    }

    public bool PathStillValid(List<Tile> path) {
        return Pathfinding.PathStillValid(path);
    }

    private void OnDrawGizmosSelected() {
        if(Pathfinding == null || Pathfinding.GetGrid() == null)
            return;

        for(int x = 0; x < Pathfinding.GetGrid().GetWidth(); x++) {
            for(int y = 0; y < Pathfinding.GetGrid().GetHeight(); y++) {
                Node node = Pathfinding.GetGrid().GetNode(x, y);
                Gizmos.color = (node.isWalkable) ? Color.green : Color.red;
                Gizmos.DrawCube(new Vector3(x, y, 0), new Vector3(0.25f, 0.25f, 0.01f));
//                Handles.Label(new Vector3(x + 0.25f, y - 0.25f, 0), node.movementPenalty.ToString());
            }
        }
    }

    public Pathfinding GetPathfinding() {
        return Pathfinding;
    }

}
