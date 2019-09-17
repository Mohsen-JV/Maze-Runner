using System.Linq;
using System.Collections.Generic;
using System;
namespace MazeRuner
{
    public enum Direcion { Null, N, NE, E, SE, S, SW, W, NW }
    public class Block
    {
        Stack<Direcion> NDir;
        public bool IsWall = false;
        public bool Seen = false;
        public bool Checked = false;
        bool Is8Dir = false;
        Direcion _Dir = Direcion.Null;
        public Block() => IsWall = true;
        public Block(bool is8dir) => Is8Dir = is8dir;
        public void NextDir()
        {
            if (_Dir != Direcion.Null)
            {
                _Dir = NDir.Pop();
                if (_Dir == Direcion.Null) Checked = true;
            }
            else
            {
                Seen = true;
                NDir = new Stack<Direcion>();
                NDir.Push(Direcion.Null);
                var r = new Random();
                List<int> l;
                if(Is8Dir)
                    l=(new int[] { 1, 2, 3, 4, 5, 6, 7, 8 }).ToList();
                else l= (new int[] { 1, 3, 5, 7 }).ToList();
                var count = l.Count;
                for (int i = 0; i <count;i++)
                {
                    var temp = r.Next(l.Count);
                    NDir.Push((Direcion)l[temp]);
                    l.RemoveAt(temp);
                }
                _Dir = NDir.Pop();
            }
        }
        public int NextDir_w
        {
            get
            {
                if (_Dir == Direcion.N || _Dir == Direcion.S) return 0;
                if (_Dir == Direcion.E || _Dir == Direcion.NE || _Dir == Direcion.SE) return 1;
                return -1;
            }
        }
        public int NextDir_H
        {
            get
            {
                if (_Dir == Direcion.W || _Dir == Direcion.E) return 0;
                if (_Dir == Direcion.S || _Dir == Direcion.SW || _Dir == Direcion.SE) return 1;
                return -1;
            }
        }
    }
    public class Maze
    {
        Block[,] Mat;
        public bool Is8Dir;
        public Maze(int row, int column, bool is8dir = false)
        {
            Is8Dir = is8dir;
            Mat = new Block[row + 2, column + 2];
            for (int i = 0; i < Mat.GetLength(1); i++) Mat[0, i] = new Block();
            for (int i = 1; i < Mat.GetLength(0); i++) Mat[i, 0] = new Block();
            for (int i = 1; i < Mat.GetLength(0); i++) Mat[i, Mat.GetLength(1) - 1] = new Block();
            for (int i = 1; i < Mat.GetLength(1) - 1; i++) Mat[Mat.GetLength(0) - 1, i] = new Block();

            for (int i = 1; i <= row; i++)
                for (int ip = 1; ip <= column; ip++)
                    Mat[i, ip] = new Block(is8dir);
        }
        public Block this[int row, int column]
        {
            get => Mat[row + 1, column + 1];
            set => Mat[row + 1, column + 1] = value;
        }
        public int Row => Mat.GetLength(0) - 2;
        public int Column => Mat.GetLength(1) - 2;
    }
}
