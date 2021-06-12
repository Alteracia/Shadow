using System;
using System.IO;
using UnityEngine;

public class LevelsController : MonoBehaviour
{
    [SerializeField]
    private LevelsGroup levels;
    [SerializeField] 
    private UIScreenEvents startScreenEvent;
    [SerializeField] 
    private UIScreenEvents winScreenEvent;
    [SerializeField] 
    private UIScreenEvents looseScreenEvent;

    private LevelSettings _current;
    
    [System.Serializable]
    private class UserProgress
    {
        public int lastLevel = 0;

        public UserProgress(int progress)
        {
            lastLevel = progress;
        }

        public void UpdateProgress(int level)
        {
            if (level < lastLevel) return;

            lastLevel = level;
            
            SaveProgress(lastLevel);
        }
        
        private static void SaveProgress(int lastLevel)
        {
            string path2 = Path.Combine(Application.persistentDataPath, "progress.json");
            StreamWriter writer = new StreamWriter(path2);
            string progress = JsonUtility.ToJson(new UserProgress(lastLevel));
            writer.Write(progress);
            writer.Close();
        }
    }

    private readonly UserProgress _progress = new UserProgress(0);

    private bool _menu;

    private void Start()
    {
        levels.SubscribeToLevelStart(StartLevel);

        startScreenEvent.OnButton0Event += StartGame;
        startScreenEvent.OnButton1Event += StartTestGame;
        
        looseScreenEvent.OnButton1Event += () =>
        {
            if (_menu) Application.Quit();
            else FinishLevel();
        };
    }

    void StartGame()
    {
        _menu = true;
        
        string path = Path.Combine(Application.persistentDataPath, "progress.json");
        Debug.Log(path);
        
        if (File.Exists(path))
        {
            // Try load User progress
            StreamReader reader = new StreamReader(path);
            JsonUtility.FromJsonOverwrite(reader.ReadToEnd(), _progress);
            reader.Close();
        }
        else _progress.UpdateProgress(0);

        levels.SetProgress(_progress.lastLevel);
        
        winScreenEvent.OnButton0Event += () =>
        {
            _progress.UpdateProgress(levels.SetProgress(_current));
            FinishLevel();
        };
        winScreenEvent.OnButton1Event += () =>
        {
            _progress.UpdateProgress(levels.SetProgress(_current));
            Application.Quit();
        };
    }
    
    void StartTestGame()
    {
        _menu = true;

        levels.SetProgress(3);
        
        winScreenEvent.OnButton0Event += FinishLevel;
        winScreenEvent.OnButton1Event += Application.Quit;
    }

    private void StartLevel(LevelSettings level)
    {
        _menu = false;
        _current = level;
        
        // Place object
        foreach (var po in level.Objects)
        {
            PuzzleObject placed = ObjectPool.Instance.PlaceObject(po);
            placed.OnWon += level.InvokeLevelSolved;
        }
        
        if (ObjectPool.Instance.InScene[0])
            CameraRayCaster.Instance.SelectObject(ObjectPool.Instance.InScene[0].transform);
    }

    private void FinishLevel()
    {
        ObjectPool.Instance.HideSceneObjects();
        CameraRayCaster.Instance.DeselectObject();
        _menu = true;
    }
}
