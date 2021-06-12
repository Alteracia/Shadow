using UnityEngine;

[CreateAssetMenu(fileName = "UIEvents", menuName = "ScriptableObjects/UIEvents", order = 2)]
public class UIScreenEvents : ScriptableObject
{
    public bool win;
    
    public delegate void Button0Event();

    public event Button0Event OnButton0Event;

    public void Invoke0Event()
    {
        OnButton0Event?.Invoke();
    }
    
    public delegate void Button1Event();

    public event Button1Event OnButton1Event;

    public void Invoke1Event()
    {
        OnButton1Event?.Invoke();
    }
}
