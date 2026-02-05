using UnityEngine;

public abstract class ActorScript : MonoBehaviour
{
    #region Variables
    [Header("Actor Settings")]
    [SerializeField]
    protected float moveSpeed = 5f;
    [SerializeField]
    protected int health;
    
    protected Rigidbody2D rb;

    #endregion
    
    protected virtual void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    protected virtual void Update()
    {
        HandleMovement();
        HandleActions();
    }

    // Abstract methods - child classes must implement
    protected abstract void HandleMovement();
    protected abstract void HandleActions();
    
    // Virtual methods - can be overridden
    public virtual void TakeDamage(int damage)
    {
        health -= damage;
        Debug.Log($"{gameObject.name} took {damage} damage. Health: {health}");
    }
}