using UnityEngine;

[CreateAssetMenu(fileName = "Level_", menuName = "ScriptableObjects/LevelSettings", order = 1)]
public class LevelSettings : ScriptableObject
{
    public bool locked = true;
    
    [SerializeField][Range(0, 3)]
    private int difficulty = 0;
    public int Difficulty => difficulty;

    [SerializeField] 
    private string hint;
    public string Hint => hint;

    [SerializeField] 
    private PuzzleObject[] objects;
    public PuzzleObject[] Objects => objects;
}
