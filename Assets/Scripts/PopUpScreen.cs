using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class PopUpScreen : MonoBehaviour
{
    [SerializeField] 
    private bool levelPopUp;
    
    [SerializeField] 
    private UIDocument uiDocument;
    
    [SerializeField]
    private LevelsGroup levels;
    
    [SerializeField] 
    private UIScreenEvents popupScreenEvents;
    
    Button _button0;
    Button _button1;

    private VisualElement _popUpScreen;
    private static bool _onScreen;

    void Start()
    {
        var root = uiDocument.rootVisualElement;

        _popUpScreen = root.Q<VisualElement>("Screen");
        
        if (levelPopUp)
        {
            _popUpScreen.style.display = DisplayStyle.None;
            levels.SubscribeToLevelFinish(PopUpShow);
        }
        else _onScreen = true;
        
        _button0 = root.Children().Last().Children().Last().Q<Button>("Button0");
        _button1 = root.Children().Last().Children().Last().Q<Button>("Button1");
        
        // add event handler
        if (_button0 != null && _button1 != null)
        {
            _button0.clickable.clicked += popupScreenEvents.Invoke0Event;
            _button0.clickable.clicked += PopUpButtonPressed;
            
            _button1.clickable.clicked += popupScreenEvents.Invoke1Event;
            _button1.clickable.clicked += PopUpButtonPressed;
        }
        else
        {
            Debug.LogWarning("Can't find buttons");
        }
    }

    private void PopUpButtonPressed()
    {
        _popUpScreen.style.display = DisplayStyle.None;
        _onScreen = false;
        CameraRayCaster.Instance.ActivateRayCast(true);
    }

    private void PopUpShow(LevelSettings level, bool win)
    {
        if (win != popupScreenEvents.win || _onScreen) return;

        _popUpScreen.style.display = DisplayStyle.Flex;
        _onScreen = true;
        CameraRayCaster.Instance.ActivateRayCast(false);
    }

    public void PopUpSwitch(InputAction.CallbackContext context)
    {
        bool visible = _popUpScreen.style.display == DisplayStyle.Flex;
        
        if (_onScreen && !visible) return;
        
        _popUpScreen.style.display = visible ? DisplayStyle.None : DisplayStyle.Flex;
        _onScreen = !visible;
        CameraRayCaster.Instance.ActivateRayCast(!_onScreen);
    }
}
