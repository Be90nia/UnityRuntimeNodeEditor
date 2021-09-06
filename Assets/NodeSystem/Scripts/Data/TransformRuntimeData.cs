using UnityEngine;

namespace RuntimeNodeEditor
{
    public class TransformRuntimeData
    {
        public Vector3 position;
        private Quaternion _quaternion;
        public Quaternion quaternion
        {
            get => _quaternion;
            set => _quaternion = value;
        }

        public Vector3 rotation
        {
            get => _quaternion.eulerAngles;

            set
            {
                _quaternion = Quaternion.Euler(rotation);
            }
        }

        public Vector3 scale;

        public TransformRuntimeData()
        {
            position = Vector3.zero;
            scale = Vector3.one;
            _quaternion = Quaternion.Euler(Vector3.zero);
        }
    }

}

