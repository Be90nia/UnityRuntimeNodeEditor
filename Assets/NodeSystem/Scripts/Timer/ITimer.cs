

namespace RuntimeNodeEditor
{
    public interface ITimer
    {
        void OnUpdate(float time);
        void OnComplete();
    }
}
