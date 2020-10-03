using UnityEngine;

public class AnchorController : MonoBehaviourWrapper
{
    public void positionIt(Vector2 pos)
    {
        transform.position = pos;
    }
}
