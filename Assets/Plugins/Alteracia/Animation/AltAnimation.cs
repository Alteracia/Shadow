using System;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace Alteracia.Animation
{
    public enum TargetOperation { Overwrite, Add, Multiply }
    
    public interface IAnimation
    {
        void ChangeFinish(System.Object finish);
        void Play(Action finishCallback = null);
        bool Running { get; }
        void ChangeCallback(Action finishCallback = null);
        Task Wait(bool changeCallback = false, Action finishCallback = null);
        void Stop(bool invokeFinishCallback);
    }
    
    [Serializable]
    public abstract class AltAnimation : ScriptableObject, IAnimation
    {
        /// <summary>
        /// Do nothing or play animation again when Play is called while animation is running.
        /// In both cases callback OnFinish will be overwritten.
        /// </summary>
        [Header("Behavior")]
        [SerializeField]
        private bool playOver = false;
        
        [SerializeField] 
        private bool loop = false;
        
        [SerializeField]
        private bool swing = false;
        
        private enum AnimationProperty { Duration, Speed, Start }
        /// <summary>
        /// Duration - duration always the same. Start property has no affect on animation.
        /// Speed - new duration will calculate from current path. Start should be specified correctly.
        /// Start - duration always the same. Animation always begin from start. Target will be teleport if it is away from start.
        /// </summary>
        [Header("Time")]
        [SerializeField] 
        private AnimationProperty constant;
        /// <summary>
        /// Delay in seconds once after Play called.
        /// Use curves instead easings to set delay in other place of timeline.
        /// </summary>
        [SerializeField]
        [Range(0f, 5f)]
        private float delay = 0f;
        /// <summary>
        /// Duration in seconds.
        /// </summary>
        [SerializeField]
        [Range(0.0001f, 5f)]
        private float duration = 1f;
        /// <summary>
        /// Duration combine with delay.
        /// How long will be animation playing.
        /// </summary>
        public float Duration => duration + delay;

        private float _calculatedDuration = 0f;
        /// <summary>
        /// Duration which was calculate before animation was last Played
        /// </summary>
        public float CalculatedDuration => _calculatedDuration;

        /// <summary>
        /// Type of easing.
        /// Linear - no easing.
        /// Curve - use AnimationCurve.
        /// </summary>
        [Header("Interpolation")]
        [SerializeField]
        private Alteracia.Logic.AltMath.EasingType easing = Alteracia.Logic.AltMath.EasingType.Linear;
        
        [SerializeField]
        private AnimationCurve curve = AnimationCurve.EaseInOut(0f, 0f, 1f, 1f);


        [Header("Target")]
        [SerializeField]
        private bool multiComponents = false;
        /// <summary>
        /// If true: get all Components
        /// </summary>
        public bool MultiComponents => multiComponents;
        
        [SerializeField]
        private bool excludeSelf = false;
        /// <summary>
        /// If true: exclude parent gameObject
        /// </summary>
        public bool ExcludeSelf => excludeSelf;

        [Tooltip("If empty: target will be the first Component, or all Components")][SerializeField]
        private string gameObjectName;
        /// <summary>
        /// Name of object to get target Component from.
        /// If name is empty: first Component of animator or its child will be set to target.
        /// If no object found: animation will be exclude.
        /// </summary>
        public string GameObjectName => gameObjectName;
        [SerializeField]
        protected TargetOperation operation = TargetOperation.Overwrite;

        protected Component[] components;

        /// <summary>
        /// Target Components 
        /// </summary>
        public Component[] Components // TODO Self init pass Transform
        {
            get => components;
            set => components = value;
        }

        public virtual Type GetComponentType()
        {
            return null;
        }

        private delegate void Finished();
        private event Finished OnFinished;
        
        private CancellationTokenSource _cancelTokenSource = new CancellationTokenSource();
        private Task<bool> _animationRun = null;
        public bool Running => _animationRun != null;

        private float _timer;
        protected float progress;

        public virtual void ChangeFinish(System.Object finish) { } // TODO Rid of
        
        

        public async void Play(Action finishCallback = null)
        {
            // Always change callback
            ChangeCallback(finishCallback);
           
            // IF Task is running and multi play not allowed - break
            if (_animationRun != null)
            {
                // Do nothing
                if (!this.playOver) return ;
                // Cancel running task and Play again
                this.Stop(false);
            }
            
            if (components == null || components.Length == 0 || !this.PrepareTargets())
            {
                InvokeFinishCallback();
                return;
            }
            
            this.PrepareParameters();
           
            // Set Task
            _animationRun = this.RunAnimation();
            // Wait
            bool stopped = await _animationRun;
            // Set Task to null
            _animationRun = null;
            
            if (stopped) return;
            // Callback only if animation was finished by self, not canceled
            InvokeFinishCallback();
        }

        private void PrepareParameters()
        {
            _timer = -delay;
            
            switch (constant)
            {
                case AnimationProperty.Duration:
                    _calculatedDuration = duration;
                    break;
                case AnimationProperty.Speed:
                    UpdateCurrentProgressFromStart();
                    _calculatedDuration = duration * Alteracia.Logic.AltMath.Ease(this.easing, 1 - progress, curve);
                    break;
                case AnimationProperty.Start:
                    _calculatedDuration = duration;
                    SetConstantStart();
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }

            switch (operation)
            {
                case TargetOperation.Overwrite:
                    OverwriteTarget();
                    break;
                case TargetOperation.Add:
                    AddTarget();
                    break;
                case TargetOperation.Multiply:
                    MultiplyTarget();
                    break;
                default:
                    throw new System.ArgumentOutOfRangeException();
            }
            _cancelTokenSource = new CancellationTokenSource();
        }

        protected virtual bool PrepareTargets() { return false; }

        protected virtual void UpdateCurrentProgressFromStart() { }
        protected virtual void SetConstantStart() {}
        
        protected virtual void OverwriteTarget() {}
        protected virtual void AddTarget() {}
        protected virtual void MultiplyTarget() {}
        

        private async Task<bool> RunAnimation()
        {
            while (_timer < _calculatedDuration || loop || (swing && _timer < _calculatedDuration * 2f))
            {
                if (_cancelTokenSource.Token.IsCancellationRequested)
                    return true;
               
                _timer += Time.deltaTime;  // overflow ?

                float alpha = _timer / _calculatedDuration;
                if (loop || swing)
                    alpha = swing ? Mathf.PingPong(alpha, 1f) : Mathf.Repeat(alpha, 1f);
                else
                    alpha = Mathf.Clamp01(alpha);
                
                this.progress = Alteracia.Logic.AltMath.Ease(easing, alpha);
                                
                this.Interpolate();
                
                await Task.Yield();
            }
            
            return false;
        }
        
        protected virtual void Interpolate() { }

        public void Stop(bool invokeFinishCallback)
        {
            _cancelTokenSource.Cancel();
            
            // AnimationFinished Callback
            if (invokeFinishCallback) this.OnFinished?.Invoke();
            
            this.OnFinished = null;
        }

        public async Task Wait(bool changeCallback = false, Action finishCallback = null)
        {
            if (changeCallback) ChangeCallback(finishCallback);
            if (_animationRun != null)
                await _animationRun;
        }

        public void InvokeFinishCallback()
        {
            this.OnFinished?.Invoke();
            this.OnFinished = null;
        }

        public void ChangeCallback(Action finishCallback = null)
        {
            this.OnFinished = null;
            if (finishCallback != null)
                this.OnFinished = finishCallback.Invoke;
        }
    }
}
