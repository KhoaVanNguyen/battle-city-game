using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace BattleCity
{
    public class Item : Sprite
    {
        public int typeItem     { get; set; }
        public int timeLive     { get; set; }
        public int timeLiveSize { get; set; }

        public Item(int typeItem, int qx, int qy)
        {
            this.width        = 32;
            this.typeItem     = typeItem;
            this.x            = qx * 16;
            this.y            = qy * 16;
            this.timeLive     = 0;
            this.timeLiveSize = 1000;
            MainForm.items.Add(this);
        }

        public override void Update()
        {
            if (timeLive > timeLiveSize)
            {
                MainForm.items.Remove(this);
            }
            timeLive++;
            for (int i = 0; i < MainForm.numberPeopleLive; i++)
            {
                if (hitTest(MainForm.tanks[i]))
                {
                        
                    switch (typeItem)
                    {
                        case 0:
                            MainForm.homeIsSteel = true;
                            MainForm.items.Remove(this);
                            break;
                        case 1:
                            MainForm.tanks[i].life += 1;
                            MainForm.items.Remove(this);
                            break;
                        case 2:
                            MainForm.tanks[i].isShield = true;
                            MainForm.items.Remove(this);
                            break;
                    }
                }
            }
        }

        public override void Draw(Graphics canvas)
        {
            canvas.ResetTransform();
            canvas.TranslateTransform(x, y);
            canvas.DrawImage(Global.sprites, 0, 0, new Rectangle(typeItem * 32 + 160, 128, 32, 32), GraphicsUnit.Pixel);
        }
    }
}
