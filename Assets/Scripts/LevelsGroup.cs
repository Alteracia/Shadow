using UnityEngine;

[CreateAssetMenu(fileName = "LevelsGroup", menuName = "ScriptableObjects/LevelsGroup", order = 0)]
public class LevelsGroup : ScriptableObject
{
    [SerializeField]
    private LevelSettings[] levels;

    public bool allUnlocked = false;

    public delegate void StartLevel(LevelSettings level);
    public event StartLevel OnStartLevel;
    
    public void InvokeStartLevel(int level)
    {
        // Check range
        if (level >= levels.Length || level < 0) return;
        // Check lock
        if (!allUnlocked && levels[level].locked) return;
        // Call
        OnStartLevel?.Invoke(levels[level]);
    }
}
