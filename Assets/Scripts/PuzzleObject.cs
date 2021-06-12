using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class PuzzleObject : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string id;
    
    [SerializeField]
    private Vector3 startPosition;
    [SerializeField]
    private Vector3 winingRelativePosition;
    [NonSerialized]
    public Transform Previous;
    [SerializeField]
    private Vector3 startRotation;
    [SerializeField] 
    private Vector3 acceptableWiningRotation;

    private bool _isWin;
    public bool isWin => _isWin;

    public delegate void Won();
    public Won OnWon;

    private static readonly float PositionRange = 0.05f;
    private static readonly float RotationRange = 18f;
    private static readonly float PositionLimitRadius = 0.5f;
    
    public void Reset()
    {
        OnWon = null;
        _isWin = false;
        
        this.transform.localPosition = startPosition;
        this.transform.localRotation = Quaternion.Euler(startRotation);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        ObjectController.OnMove += this.Move;
        ObjectController.OnRotateX += this.RotateX;
        ObjectController.OnRotateY += this.RotateY;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        ObjectController.OnMove -= this.Move;
        ObjectController.OnRotateX -= this.RotateX;
        ObjectController.OnRotateY -= this.RotateY;
    }
    
    private void RotateX(float angleDelta)
    {
        Vector3 euler = new Vector3(0f, 0f, angleDelta);
        this.transform.Rotate(euler);
        
        CheckTransform();
    }
    
    private void RotateY(float angleDelta)
    {
        Vector3 euler = new Vector3(angleDelta, 0f, 0f);
        this.transform.Rotate(euler);
        
        CheckTransform();
    }

    private void Move(Vector2 positionDelta)
    {
        Vector3 newPosition = this.transform.localPosition - new Vector3(positionDelta.x, 0f, positionDelta.y);
        
        if (newPosition.magnitude > PositionLimitRadius) return;
        
        this.transform.localPosition = newPosition;
        CheckTransform();
    }
    
    private void CheckTransform()
    {
        Quaternion rot = this.transform.localRotation;
        
        Vector3 relPos = Vector3.zero;
        if (Previous) relPos = this.transform.localPosition - Previous.localPosition;
        
        _isWin = (Quaternion.Angle(rot, Quaternion.identity) < RotationRange
                 || Quaternion.Angle(rot, Quaternion.Euler(acceptableWiningRotation)) < RotationRange)
                 && (!Previous || (Vector3.Angle(relPos, winingRelativePosition) < RotationRange 
                 && Vector3.Distance(relPos, winingRelativePosition) < PositionRange));
        
        if (_isWin) OnWon?.Invoke();
    }
}
