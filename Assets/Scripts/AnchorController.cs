﻿using UnityEngine;

public class AnchorController : MonoBehaviourWrapper
{
    public void positionIt(Vector2 coord)
    {
        transform.position = coord;
    }
}
