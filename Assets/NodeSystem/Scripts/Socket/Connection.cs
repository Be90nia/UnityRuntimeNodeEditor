namespace RuntimeNodeEditor
{
    public class Connection : IConnection
    {
        public string ConnectID;
        public SocketInput Input;
        public SocketOutput Output;
        string IConnection.ConnectID => ConnectID;
    }
}