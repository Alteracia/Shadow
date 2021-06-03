using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelsController : MonoBehaviour
{
    [SerializeField]
    private LevelsGroup levels;

    // Start is called before the first frame update
    void Start()
    {
        levels.OnStartLevel += StartLevel;
        
        levels.InvokeStartLevel(0);
    }
    
    private void StartLevel(LevelSettings level)
    {
        // SetUp controls
        
        // Place object
        foreach (var po in level.Objects)
        {
            ObjectPool.Instance.PlaceObject(po);
        }
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
