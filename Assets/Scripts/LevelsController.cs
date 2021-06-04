using System.IO;
using UnityEngine;

public class LevelsController : MonoBehaviour
{
    [SerializeField]
    private LevelsGroup levels;

    public class UserProgress
    {
        public int lastLevel;

        public UserProgress(int progress)
        {
            lastLevel = progress;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        int lastLevel = 0;
        string path = Path.Combine(Application.persistentDataPath, "progress.json");
        Debug.Log($"Read user progress from {path}");
        if (File.Exists(path))
        {
            // Try load User progress
            StreamReader reader = new StreamReader(path);
            UserProgress progress = JsonUtility.FromJson<UserProgress>(reader.ReadToEnd());
            reader.Close();
            lastLevel = progress.lastLevel;
        }
        else
        {
            StreamWriter writer = new StreamWriter(path);
            string progress = JsonUtility.ToJson(new UserProgress(lastLevel));
            writer.Write(progress);
            writer.Close();
        }

        levels.SetProgress(lastLevel);
        
        levels.OnStartLevel += StartLevel;
        
        //levels.InvokeStartLevel(0);
    }
    
    private void StartLevel(LevelSettings level)
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        
    }
}
