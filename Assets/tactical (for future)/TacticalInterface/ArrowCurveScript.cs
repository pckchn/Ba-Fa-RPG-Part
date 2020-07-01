using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class ArrowCurveScript : MonoBehaviour
{
    LineRenderer lr;
    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

}
