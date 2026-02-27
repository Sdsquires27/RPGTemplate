using UnityEngine;
using System.Collections.Generic;
using Unity.VisualScripting;

public class HexGrid : MonoBehaviour
{
    public Dictionary<Vector2, HexTile> hexTiles  {get; private set;}
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region HexMath
    Hex axialSubtract(Hex a, Hex b)
    {
        return new Hex(new Vector2(a.axial.x - b.axial.x, a.axial.y - b.axial.y));
    }
    Hex axialAdd(Hex hex, Hex vec)
    {
        return new Hex(new Vector2(hex.axial.x + vec.axial.x, hex.axial.y + vec.axial.y));
    }
    int axialDistance(Hex a, Hex b)
    {
        Hex diff = axialSubtract(a, b);
        return (int)(Mathf.Abs(diff.axial.x) + Mathf.Abs(diff.axial.y) + Mathf.Abs(findSCoordinate(diff.axial))) / 2;
    }

    float axialDistance(HexTile a, HexTile b)
    {
        return axialDistance(a.hex, b.hex);
    }

    float findSCoordinate(Vector2 axial)
    {
        return -axial.x - axial.y;
    }
    #endregion

    #region LineDraw
    float lerp(float a, float b, float t)
    {
        return a + (b - a) * t;
    }

    Hex hexLerp(Hex a, Hex b, float t)
    {
        return new Hex(new Vector2(lerp(a.axial.x, b.axial.x, t), lerp(a.axial.y, b.axial.y, t)));
    }

    Hex[] hexLineDraw(Hex a, Hex b)
    {
        int N = axialDistance(a, b);
        Hex[] results = new Hex[N + 2];
        Hex aNudge = new Hex(new Vector2(a.axial.x + 0.000001f, a.axial.y + 0.000001f));
        results[0] = aNudge;
        for (int i = 0; i <= N; i++)
        {
            results[i] = hexLerp(a, b, 1f/N * i);
        }
        return results;
    }
    #endregion

    #region OtherHexFunctions
    Hex[] movementRange(Hex center, int range)
    {
        List<Hex> results = new List<Hex>();
        for (int q = -range; q <= range; q++)
        {
            int rMin = Mathf.Max(-range, -q - range);
            int rMax = Mathf.Min(range, -q + range);
            for (int r = rMin; r <= rMax; r++)
            {
                results.Add(axialAdd(center, new Hex(new Vector2(q, r))));
            }
        }
        return results.ToArray();
    }
    #endregion
}


