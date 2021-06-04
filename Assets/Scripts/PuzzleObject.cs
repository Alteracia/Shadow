using UnityEngine;

public class PuzzleObject : MonoBehaviour
{
    public string id;
    
    [SerializeField]
    private Vector3 startPosition;
    [SerializeField]
    private Vector3 startRotation;
    
    public void Reset()
    {
        this.transform.localPosition = startPosition;
        this.transform.localRotation = Quaternion.Euler(startRotation);
    }

    public void Rotate(Vector2 anglesDelta)
    {
        
    }

    public void Move(Vector2 positionDelta)
    {
        this.transform.localPosition -= new Vector3(positionDelta.x, 0f, positionDelta.y);
    }
}
