﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Pathfinding : MonoBehaviour {

    public static Pathfinding Instance;

	//References
	private PathfindingController pathController;

	//Data
    private Grid Grid;

    void Awake() {
        Instance = this;
    }

	void Start() {
	    pathController = PathfindingController.Instance;
	    Grid = new Grid(WorldController.WIDTH, WorldController.HEIGHT);
	}

	public bool PathStillValid(Tile[] path, int currentIndex) {
		for(int i = currentIndex; i < path.Length; i++) {
			Tile tile = path[i];

			if(!Grid.GetNode(tile.GetX(), tile.GetY()).isWalkable)
				return false;
		}

		return true;
	}

	public void FindPath(Tile start, Tile end) {
		Tile[] path = FindPathInstant(start, end);
		pathController.FinishedProcessingPath(path, (path != null));
	}

	public Tile[] FindPathInstant(Tile start, Tile end) {
		Stopwatch sw = new Stopwatch();
		sw.Start();

		bool pathSuccess = false;

		Node startNode = Grid.GetNode(start.GetX(), start.GetY());
		Node targetNode = Grid.GetNode(end.GetX(), end.GetY());

		if(targetNode.isWalkable) {
			Heap<Node> openSet = new Heap<Node>(Grid.GetWidth() * Grid.GetHeight());
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);

			while(openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst();

				closedSet.Add(currentNode);

				if(currentNode == targetNode) {
					sw.Stop();
//					print("Path Found: " + sw.ElapsedMilliseconds + "ms.");
					pathSuccess = true;
					break;//Path found
				}

				foreach(Node neighbour in Grid.GetNeighbours(currentNode)) {
					if(!neighbour.isWalkable || WillCutCorner(currentNode, neighbour) || closedSet.Contains(neighbour))
						continue;

					int newMovementCostToNeighbour = currentNode.gCost + GetDistance(currentNode, neighbour) + neighbour.movementPenalty;

					if(newMovementCostToNeighbour < neighbour.gCost || !openSet.Contains(neighbour)) {
						neighbour.gCost = newMovementCostToNeighbour;
						neighbour.hCost = GetDistance(neighbour, targetNode);
						neighbour.parent = currentNode;

						if(!openSet.Contains(neighbour))
							openSet.Add(neighbour);
						else
							openSet.UpdateItem(neighbour);
					}
				}
			}
		}

		Tile[] waypoints = null;

		if(pathSuccess) {
			waypoints = RetracePath(startNode, targetNode);
		}

		if(waypoints != null && waypoints.Length == 0)
			waypoints = null;

		return waypoints;
	}

	Tile[] RetracePath(Node startNode, Node endNode) {
		List<Tile> path = new List<Tile>();
		Node currentNode = endNode;

		while(currentNode != startNode) {
			path.Add(currentNode.tile);
			currentNode = currentNode.parent;
		}
		Tile[] waypoints = path.ToArray();
		Array.Reverse(waypoints);
		return waypoints;
	}

	int GetDistance(Node nodeA, Node nodeB) {
		int distX = Mathf.Abs(nodeA.tile.GetX() - nodeB.tile.GetX());
		int distY = Mathf.Abs(nodeA.tile.GetY() - nodeB.tile.GetY());

		if(distX > distY)
			return 14 * distY + 10 * (distX - distY);

		return 14 * distX + 10 * (distY - distX);
	}

	private bool WillCutCorner(Node currNode, Node neighbourNode ) {

		int dX = currNode.tile.GetX() - neighbourNode.tile.GetX();
		int dY = currNode.tile.GetY() - neighbourNode.tile.GetY();

		if(Mathf.Abs(dX) + Mathf.Abs(dY) == 2) {
			//We are Diagonal

			if(!Grid.GetNode(currNode.tile.GetX() - dX, currNode.tile.GetY()).isWalkable) {
				//East or West are un-walkable, so we would be cutting a corner
				return true;
			}

			if(!Grid.GetNode(currNode.tile.GetX(), currNode.tile.GetY() - dY).isWalkable) {
				//North or South are un-walkable, so we would be cutting a corner
				return true;
			}
		}

		return false;
	}

    public Grid GetGrid() {
        return Grid;
    }

}
