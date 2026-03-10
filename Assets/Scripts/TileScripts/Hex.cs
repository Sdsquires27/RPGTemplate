using UnityEngine;

public class Hex
{
    public Vector2Int axial { get; private set; }
    public int r {get { return (int)axial.y; } }
    public int q {get { return (int)axial.x; } }
    public int s {get { return (int)(-axial.x - axial.y); } }
    public Hex(Vector2Int axial)
    {
        this.axial = axial;
    }
}
