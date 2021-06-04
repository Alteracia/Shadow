using UnityEngine;

[CreateAssetMenu(fileName = "LevelsGroup", menuName = "ScriptableObjects/LevelsGroup", order = 0)]
public class LevelsGroup : ScriptableObject
{
    [SerializeField] private LevelSettings[] levels;

    public delegate void StartLevel(LevelSettings level);
    public event StartLevel OnStartLevel;
    
    public delegate void UnlockLevel(LevelSettings level);
    public event UnlockLevel OnUnlockLevel;

    public void InvokeStartLevel(int level)
    {
        // Check range
        if (level >= levels.Length || level < 0) return;
        // Check lock
        if (levels[level].locked) return;
        // Call
        OnStartLevel?.Invoke(levels[level]);
    }

    public void SetProgress(int progress)
    {
        // Check range
        if (progress >= levels.Length || progress < 0) return;
        // Unlock
        for (int i = 0; i < levels.Length; i++)
        {
            levels[i].locked = i + 1 < progress;
            if (!levels[i].locked)
                OnUnlockLevel?.Invoke(levels[i]);
        }
    }
}
