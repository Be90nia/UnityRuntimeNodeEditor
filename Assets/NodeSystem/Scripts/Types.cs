namespace RuntimeNodeEditor
{
    public enum NodeType
    {
        Float,
        Int,
        Vector3,
        Object,
        Class,
        Transform,
        GameObject,
        ITimer,
        Group
    }

    public enum MathOperations
    {
        Multiply,
        Divide,
        Add,
        Substract
    }

    public enum SocketType
    {
        Input,
        Output
    }

    public enum ConnectionType
    {
        Single,
        Multiple
    }
}