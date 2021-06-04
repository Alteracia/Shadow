using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    
    private readonly List<PuzzleObject> _pool = new List<PuzzleObject>();
    
    // Start is called before the first frame update
    void Awake()
    {
        Instance = this;
    }

    public PuzzleObject PlaceObject(PuzzleObject puzzleObject)
    {
        PuzzleObject current;
        
        if (_pool.Any(po => po.id == puzzleObject.id))
        {
            current = _pool.First(po => po.id == puzzleObject.id);
            current.gameObject.SetActive(true);
        }
        else
        {
            current = Instantiate(puzzleObject, this.transform);
            _pool.Add(current);
        }
        
        current.Reset();
        return current;
    }
}
