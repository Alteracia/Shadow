using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Camera))]
public class CameraRayCaster : MonoBehaviour
{
    public static CameraRayCaster Instance;

    private bool _active;
    private Camera _camera;
    private Transform _hitted;

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        _camera = GetComponent<Camera>();
    }

    public void ActivateRayCast(bool active)
    {
        _active = active;
       if (!_active) DeselectObject();
    }

    public void SelectObject(Transform transf)
    {
        // Always one object
        DeselectObject();
        
        _hitted = transf.transform;
        IPointerEnterHandler enter = _hitted.GetComponent<IPointerEnterHandler>();
        if (enter != null) enter.OnPointerEnter(null);
    }

    public void DeselectObject()
    {
        if (!_hitted) return;
        
        IPointerExitHandler exit = _hitted.GetComponent<IPointerExitHandler>();
        if (exit != null) exit.OnPointerExit(null);
        _hitted = null;
    }

    public void RayCastFromPointerPosition(InputAction.CallbackContext context)
    {
        if (!_active) return;
        
        Vector2 mouseScreenPosition = Pointer.current.position.ReadValue();
        
        Ray ray = _camera.ScreenPointToRay(
            new Vector3(mouseScreenPosition.x, mouseScreenPosition.y, 0f));
        if (Physics.Raycast(ray, out var hit))
        {
            if (_hitted != hit.transform)
            {
                SelectObject(hit.transform);
            }
        }
    }
}
