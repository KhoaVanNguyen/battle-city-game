using System;
using System.Drawing;
using System.Windows.Forms;

namespace BattleCity
{
    [Serializable]
    public class TankP2 : Tank
    {
        public TankP2()
        {
            this.deltaTime = 1.4f;
            this.life = 5;
            this.isShield = true;
            this.typeTank = 1;
            this.currentDir = Global.up;
            this.timeShotCricle = 40;
            this.x = 129 + 128;
            this.y = 385;
        }

        public void keyboardControl()
        {
            if (Keyboard.IsKeyDown(Keys.D))
            {
                isStop = false;
                move(Global.right);
            }
            else if (Keyboard.IsKeyDown(Keys.A))
            {
                isStop = false;
                move(Global.left);
            }
            else if (Keyboard.IsKeyDown(Keys.S))
            {
                isStop = false;
                move(Global.down);
            }
            else if (Keyboard.IsKeyDown(Keys.W))
            {
                isStop = false;
                move(Global.up);
            }
            else
            {
                isStop = true;
            }

            if (Keyboard.IsKeyDown(Keys.F))
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
