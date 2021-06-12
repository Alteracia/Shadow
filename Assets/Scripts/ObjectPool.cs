using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool Instance;
    
    private readonly List<PuzzleObject> _pool = new List<PuzzleObject>();
    
    private readonly List<PuzzleObject> _inScene = new List<PuzzleObject>();
    public List<PuzzleObject> InScene => _inScene;
    
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
            if (_inScene.Count > 0)
                current.Previous = _inScene[_inScene.Count - 1].transform;
            _pool.Add(current);
        }
        
        current.Reset();
        
        _inScene.Add(current);
        
        return current;
    }

    public void HideSceneObjects()
    {
        foreach (var po in _inScene)
        {
            po.Reset();
            po.gameObject.SetActive(false);
        }
        _inScene.Clear();
    }
}
