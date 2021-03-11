using System;

namespace data
{
    public class DataEmulator
    {
        public DataEmulator(int size = 4) {
            useBios = tested = new bool[size];

            pathExe = pathBios = pathCue = new string[size];
        }
        public bool[] useBios,tested;
        public string[] pathExe,pathBios,pathCue;


        public bool skipWindow;
        public int selectedID;
    }
}