using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class GridData {

    public GridFieldData[][] gridFieldsData;

    public GridData(Tile[][] grid){
        this.gridFieldsData = new GridFieldData[grid.Length][];
        for(int i = 0; i < grid.Length; i++){
            this.gridFieldsData[i] = new GridFieldData[grid[i].Length];
            Tile[] gridRow = grid[i];
            for (int j = 0; j < gridRow.Length; j++){
                Tile tile = gridRow[j];
                this.gridFieldsData[i][j] = new GridFieldData(tile.x, tile.z, tile.isObstacle);
            }
        }
    }
}

[Serializable]
public class GridFieldData{

    public int xPos;
    public int zPos;
    public bool isObstacle;

    public GridFieldData(int xPos, int zPos, bool isObstacle){
        this.xPos = xPos;
        this.zPos = zPos;
        this.isObstacle = isObstacle;
    }
}