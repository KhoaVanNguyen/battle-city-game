using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace BattleCity
{
    public class Explosion : Sprite
    {
        public int timeFrame     { get; set; }
        public int timeFrameSize { get; set; }
        public int currentFrame  { get; set; }
        public int frameSize     { get; set; }
        public int xSprite       { get; set; }
        public int ySprite       { get; set; }
        public int [] currentX   { get; set; }

        public Explosion(int TypeEx, float x, float y)
        {
            this.x = x;
            this.y = y;
            currentFrame = 0;
            timeFrame = 0;
            timeFrameSize = 2;
            if (TypeEx == 0)
            {
                width = 32;
                xSprite = 64;
                ySprite = 128;
                frameSize = 3;
                currentX = new int[5] { 0, 1, 2, 1, 0 };
            }
            else
            {
                width = 64;
                xSprite = 0;
                ySprite = 162;
                frameSize = 5;
                currentX = new int[9] { 0, 1, 2, 3, 4, 3, 2, 1, 0 };
            }
            MainForm.explosions.Add(this);
        }

        public override void Update()
        {
            timeFrame += 1;
            if (currentFrame > (frameSize - 1) * 2 - 1)
            {
                MainForm.explosions.Remove(this);
            }
            if (timeFrame > timeFrameSize)
            {
                currentFrame++;
                timeFrame = 0;
            }
        }

        public override void Draw(Graphics canvas)
        {
            canvas.ResetTransform();
            canvas.TranslateTransform(this.x - width / 2, this.y - width / 2);
            canvas.DrawImage(Global.sprites, 0, 0, new Rectangle(xSprite + (currentX[currentFrame]) * width - 1, ySprite - 1, width, width), GraphicsUnit.Pixel);
        }
    }
}
