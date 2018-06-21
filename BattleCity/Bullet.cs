using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;

namespace BattleCity
{
    [Serializable]
    public class Bullet : Sprite
    {
        public int typeBullet { get; set; }
        public float deltaTime { get; set; }
        public int dir { get; set; }

        public Bullet(float x, float y, int dir, int type)
        {
            width = 8;
            deltaTime = 4.2f;
            this.typeBullet = type;
            this.x = x;
            this.y = y;
            this.dir = dir;
            switch (this.dir)
            {
                case Global.up: this.x += 12; break;
                case Global.down: this.x += 12; this.y += 24; break;
                case Global.right: this.y += 12; this.x += 24; break;
                case Global.left: this.y += 12; break;
            }
            MainForm.bullets.Add(this);
        }

        public void Remove()
        {
            MainForm.bullets.Remove(this);
        }

        public void move()
        {
            switch (this.dir)
            {
                case Global.up: this.y -= deltaTime; break;
                case Global.down: this.y += deltaTime; break;
                case Global.left: this.x -= deltaTime; break;
                case Global.right: this.x += deltaTime; break;
            }
        }

        public bool checkHit()
        {
            foreach (var item in MainForm.tanks)
            {
                if (this.typeBullet == 0 && item.typeTank == 1 || this.typeBullet == 1 && item.typeTank == 0)
                {
                    continue;
                }
                if (this.typeBullet > 1 && item.typeTank > 1)
                {
                    continue;
                }
                if (this.x + 8 >= item.x && item.x + 24 >= this.x && this.y + 8 >= item.y && item.y + 24 >= this.y && this.typeBullet != item.typeTank)
                {
                    var explsion = new Explosion(1, this.x, this.y);
                    if (item.isShield)
                    {
                        return true;
                    }
                    if (item.typeTank == 6)
                    {
                        item.life--;
                        item.typeTank = 5;
                        return true;
                    }
                    if (item.typeTank == 5)
                    {
                        item.life--;
                        item.typeTank = 4;
                        return true;
                    }
                    if (item.typeTank == 4)
                    {
                        item.life--;
                        item.Remove();
                        return true;
                    }
                    if (item.typeTank == 0 || item.typeTank == 1)
                    {
                        item.life--;
                        if (item.life == 0)
                        {
                            if (item.typeTank == 0)
                            {
                                MainForm.P1Live = false;
                            }
                            else if (item.typeTank == 1)
                            {
                                MainForm.P2Live = false;
                            }
                            item.Remove();
                        }
                        else
                        {
                            if (item.typeTank == 0)
                            {
                                item.currentDir = Global.up;
                                item.x = 129;
                                item.y = 385;
                                item.isShield = true;
                            }
                            else if (item.typeTank == 1)
                            {
                                item.currentDir = Global.up;
                                item.x = 129 + 128;
                                item.y = 385;
                                item.isShield = true;
                            }
                        }
                        return true;
                    }
                    if (MainForm.awayTank.Count > 0)
                    {
                        MainForm.AITank = new TankAI(MainForm.awayTank[0]);
                        MainForm.awayTank.RemoveAt(0);
                        MainForm.timeInitTank = 0;
                    }
                    item.Remove();
                    return true;
                }
            }
            bool isHit = false;
            float tx, ty;
            Point[] pnts = new Point[2];
            switch (this.dir)
            {
                case Global.up:
                    tx = this.x;
                    ty = this.y + 8;
                    pnts[0] = new Point(roundBlock(tx - 12), roundBlock(ty - 16));
                    pnts[1] = new Point(roundBlock(tx + 4), roundBlock(ty - 16));
                    break;
                case Global.down:
                    tx = this.x;
                    ty = this.y - 8;
                    pnts[0] = new Point(roundBlock(tx - 12), roundBlock(ty + 8));
                    pnts[1] = new Point(roundBlock(tx + 4), roundBlock(ty + 8));
                    break;
                case Global.left:
                    tx = this.x + 8;
                    ty = this.y;
                    pnts[0] = new Point(roundBlock(tx - 16), roundBlock(ty - 12));
                    pnts[1] = new Point(roundBlock(tx - 16), roundBlock(ty + 4));
                    break;
                case Global.right:
                    tx = this.x - 8;
                    ty = this.y;
                    pnts[0] = new Point(roundBlock(tx + 8), roundBlock(ty - 12));
                    pnts[1] = new Point(roundBlock(tx + 8), roundBlock(ty + 4));
                    break;
            }
            for (int i = 0; i < 2; i++)
            {
                if (pnts[i].X > 25) { pnts[i].X = 25; }
                if (pnts[i].X < 0) { pnts[i].X = 0; }
                if (pnts[i].Y > 25) { pnts[i].Y = 25; }
                if (pnts[i].Y < 0) { pnts[i].Y = 0; }
            }
            if (this.x < 0 || this.y < 0 || this.x > 416 || this.y > 416)
            {
                var explsion = new Explosion(0, this.x, this.y);
                return true;
            }
            for (int i = 0; i < 2; i++)
            {
                int px = pnts[i].X;
                int py = pnts[i].Y;
                int wallType = MainForm.map[py, px];
                if (wallType == 9)
                {
                    continue;
                }
                if (wallType == 7 || wallType == 8)
                {
                    MainForm.HomeLive = false;
                    MainForm.map[24, 12] = 10;
                    var explsion = new Explosion(1, this.x, this.y);
                }
                if (wallType == Global.wall || wallType == Global.steel || wallType > 2)
                {
                    var explsion = new Explosion(0, this.x, this.y);
                    isHit = true;
                    if (wallType == Global.wall)
                    {
                        switch (dir)
                        {
                            case Global.up: MainForm.map[py, px] = Global.wall1;
                                break;
                            case Global.down: MainForm.map[py, px] = Global.wall2;
                                break;
                            case Global.left: MainForm.map[py, px] = Global.wall3;
                                break;
                            case Global.right: MainForm.map[py, px] = Global.wall4;
                                break;
                        }
                    }
                    if (wallType > 2)
                    {
                        MainForm.map[py, px] = Global.non;
                    }
                }
            }
            return isHit;
        }

        public override void Draw(Graphics gr)
        {
            gr.ResetTransform();
            gr.TranslateTransform(x, y);
            gr.DrawImage(Global.sprites, 0, 0, new Rectangle(8 * dir + 256, 144, 8, 8), GraphicsUnit.Pixel);
        }

        public override void Update()
        {
            this.move();
            bool isHit = false;
            if (this.checkHit())
            {
                isHit = true;
            }
            if (isHit)
            {
                this.Remove();
            }
        }

        public int roundBlock(float temp)
        {
            return (int)Math.Round(temp / 16);
        }
    }
}
