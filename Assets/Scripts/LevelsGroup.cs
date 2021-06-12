using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem.Controls;

[CreateAssetMenu(fileName = "LevelsGroup", menuName = "ScriptableObjects/LevelsGroup", order = 0)]
public class LevelsGroup : ScriptableObject
{
    [SerializeField] 
    private LevelSettings[] levels;
    
    public void SubscribeToLevelStart(Action<LevelSettings> callback)
    {
        foreach (var level in levels)
        {
            level.OnStartLevel += callback.Invoke;
        }
    }
    
    public void SubscribeToLevelFinish(Action<LevelSettings, bool> callback)
    {
        foreach (var level in levels)
        {
            level.OnFinishLevel += callback.Invoke;
        }
    }

    public void SetProgress(int progress)
    {
        // Check range
        progress = Mathf.Clamp(progress, 0, levels.Length - 1);
        // Set Lock/Unlock
        for (int i = 0; i < levels.Length; i++)
        {
            if (i <= progress) levels[i].InvokeUnlockLevel();
            else levels[i].LockLevel();
        }
    }

    public int SetProgress(LevelSettings level)
    {
        int index = levels.ToList().IndexOf(level);
        
        if (index + 1 < levels.Length)
        {
            index++;
            levels[index].InvokeUnlockLevel();
        }
        
        return index;
    }
}
