using Unity.VisualScripting;
using UnityEngine;

public class LineManager : MonoBehaviour
{
    [Header("Line Settings")]
    [SerializeField] int size = 1;
    [SerializeField] Vector3 pos = Vector3.zero;
    LineRenderer lineRendrer;


    Vector3 flatHexCorner(Vector3 center, float size, int i)
    {
        float angle_deg = 60 * i;
        float angle_rad = Mathf.PI / 180 * angle_deg;
        return new Vector3(center.x + size * Mathf.Cos(angle_rad),
                           center.y + size * Mathf.Sin(angle_rad),
                           center.z);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        lineRendrer = gameObject.AddComponent<LineRenderer>();
        lineRendrer.positionCount = 7; // 6 corners + 1 to close the loop
        lineRendrer.widthMultiplier = 0.1f;
        lineRendrer.useWorldSpace = true;

        for (int i = 0; i < 7; i++)
        {
            lineRendrer.SetPosition(i, flatHexCorner(pos, size, i % 6));
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
