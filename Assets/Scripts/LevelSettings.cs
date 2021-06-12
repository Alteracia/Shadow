using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "Level_", menuName = "ScriptableObjects/LevelSettings", order = 1)]
public class LevelSettings : ScriptableObject
{
    [SerializeField]
    private bool locked = true;
    public bool Locked => locked;
    
    [SerializeField][Range(0, 3)]
    private int difficulty = 0;
    public int Difficulty => difficulty;

    [SerializeField] 
    private string hint;
    public string Hint => hint;

    [SerializeField] 
    private PuzzleObject[] objects;
    public PuzzleObject[] Objects => objects;
    
    public delegate void StartLevel(LevelSettings level);
    public event StartLevel OnStartLevel;

    public delegate void FinishLevel(LevelSettings level, bool win);
    public event FinishLevel OnFinishLevel;
    
    public delegate void UnlockLevel(LevelSettings level);
    public event UnlockLevel OnUnlockLevel;

    public void LockLevel()
    {
        locked = true;
    }
    
    public void InvokeStartLevel()
    {
        if (locked) return;
        
        OnStartLevel?.Invoke(this);
    }

    public void InvokeLevelSolved()
    {
        if (ObjectPool.Instance.InScene.Any(obj => !obj.isWin))
            return;

        OnFinishLevel?.Invoke(this, true);
    }

    public void InvokeUnlockLevel()
    {
        locked = false;
        OnUnlockLevel?.Invoke(this);
    }
}
