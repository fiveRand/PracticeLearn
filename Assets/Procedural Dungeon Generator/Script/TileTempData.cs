using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TileTempData
{
    public TileBase tileBase;
    public Matrix4x4 matrix4X4;

    public TileTempData(TileBase tilebase, Matrix4x4 matrix4X4)
    {
        this.tileBase = tilebase;
        this.matrix4X4 = matrix4X4;
    }
}
