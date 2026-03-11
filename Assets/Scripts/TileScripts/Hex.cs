using UnityEngine;

[System.Serializable]
public class Hex
{
    public Vector2Int axial;
    public int r => axial.y;
    public int q => axial.x;
    public int s => -axial.x - axial.y;

    public Hex(Vector2Int axial)
    {
        this.axial = axial;
    }
}