using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BattleCity
{
    [Serializable]
    public class Tank : Sprite
    {
        public Tank()
        {
            MainForm.tanks.Add(this);
        }
    }
}
