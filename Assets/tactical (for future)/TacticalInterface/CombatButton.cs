using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CombatButton : MonoBehaviour
{
    public Vector3 InterfacePosition;
    // Start is called before the first frame update
    private void Awake()
    {
        //transform.GetComponent<Image>().color = new Color(transform.GetComponent<Image>().color.r, transform.GetComponent<Image>().color.g, transform.GetComponent<Image>().color.b, 0);
        transform.GetComponent<SpriteRenderer>().color = new Color(transform.GetComponent<SpriteRenderer>().color.r, transform.GetComponent<SpriteRenderer>().color.g, transform.GetComponent<SpriteRenderer>().color.b, 0);
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
