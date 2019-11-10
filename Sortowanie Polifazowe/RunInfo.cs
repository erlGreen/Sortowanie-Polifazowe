using System;
using System.Collections.Generic;
using System.Text;

namespace Sortowanie_Polifazowe
{
    class RunInfo
    {
        int tape1Runs;
        int tape2Runs;
        int tape3Runs;
        public RunInfo(int tape1, int tape2)
        {
            tape1Runs = tape1;
            tape2Runs = tape2;
            tape3Runs = 0;
        }

        public int GetRuns(string tape)
        {
            if (tape == "tape1.txt")
                return tape1Runs;
            else if (tape == "tape2.txt")
                return tape2Runs;
            else return tape3Runs;
        }

        public void SetRuns(string tape, int value)
        {
            if (tape == "tape1.txt")
                tape1Runs = value;
            else if (tape == "tape2.txt")
                tape2Runs = value;
            else tape3Runs = value;
        }
    }
}
