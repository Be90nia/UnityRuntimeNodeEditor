using System;
using System.Collections.Generic;

namespace RuntimeNodeEditor
{
    public class SocketRegisterer
    {
        public List<SocketOutput> OutputList;
        public List<SocketInput> InputList;

        public SocketRegisterer()
        {
            OutputList = new List<SocketOutput>();
            InputList = new List<SocketInput>();
        }

        public void Register(SocketOutput output)
        {
            OutputList.Add(output);
        }

        public void Register(SocketInput input)
        {
            InputList.Add(input);
        }
    }
}