using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HPBarLoc : MonoBehaviour
{
    // Start is called before the first frame update
    public float Offset2d = 25;
    public float Offset3d;
    [HideInInspector]public Vector3 location;
    private void Awake()
    {
        location = transform.position + Vector3.up * Offset3d;
    }
}
