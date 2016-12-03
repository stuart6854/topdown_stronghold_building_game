using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid {

    //References

    //Data
    private Node[,] grid;
    private int Width;
    private int Height;

    public Grid(int width, int height) {
        this.Width = width;
        this.Height = height;

        CreateGrid();

        WorldController.Instance.GetWorld().RegOnWOUpdatedCB(OnTileChanged);
    }

    private void CreateGrid() {
        grid = new Node[Width, Height];

        for(int x = 0; x < Width; x++) {
            for(int y = 0; y < Height; y++) {
                Tile tile = WorldController.Instance.GetTileAt(x, y);

                grid[x, y] = new Node(tile, IsTileWalkable(tile), (int)(tile.GetMovementMultiplier() * 10f));
            }
        }
    }

    public Node GetNode(int x, int y) {
        return grid[x, y];
    }

    public List<Node> GetNeighbours(Node node) {
        List<Node> neighbours = new List<Node>();

        for(int x = -1; x <= 1; x++) {
            for(int y = -1; y <= 1; y++) {
                if(x == 0 && y == 0)
                    continue;

                int checkX = (int)node.tile.GetX() + x;
                int checkY = (int)node.tile.GetY() + y;
                if(checkX >= 0 && checkX < Width && checkY >= 0 && checkY < Height) {
                    neighbours.Add(grid[checkX, checkY]);
                }
            }
        }

        return neighbours;
    }

    private bool IsTileWalkable(Tile tile) {
        if(tile.GetInstalledObject() != null)
            if(tile.GetInstalledObject().GetMovementMultiplier() == 0)
                return false;

        if(tile.GetMovementMultiplier() == 0)
            return false;

        return true;
    }

    public void OnTileChanged(WorldObject worldObject) {
        Tile tile = null;
        if(worldObject is Tile)
            tile = (Tile) worldObject;
        else if(worldObject is InstalledObject) {
            InstalledObject io = (InstalledObject) worldObject;
            tile = io.GetTile();
        }

        if(tile != null)
            grid[(int)worldObject.GetX(), (int)worldObject.GetY()] = new Node(tile, IsTileWalkable(tile), (int)(tile.GetMovementMultiplier() * 10f));
    }

    public int GetWidth() {
        return Width;
    }

    public int GetHeight() {
        return Height;
    }

}
