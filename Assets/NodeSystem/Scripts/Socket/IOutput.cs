using System;

namespace RuntimeNodeEditor
{
    public interface IOutput
    {
        NodeType GetNodeType();
        T GetValue<T>();
        event Action ValueUpdated;
    }
}