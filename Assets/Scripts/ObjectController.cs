using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 0.01f;
    
    [SerializeField]
    private LevelsGroup levels;
    
    private bool _lockRotationY;
    private bool _lockMove;

    public delegate void Moved(Vector2 delta);
    public event Moved OnMove;


    // Start is called before the first frame update
    void Start()
    {
        levels.OnStartLevel += SetUpForLevel;
        
        levels.InvokeStartLevel(0);
    }
    
    private void SetUpForLevel(LevelSettings level)
    {
        OnMove = null;
        
        _lockRotationY = level.Difficulty < 1;
        _lockMove = level.Difficulty < 2;
        
        // Place object
        foreach (var po in level.Objects)
        {
            PuzzleObject placed = ObjectPool.Instance.PlaceObject(po);
            OnMove += placed.Move;
        }
    }

    public void MoveObject(InputAction.CallbackContext context)//InputValue value)
    {
        if (_lockMove || !Keyboard.current.ctrlKey.isPressed) return;
        
        OnMove?.Invoke(context.ReadValue<Vector2>() * moveSpeed);
    }

}
