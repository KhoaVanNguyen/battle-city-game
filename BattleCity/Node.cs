using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace BattleCity
{
    public class Node : IComparable<Node>
    {
        public int x;
        public int y;
        public int f;
        public Node pre;

        public Node(int x, int y, int f, Node pre)
        {
            this.x = x;
            this.y = y;
            this.f = f;
            this.pre = pre;
        }

        public int CompareTo(Node other)
        {
            if (this.f < other.f) return -1;
            else if (this.f > other.f) return 1;
            else return 0;
        }
    }
}