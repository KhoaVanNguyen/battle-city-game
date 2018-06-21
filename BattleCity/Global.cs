﻿using System;
using System.Drawing;

namespace BattleCity
{
    public static class Global
    {
        public static Bitmap sprites = BattleCity.Properties.Resources.sprites;
        public const int up    = 0;
        public const int down  = 1;
        public const int left  = 2;
        public const int right = 3;

        public const int non   = 0;
        public const int wall  = 1;
        public const int steel = 2;
        public const int wall1 = 3;
        public const int wall2 = 4;
        public const int wall3 = 5;
        public const int wall4 = 6;
        public const int home  = 7;
        public const int tree  = 9;
        public const int homeDestroy = 10;

        public static int[,] map1 = new int [,]
        {
		    {0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,1,1,2,2,1,1,0,0,1,1,0,0,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,1,1,2,2,1,1,0,0,1,1,0,0,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,0,0,1,1,0,0,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,0,0,1,1,0,0,1,1,0,0},
			{0,0,0,0,0,0,0,0,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,0,0},
			{1,1,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,1,1},
			{2,2,0,0,1,1,1,1,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,2,2},
			{0,0,0,0,0,0,0,0,0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0},
			{0,0,1,1,0,0,1,1,0,0,1,1,1,1,1,1,0,0,1,1,0,0,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,0,0,1,1,0,0,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,0,0,0,0,0,0,0,0,1,1,0,0,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,0,1,1,1,1,0,0,0,1,1,0,0,1,1,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,1,7,8,1,0,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,1,8,8,1,0,0,0,0,0,0,0,0,0,0,0},
        };

        public static int[,] map2 = new int[, ]
        {
		    {0,0,0,0,0,0,2,2,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,2,2,0,0,0,0,0,0,2,2,0,0,0,0,0,0,0,0,0,0},
			{0,0,1,1,0,0,2,2,0,0,0,0,0,0,1,1,0,0,1,1,0,0,1,1,0,0},
			{0,0,1,1,0,0,2,2,0,0,0,0,0,0,1,1,0,0,1,1,0,0,1,1,0,0},
			{0,0,1,1,0,0,0,0,0,0,0,0,1,1,1,1,0,0,1,1,2,2,1,1,0,0},
			{0,0,1,1,0,0,0,0,0,0,0,0,1,1,1,1,0,0,1,1,2,2,1,1,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,2,2,0,0,0,0,0,0},
			{9,9,0,0,0,0,0,0,0,0,0,0,2,2,0,0,0,0,1,1,9,9,1,1,2,2},
			{9,9,0,0,0,0,0,0,0,0,0,0,2,2,0,0,0,0,1,1,9,9,1,1,2,2},
			{9,9,9,9,0,0,0,0,0,0,1,1,0,0,0,0,2,2,0,0,9,9,0,0,0,0},
			{9,9,9,9,0,0,0,0,0,0,1,1,0,0,0,0,2,2,0,0,9,9,0,0,0,0},
			{0,0,1,1,1,1,1,1,9,9,9,9,9,9,2,2,0,0,0,0,9,9,1,1,0,0},
			{0,0,1,1,1,1,1,1,9,9,9,9,9,9,2,2,0,0,0,0,9,9,1,1,0,0},
			{0,0,0,0,0,0,2,2,9,9,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0},
			{0,0,0,0,0,0,2,2,9,9,1,1,0,0,1,1,0,0,1,1,0,0,1,1,0,0},
			{2,2,1,1,0,0,2,2,0,0,1,1,0,0,1,1,0,0,0,0,0,0,1,1,0,0},
			{2,2,1,1,0,0,2,2,0,0,1,1,0,0,1,1,0,0,0,0,0,0,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,1,1,1,1,1,1,0,0,1,1,2,2,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,1,1,1,1,1,1,0,0,1,1,2,2,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0},
			{0,0,1,1,0,0,1,1,0,0,1,1,1,1,1,1,0,0,0,0,0,0,0,0,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,1,1,0,0,1,1,0,0},
			{0,0,0,0,0,0,0,0,0,0,0,1,1,1,1,0,0,0,1,1,0,0,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,0,1,7,8,1,0,0,0,1,1,1,1,1,1,0,0},
			{0,0,1,1,0,0,1,1,0,0,0,1,8,8,1,0,0,0,1,1,1,1,1,1,0,0},
        };
    }
}