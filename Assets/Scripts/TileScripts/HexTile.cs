using UnityEngine;

public class HexTile : MonoBehaviour
{
    public Hex hex { get; private set; }

    public bool isWalkable { get; private set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public void Init(Vector2Int axial)
    {
        hex = new Hex(axial);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    Vector3 hexCorner(Vector3 center, float size, int i)
    {
        float angle_deg = 60 * i; // flat-top starts at 0°
        float angle_rad = Mathf.PI / 180 * angle_deg;
        return new Vector3(center.x + size * Mathf.Cos(angle_rad),
                        center.y + size * Mathf.Sin(angle_rad),
                        center.z);
    }

    void OnDrawGizmos()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        if (sr == null) return;
        float size = sr.sprite.bounds.size.y / 2f; // matches hexSize calculation
        for (int i = 0; i < 7; i++)
        {
            Gizmos.DrawLine(
                hexCorner(transform.position, size, i % 6),
                hexCorner(transform.position, size, (i + 1) % 6)
            );
        }
    }
}
