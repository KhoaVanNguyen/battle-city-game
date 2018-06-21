using System;
using System.Drawing;
using System.Windows.Forms;

namespace BattleCity
{
    [Serializable]
    public class TankP1 : Tank
    {
        public TankP1()
        {
            this.deltaTime = 1.4f;
            this.life = 5;
            this.isShield = true;
            this.typeTank = 0;
            this.timeShotCricle = 40;
            this.currentDir = Global.up;
            this.x = 129;
            this.y = 385;
        }

        public void keyboardControl()
        {
            if (Keyboard.IsKeyDown(Keys.Right))
            {
                isStop = false;
                move(Global.right);
            }
            else if (Keyboard.IsKeyDown(Keys.Left))
            {
                isStop = false;
                move(Global.left);
            }
            else if (Keyboard.IsKeyDown(Keys.Down))
            {
                isStop = false;
                move(Global.down);
            }
            else if (Keyboard.IsKeyDown(Keys.Up))
            {
                isStop = false;
                move(Global.up);
            }
            else
            {
                isStop = true;
            }

            if (Keyboard.IsKeyDown(Keys.Space))
            {
                shot();
            }
        }

        public override void Update()
        {
            keyboardControl();
            base.Update();
        }
    }
}
