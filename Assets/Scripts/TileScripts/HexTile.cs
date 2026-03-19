using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class HexTile : MonoBehaviour
{
    [SerializeField] private bool _isWalkable = true;
    public bool isWalkable => _isWalkable;

    public int movementCost { get; private set; } = 1;
    [SerializeField] private Hex _hex;
    public Hex hex => _hex;
    // In HexTile
    private SpriteRenderer sr;
    private static readonly int OutlineColor = Shader.PropertyToID("_OutlineColor");

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    public void SetOutlineColor(Color color)
    {
        // Use MaterialPropertyBlock to avoid creating new material instances
        MaterialPropertyBlock block = new MaterialPropertyBlock();
        sr.GetPropertyBlock(block);
        block.SetColor(OutlineColor, color);
        sr.SetPropertyBlock(block);
    }

    public void HideOutline()
    {
        SetOutlineColor(Color.clear);
    }

    public void Init(Vector2Int axial)
    {
        _hex = new Hex(axial);
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
