using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BattleCity
{
    [Serializable]
    public class Tank : Sprite
    {
        public float        deltaTime { get; set; }
        public int               life { get; set; }
        public int           typeTank { get; set; }
        public bool            isStop { get; set; }

        public int         currentDir { get; set; }
        public int        previousDir { get; set; }

        public int       currentFrame { get; set; }
        public int          timeFrame { get; set; }
        public int    timeFrameCricle { get; set; }

        public bool            isShot { get; set; }
        public int           timeShot { get; set; }
        public int     timeShotCricle { get; set; }

        public bool          isShield { get; set; }
        public int         timeShield { get; set; }
        public int currentShieldFrame { get; set; }
        public int    timeShieldFrame { get; set; }
        public int   timeShieldCricle { get; set; }
        public int     timeShieldSize { get; set; }

        public int             blockx { get; set; }
        public int             blocky { get; set; }

        public Tank()
        {
            this.width              = 32;
            this.isStop             = true;
            this.currentFrame       = 0;
            this.timeFrame          = 0;
            this.timeFrameCricle    = 3;

            this.timeShot           = 0;
            this.isShot             = false;

            this.timeShield         = 0;
            this.timeShieldFrame    = 0;
            this.currentShieldFrame = 0;
            this.timeShieldCricle   = 10;
            this.timeShieldSize     = 200;

            MainForm.tanks.Add(this);
        }

        public override void Update()
        {
            if (this.isShield)
            {
                timeShield++;
                if (timeShield > timeShieldSize)
                {
                    timeShield = 0;
                    timeShieldFrame = 0;
                    this.isShield = false;
                }
                else
                {
                    if (timeShieldFrame > timeShieldCricle)
                    {
                        if (currentShieldFrame == 0)
                        {
                            currentShieldFrame = 1;
                        }
                        else
                        {
                            currentShieldFrame = 0;
                        }
                        timeShieldFrame = 0;
                    }
                    timeShieldFrame++;
                }
            }

            if (timeFrame > timeFrameCricle)
            {
                if (currentFrame == 0)
                {
                    currentFrame = 1;
                }
                else
                {
                    currentFrame = 0;
                }
                timeFrame = 0;
            }
            if (!isStop)
            {
                timeFrame++;
            }
            if (isShot)
            {
                timeShot++;
                if (timeShot > timeShotCricle)
                {
                    timeShot = 0;
                    isShot = false;
                }
            }
        }

        public override void Draw(Graphics canvas)
        {
            canvas.ResetTransform();
            canvas.TranslateTransform(x, y);
            canvas.DrawImage(Global.sprites, 0, 0, new Rectangle(64 * typeTank + currentFrame * 32, currentDir * 32, 32, 32), GraphicsUnit.Pixel);
            if (this.isShield)
            {
                canvas.DrawImage(Global.sprites, 0, 0, new Rectangle(288 + currentShieldFrame * 32, 128, 32, 32), GraphicsUnit.Pixel);
            }
        }

        public void Remove()
        {
            MainForm.tanks.Remove(this);
        }

        public bool checkWalk(int x, int y)
        {
            if (y > 25)
            {
                y = 25;
            }
            else if (y < 0)
            {
                y = 0;
            }
            if (x < 0)
            {
                x = 0;
            }
            else if (x > 25)
            {
                x = 25;
            }
            if (MainForm.map[y, x] == Global.non || MainForm.map[y, x] == Global.tree)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool checkTank()
        {
            foreach (var tank in MainForm.tanks)
            {
                if (this != tank)
                {
                    var minx = this.x > tank.x ? this.x : tank.x;
                    var maxx = this.x + 28 < tank.x + 28 ? this.x + 28 : tank.x + 28;
                    var miny = this.y > tank.y ? this.y : tank.y;
                    var maxy = this.y + 28 < tank.y + 28 ? this.y + 28 : tank.y + 28;
                    if (minx <= maxx && miny <= maxy)
                    { 
                        ////
                        switch (this.currentDir)
                        {
                            case Global.up:
                                if (tank.y + 24 < this.y)
                                {
                                    return true;
                                }
                                break;
                            case Global.down:
                                if (tank.y > this.y + 24)
                                {
                                    return true;
                                }
                                break;
                            case Global.left:
                                if (tank.x + 24 < this.x)
                                {
                                    return true;
                                }
                                break;
                            case Global.right:
                                if (tank.x > this.x + 24)
                                {
                                    return true;
                                }
                                break;
                        }
                        ////
                    }
                }
            }
            return false;
        }

        public void setPosition()
        {
            if ((this.previousDir < 2 && this.currentDir > 1) || (this.previousDir > 1 && this.currentDir < 2))
            {
                if (this.currentDir == Global.up || this.currentDir == Global.down)
                {
                    this.x = (float)(((int)((this.x + 8) / 16)) * 16);
                }
                else
                {
                    this.y = (float)(((int)((this.y + 8) / 16)) * 16);
                }
            }
            this.previousDir = this.currentDir;
        }

        public bool moveUp()
        {
            this.currentDir = Global.up;
            this.setPosition();
            int blockx = (int)((this.x) / 16);
            int blocky = (int)((this.y) / 16);
            if (this.y > -1 && checkWalk(blockx, blocky) && checkWalk(blockx + 1, blocky))
            {
                return !checkTank();
            }
            else
            {
                return false;
            }
        }

        public bool moveDown()
        {
            this.currentDir = Global.down;
            this.setPosition();
            int blockx = (int)((this.x) / 16);
            int blocky = (int)((this.y + width) / 16);
            if (this.y + this.width < 416 && checkWalk(blockx, blocky) && checkWalk(blockx + 1, blocky))
            {
                return !checkTank();
            }
            else
            {
                return false;
            }
        }

        public bool moveLeft()
        {
            this.currentDir = Global.left;
            this.setPosition();
            int blockx = (int)((this.x) / 16);
            int blocky = (int)((this.y) / 16);
            if (this.x > -1 && checkWalk(blockx, blocky) && checkWalk(blockx, blocky + 1))
            {
                return !checkTank();
            }
            else
            {
                return false;
            }
        }

        public bool moveRight()
        {
            this.currentDir = Global.right;
            this.setPosition();
            int blockx = (int)((this.x + width) / 16);
            int blocky = (int)((this.y) / 16);
            if (this.x + this.width < 416 && checkWalk(blockx, blocky) && checkWalk(blockx, blocky + 1))
            {
                return !checkTank();
            }
            else
            {
                return false;
            }
        }

        public bool move(int dir)
        {
            switch (dir)
            {
                case Global.up:
                    {
                        if (this.moveUp())
                        {
                            this.y -= deltaTime;
                            return true;
                        }
                        break;
                    }
                case Global.down:
                    {
                        if (this.moveDown())
                        {
                            this.y += deltaTime;
                            return true;
                        }
                        break;
                    }
                case Global.left:
                    {
                        if (this.moveLeft())
                        {
                            this.x -= deltaTime;
                            return true;
                        }
                        break;
                    }
                case Global.right:
                    {
                        if (this.moveRight())
                        {
                            this.x += deltaTime;
                            return true;
                        }
                        break;
                    }
                default: break;
            }
            return false;
        }

        public void shot()
        {
            if (!this.isShot)
            {
                this.isShot = true;
                var bullet = new Bullet(this.x, this.y, this.currentDir, this.typeTank);
            }
        }

        public int roundBlock(float coor)
        {
            return (int)Math.Round(coor / 16);
        }
    }
}
