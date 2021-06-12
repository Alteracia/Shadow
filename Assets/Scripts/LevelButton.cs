using Alteracia.Animation;
using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(AltAnimator))]
public class LevelButton : MonoBehaviour, IPointerEnterHandler
{
    [SerializeField] 
    private LevelSettings level;

    private AltAnimator _animator;
    
    void Awake()
    {
        _animator = GetComponent<AltAnimator>();
        level.OnUnlockLevel += this.OnUnlock;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        level.InvokeStartLevel();
    }

    private void OnUnlock(LevelSettings level)
    {
        _animator.Play(Animator.StringToHash("unlock"));
    }
}
