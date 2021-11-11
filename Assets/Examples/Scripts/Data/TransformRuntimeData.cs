using UnityEngine;

namespace RuntimeNodeEditor
{
    public class TransformRuntimeData
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 scale;

        public TransformRuntimeData()
        {
            position = Vector3.zero;
            scale = Vector3.one;
            rotation = Vector3.zero;
//            _quaternion = Quaternion.Euler(Vector3.zero);
        }
    }

}

