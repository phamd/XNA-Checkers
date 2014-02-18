using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace _2ME3_Checkers
{
    interface I_PieceInterface
    {
        //getters
        string getType(); //TODO: REPLACE string with enum
        int getLocationX();
        int getLocationY();
        bool getValidMovement(int column, int row); // input a location and check if it is valid to move there

        //setters
        void setType(string newType); // king piece vs normal piece
        void setLocation(int column, int row);
    }
}
