using UnityEngine;

public class HexTile : MonoBehaviour
{
    public Hex hex { get; private set; }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3 flatHexCorner(Vector3 center, float size, int i)
    {
        float angle_deg = 60 * i;
        float angle_rad = Mathf.PI / 180 * angle_deg;
        return new Vector3(center.x + size * Mathf.Cos(angle_rad),
                           center.y + size * Mathf.Sin(angle_rad),
                           center.z);
    }

    void OnDrawGizmos()
    {
        for (int i = 0; i < 7; i++)
        {
            Gizmos.DrawLine(flatHexCorner(transform.position, transform.localScale.x, i % 6), flatHexCorner(transform.position, transform.localScale.x, (i + 1) % 6));
        }
    }
}
