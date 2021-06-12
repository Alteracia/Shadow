using UnityEngine;

namespace Alteracia.Animation
{
    [CreateAssetMenu(fileName = "RotationAnimation", menuName = "AltAnimations/Rotation", order = 2)]
    [System.Serializable]
    public class Rotation : AltAnimation
    {
        private Transform[] _transforms;
        [SerializeField] 
        private bool local;
        [SerializeField]
        private Vector3 start;
        [SerializeField] 
        private Vector3 finish;
        
        private Quaternion _start;
        private Quaternion _finish;
        
        public override void ChangeFinish(object finish)
        {
            Quaternion rot = (Quaternion) finish;
            this.finish = rot.eulerAngles;
        }

        protected override bool PrepareTargets()
        {
            if (_transforms == null || _transforms.Length == 0)
                _transforms = System.Array.ConvertAll(components, item => (Transform)item);
            if (_transforms == null || _transforms.Length == 0) return false;
            
            _start = local ? _transforms[0].localRotation : _transforms[0].rotation;

            return true;
        }

        protected override void UpdateCurrentProgressFromStart()
        {
            progress = Quaternion.Angle(Quaternion.Euler(start), _start) / Quaternion.Angle(Quaternion.Euler(start), Quaternion.Euler(finish));
        }

        protected override void SetConstantStart()
        {
            _start = Quaternion.Euler(start);
        }

        protected override void OverwriteTarget()
        {
            _finish = Quaternion.Euler(finish);
        }

        protected override void AddTarget()
        {
            Quaternion rot = local ? _transforms[0].localRotation : _transforms[0].rotation;
            Vector3 euler = rot.eulerAngles + finish;
            _finish = Quaternion.Euler(euler);
        }

        protected override void MultiplyTarget()
        {
            Quaternion rot = local ? _transforms[0].localRotation : _transforms[0].rotation;
            _finish = rot * Quaternion.Euler(finish);
        }

        protected override void Interpolate()
        {
            base.Interpolate();
            
            if (_transforms == null || _transforms.Length == 0) return;
            foreach (var trans in _transforms)
            {
                if (local)
                    trans.localRotation = Quaternion.Lerp(_start, _finish, progress);
                else
                    trans.rotation = Quaternion.Lerp(_start, _finish, progress);
            }
        }
        
        public override System.Type GetComponentType()
        {
            return typeof(Transform);
        }
    }
}
