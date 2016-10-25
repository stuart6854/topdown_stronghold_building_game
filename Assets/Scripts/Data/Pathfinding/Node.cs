using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>{

    public readonly Tile tile;

    public readonly bool isWalkable;
    public readonly float movementPenalty;

    public float gCost, hCost;

    public Node parent;

    int heapIndex;

    public Node(Tile tile, bool _isWalkable, float _penalty) {
        this.tile = tile;
        this.isWalkable = _isWalkable;
        this.movementPenalty = _penalty;
    }

    public float fCost {
        get {
            return gCost + hCost;
        }
    }

    public int HeapIndex {
        get {
            return heapIndex;
        }
        set {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare) {
        int compare = fCost.CompareTo(nodeToCompare.fCost);
        if(compare == 0) {
            compare = hCost.CompareTo(nodeToCompare.hCost);
        }
        return -compare;
    }

}
