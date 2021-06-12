using UnityEngine;
using UnityEngine.InputSystem;

public class ObjectController : MonoBehaviour
{
    [SerializeField]
    private float moveSpeed = 0.01f;
    [SerializeField]
    private float rotateSpeed = 0.01f;
    
    [SerializeField]
    private LevelsGroup levels;
    
    private bool _lockRotationY;
    private bool _lockMove;

    public delegate void Moved(Vector2 delta);
    public static event Moved OnMove;
    
    public delegate void RotateX(float delta);
    public static event RotateX OnRotateX;
    
    public delegate void RotateY(float delta);
    public static event RotateY OnRotateY;
    
    void Awake()
    {
        levels.SubscribeToLevelStart(SetUpForLevel);
    }

    private void Unsubscribe()
    {
        OnMove = null;
        OnRotateX = null;
        OnRotateY = null;
    }
    
    private void SetUpForLevel(LevelSettings level)
    {
        Unsubscribe();
        
        _lockRotationY = level.Difficulty < 1;
        _lockMove = level.Difficulty < 2;
    }

    public void MoveObject(InputAction.CallbackContext context)
    {
        if (_lockMove || !Mouse.current.leftButton.isPressed 
                      || !Keyboard.current.shiftKey.isPressed 
                      || Keyboard.current.ctrlKey.isPressed) return;
        
        OnMove?.Invoke(context.ReadValue<Vector2>() * moveSpeed);
    }
    
    public void RotateObject(InputAction.CallbackContext context)
    {
        if (!Mouse.current.leftButton.isPressed 
            || Keyboard.current.shiftKey.isPressed) return;
        
        if (!Keyboard.current.ctrlKey.isPressed)
            OnRotateX?.Invoke(context.ReadValue<Vector2>().x * rotateSpeed);
        else if (!_lockRotationY)
            OnRotateY?.Invoke(context.ReadValue<Vector2>().x * rotateSpeed);
    }
}
