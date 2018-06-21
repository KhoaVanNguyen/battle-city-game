using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BattleCity
{
    [Serializable]
    public abstract class Sprite
    {
        public float x { get; set; }
        public float y { get; set; }
        public int width { get; set; }

        public virtual void Update() { ; }

        public virtual void Draw(Graphics canvas) { ; }

        public bool hitTest(Sprite obj)
        {
            if (this.x + this.width >= obj.x && obj.x + obj.width >= this.x && this.y + this.width >= obj.y && obj.y + obj.width >= this.y)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
