// Assets/Scripts/Camera/CameraController.cs
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Follow Settings")]
    [SerializeField] private float smoothSpeed = 5f;
    [SerializeField] private Vector3 offset = new Vector3(0f, 0f, -10f);

    [Header("Bounds")]
    [SerializeField] private bool useBounds = true;
    [SerializeField] private float minX = -50f;
    [SerializeField] private float maxX =  50f;
    [SerializeField] private float minY = -50f;
    [SerializeField] private float maxY =  50f;


    void Start()
    {
        SnapToTarget();
    }
    
    void LateUpdate()
    {
        if (target == null) return;

        // Desired position
        Vector3 desired = target.position + offset;

        // Smooth toward desired
        Vector3 smoothed = Vector3.Lerp(transform.position, desired, smoothSpeed * Time.deltaTime);

        // Clamp to bounds
        if (useBounds)
        {
            smoothed.x = Mathf.Clamp(smoothed.x, minX, maxX);
            smoothed.y = Mathf.Clamp(smoothed.y, minY, maxY);
        }

        // Keep Z from offset
        smoothed.z = offset.z;

        transform.position = smoothed;
    }

    // Call this to update bounds at runtime e.g. when loading a new map area
    public void SetBounds(float minX, float maxX, float minY, float maxY)
    {
        this.minX = minX;
        this.maxX = maxX;
        this.minY = minY;
        this.maxY = maxY;
    }

    // Call this to instantly snap to the target without smoothing e.g. on scene load
    public void SnapToTarget()
    {
        if (target == null) return;
        Vector3 pos = target.position + offset;
        if (useBounds)
        {
            pos.x = Mathf.Clamp(pos.x, minX, maxX);
            pos.y = Mathf.Clamp(pos.y, minY, maxY);
        }
        pos.z = offset.z;
        transform.position = pos;
    }
}