using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackDictionary : MonoBehaviour
{
    [System.Serializable]
    public struct Attack
    {
        public float manaDemand;
        public float damage;
        public float critChance;
        public bool comboFriendly;
    }
    public Dictionary<string, Attack> dictionary = new Dictionary<string, Attack>();
    // Start is called before the first frame update
    private void Awake()
    {
        
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
