using UnityEngine;

public abstract class MonoBehaviourWrapper : MonoBehaviour
{
    protected Rigidbody2D RB;
    protected BoxCollider2D BC;
    protected virtual void Start()
    {
        RB = GetComponent<Rigidbody2D>();
        BC = GetComponent<BoxCollider2D>();
    }
}
