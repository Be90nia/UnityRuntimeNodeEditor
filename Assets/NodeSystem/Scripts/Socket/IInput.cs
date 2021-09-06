

namespace RuntimeNodeEditor
{
    public interface IInput
    {
        NodeType GetInputType();
        NodeType GetNodeType();
        void SetNodeType(NodeType nodeType);
    }
}
