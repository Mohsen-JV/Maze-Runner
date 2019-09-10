using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;
using System.Threading.Tasks;

namespace MazeRuner
{
    public enum Direcion { Null, N, NE, E, SE, S, SW, W, NW }
    public class Block
    {
        public bool IsWall = false;
        public bool Seen = false;
        public bool Checked = false;
        bool Is8Dir = false;
        Direcion _Dir=Direcion.Null;
        Direcion NextDirection(Direcion dir)
        {
            if (Is8Dir)
                return dir == Direcion.NW ? Direcion.N : dir + 1;
            else return dir == Direcion.W ? Direcion.N : dir + 2;
        }
        public Block() => IsWall = true;
        public Block(bool is8dir) => Is8Dir = is8dir;
        public void NextDir()
        {
            if(_Dir!=Direcion.Null)
            {
                _Dir = NextDirection(_Dir);
                if(_Dir==Direcion.E)
                {
                    Checked= true;
                    _Dir = Direcion.Null;
                }
            }
            else
            {
                Seen = true;
                _Dir = Direcion.E;
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
        public Maze(int row,int column,bool is8dir=false)
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
        public Block this[int row,int column]
        {
            get => Mat[row + 1, column + 1];
        }
    }
}
