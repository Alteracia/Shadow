using Alteracia.Animation;
using UnityEngine;

[RequireComponent(typeof(AltAnimator))]
public class CameraLook : MonoBehaviour
{
    [SerializeField]
    private LevelsGroup levels;

    [SerializeField]
    private UIScreenEvents winScreenEvent;
    [SerializeField] 
    private UIScreenEvents looseScreenEvent;

    private AltAnimator _animator;
    
    void Start()
    {
        _animator = GetComponent<AltAnimator>();
        levels.SubscribeToLevelStart(LookAtLevel);
        winScreenEvent.OnButton0Event += LookAtMenu;
        looseScreenEvent.OnButton1Event += LookAtMenu;
    }

    private void LookAtLevel(LevelSettings level)
    {
        _animator.Play(Animator.StringToHash("level"));
    }

    private void LookAtMenu()
    {
        _animator.Play(Animator.StringToHash("menu"));
    }
}
