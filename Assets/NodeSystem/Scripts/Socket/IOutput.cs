using System;

namespace RuntimeNodeEditor
{
    public interface IOutput
    {
        NodeType GetOutpuType();
        NodeType GetNodeType();
        T GetValue<T>();
        event Action ValueUpdated;
    }
}