using System;
using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEditor;
using UnityEngine;

[RequireComponent(typeof(Pathfinding))]
public class PathfindingController : MonoBehaviour {

    //TODO: Use 2d raycasting to optimse path (eg. if we can raycast to two tiles, with one inbetween, in path then remove the path tiles inbetween)

    public static PathfindingController Instance;

    private Pathfinding Pathfinding;
    private PathRequest CurrentRequest;

    //Data
    private Queue<PathRequest> PathRequests;
    private bool IsProcessing;


    void Awake() {
        Instance = this;
        Pathfinding = GetComponent<Pathfinding>();
    }

    private void Start() {
        PathRequests = new Queue<PathRequest>();
    }

    public void RequestPath(Tile start, Tile end, Action<Tile[], bool> callback) {
        PathRequest request = new PathRequest(start, end, callback);
        PathRequests.Enqueue(request);
        TryProcessNext();
    }

    public bool PathStillValid(Tile[] path, int currentIndex) {
        return Pathfinding.PathStillValid(path, currentIndex);
    }

    private void TryProcessNext() {
        if(!IsProcessing && PathRequests.Count > 0) {
            PathRequest request = PathRequests.Dequeue();
            CurrentRequest = request;
            IsProcessing = true;
            Pathfinding.FindPath(request.pathStart, request.pathEnd);
        }
    }

    public void FinishedProcessingPath(Tile[] path, bool success) {
        CurrentRequest.callback(path, success);
        IsProcessing = false;
        TryProcessNext();
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

    struct PathRequest {
        public Tile pathStart;
        public Tile pathEnd;
        public Action<Tile[], bool> callback;

        public PathRequest(Tile start, Tile end, Action<Tile[], bool> callback) {
            this.pathStart = start;
            this.pathEnd = end;
            this.callback = callback;
        }
    }

}
