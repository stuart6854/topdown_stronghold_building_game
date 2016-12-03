using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

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

	public bool PathStillValid(List<Tile> path) {
		for(int i = 0; i < path.Count; i++) {
			Tile tile = path[i];

			if(!Grid.GetNode((int)tile.GetX(), (int)tile.GetY()).isWalkable)
				return false;
		}

		return true;
	}

	public List<Tile> FindPathInstant(Tile start, Tile end, string objectType = null) {
	    //If 'end' is null, then this A* algorithm becomes a Dijkstra Algorithm,
	    //which is A* without the heuristics

		Stopwatch sw = new Stopwatch();
		sw.Start();

		bool pathSuccess = false;

		Node startNode = Grid.GetNode((int)start.GetX(), (int)start.GetY());
	    Node targetNode = null;
	    if(end != null)
		    targetNode = Grid.GetNode((int)end.GetX(), (int)end.GetY());

		if((targetNode != null && targetNode.isWalkable) || objectType != null) {
			Heap<Node> openSet = new Heap<Node>(Grid.GetWidth() * Grid.GetHeight());
			HashSet<Node> closedSet = new HashSet<Node>();
			openSet.Add(startNode);

			while(openSet.Count > 0) {
				Node currentNode = openSet.RemoveFirst();

				closedSet.Add(currentNode);

				if(targetNode != null && currentNode == targetNode) {
					pathSuccess = true;
					break;//Path found
				}
			    if(targetNode == null) {
				    if(currentNode.tile.GetLooseItem() != null && currentNode.tile.GetLooseItem().GetObjectType() == objectType) {
				        //We have found what we are looking for
				        targetNode = currentNode;
				        pathSuccess = true;
				        break;//Path found
				    }
				}

				foreach(Node neighbour in Grid.GetNeighbours(currentNode)) {
					if(!neighbour.isWalkable || WillCutCorner(currentNode, neighbour) || closedSet.Contains(neighbour))
						continue;

				    float newMovementCostToNeighbour = neighbour.movementPenalty * GetDistance(currentNode, neighbour); //currentNode.gCost + GetDistance(currentNode, neighbour) - neighbour.movementPenalty;
				    float tempGScore = currentNode.gCost + newMovementCostToNeighbour;

				    if(openSet.Contains(neighbour) && tempGScore >= neighbour.gCost)
				        continue;

				    neighbour.parent = currentNode;
				    neighbour.gCost = tempGScore;
                    neighbour.hCost = GetHeuristic(neighbour, targetNode);

                    if(!openSet.Contains(neighbour))
                        openSet.Add(neighbour);
                    else
                        openSet.UpdateItem(neighbour);

				}
			}
		}


	    sw.Stop();
//		print("Path Found: " + sw.ElapsedMilliseconds + "ms.");

	    List<Tile> waypoints = null;

		if(pathSuccess) {
			waypoints = RetracePath(startNode, targetNode);
		}

		if(waypoints != null && waypoints.Count == 0)
			waypoints = null;

		return waypoints;
	}

    List<Tile> RetracePath(Node startNode, Node endNode) {
		List<Tile> path = new List<Tile>();
		Node currentNode = endNode;
		while(currentNode != startNode) {
			path.Add(currentNode.tile);
			currentNode = currentNode.parent;
		}
        path.Add(currentNode.tile);
        path.Reverse();
		return path;
	}

    private float GetHeuristic(Node start, Node end) {
        if(end == null) {
            // We have no fixed destination (i.e. probably looking for an inventory item)
            // so just return 0 for the cost estimate (i.e. all directions as just as good)
            return 0f;
        }

        return Mathf.Sqrt(Mathf.Pow(start.tile.GetX() - end.tile.GetX(), 2) +
                          Mathf.Pow(start.tile.GetX() - end.tile.GetY(), 2));
    }

	float GetDistance(Node nodeA, Node nodeB) {
		int distX = Mathf.Abs((int)nodeA.tile.GetX() - (int)nodeB.tile.GetX());
		int distY = Mathf.Abs((int)nodeA.tile.GetY() - (int)nodeB.tile.GetY());

	    //Horizontal and Vertical Neighbours have distance of 1
	    if(distX + distY == 1)
	        return 1f;

	    //Diagonal Neighbours have a distance of 1.41421356237
	    if(distX == 1 && distY == 1)
	        return 1.41421356237f;

	    //Otherwise, just do the math
	    return Mathf.Sqrt(Mathf.Pow(nodeA.tile.GetX() - nodeB.tile.GetX(), 2) +
	                      Mathf.Pow(nodeA.tile.GetX() - nodeB.tile.GetY(), 2));
	}

	private bool WillCutCorner(Node currNode, Node neighbourNode ) {

		int dX = (int)currNode.tile.GetX() - (int)neighbourNode.tile.GetX();
		int dY = (int)currNode.tile.GetY() - (int)neighbourNode.tile.GetY();

		if(Mathf.Abs(dX) + Mathf.Abs(dY) == 2) {
			//We are Diagonal

			if(!Grid.GetNode((int)currNode.tile.GetX() - dX, (int)currNode.tile.GetY()).isWalkable) {
				//East or West are un-walkable, so we would be cutting a corner
				return true;
			}

			if(!Grid.GetNode((int)currNode.tile.GetX(), (int)currNode.tile.GetY() - dY).isWalkable) {
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
