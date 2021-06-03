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

    public void Rotate(Vector3 vector)
    {
        
    }

    public void Move(Vector3 position)
    {
        
    }
}
