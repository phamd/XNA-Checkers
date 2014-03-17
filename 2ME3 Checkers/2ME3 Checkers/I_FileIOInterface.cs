using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2ME3_Checkers
{
    interface I_FileIOInterface
    {
        void save(Board board); //saves the current game to a file
        string load(Board board); //reads from the saved file
    }
}
