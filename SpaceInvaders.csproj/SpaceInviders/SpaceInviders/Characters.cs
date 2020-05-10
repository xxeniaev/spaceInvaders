using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SpaceInviders
{
    #region Класс Monster
    public class Monster
    {
        public int X;
        public int Y;
        public int Cost;
        public int Height;
        public int Width;
        public Monster(int x, int y)
        {
            X = x;
            Y = y;
            Cost = 100;
            Height = Resource1.monster.Height;
            Width = Resource1.monster.Width + 1;
        }

        /// <summary>
        /// Функция, которая проверяет, стоит ли стрелять монстру. Если стоит, создает патрон.
        /// </summary>
        /// <param name="creatures">Общее состояние игры (Model) - в паттерне MVC</param>
        /// <param name="rnd">Генератор псевдослучайных чисел</param>
        public void MakeShot(Creatures creatures, Random rnd)
        {
            if (rnd.Next(1, 101) < 16 && PlayerOnRange(creatures.Player))
            {
                var catr = new Catridge(this);
                creatures.CatridgesMonsters.Add(catr);
            }
        }

        /// <summary>
        /// Функция, которая определяет, находится ли игрок в диапазоне обстрела этого монстра
        /// </summary>
        /// <param name="player">Игрок</param>
        /// <returns>Булево значение, находится ли игрок в диапазоне обстрела</returns>
        public bool PlayerOnRange(Player player)
        {
            return Math.Abs(player.X + player.Width / 2 - X) <= GameSetting.RangeOnFire;
        }
    }
    #endregion

    #region Класс Player
    public class Player
    {
        public int X;
        public int Y;
        public int Direction = 0;
        public int Score = 0;
        public int Height;
        public int Width;
        public Player(int x, int y)
        {
            X = x;
            Y = y;
            Height = Resource1.player.Height;
            Width = Resource1.player.Width;
        }

        /// <summary>
        /// Функция, которая изменяет координаты игрока (сдвигает его на определенные координаты)
        /// </summary>
        public void MovePlayer()
        {
            if ((Direction > 0 && X + 2 * Height < GameSetting.Width) ||
                (Direction < 0 && X - GameSetting.MovePlayer >= 0))
                X += Direction * GameSetting.MovePlayer;
        }
    }
    #endregion

    #region Класс Catridge
    public class Catridge
    {
        public int X;
        public int Y;
        public int Height;
        public int Width;
        public int Direction;
        public Catridge(Player player)
        {
            X = player.X + player.Width / 2;
            Y = player.Y;
            Direction = -1;
            Height = Resource1.patron.Height;
            Width = Resource1.patron.Width;
        }

        public Catridge(Monster monster)
        {
            X = monster.X + monster.Width / 2;
            Y = monster.Y;
            Direction = 1;
            Height = Resource1.patron.Height;
            Width = Resource1.patron.Width;
        }

        /// <summary>
        /// Функция, которая занимается сдвигом патрона, выпущенного Игроком, и уничтожением патрона и объекта, при их столкновении
        /// </summary>
        /// <param name="creatures">Общее состояние игры (Model) - в паттерне MVC</param>
        public void MovePlayerCatridge(Creatures creatures)
        {
            var isBreak = false;
            foreach (var b in creatures.Bunkers)
                if (b.X <= X && b.X + b.Width >= X + Width && b.Y + b.Height >= Y && b.Y <= Y)
                {
                    Y = -1000;
                    return;
                }
            foreach (var m in creatures.Monsters)
                if (m.X <= X && m.X + m.Width >= X + Width && m.Y + m.Height >= Y && m.Y <= Y) //потом сделать метод IsConflicted
                {
                    creatures.Player.Score += m.Cost;
                    creatures.Monsters.Remove(m); //может быть метод потом
                    Y = -1000;
                    isBreak = true;
                    break;
                }
            if (!isBreak)
                Y -= GameSetting.MoveCatridge;
        }

        /// <summary>
        /// Функция, которая занимается сдвигом патрона, выпущенного Монстра, и уничтожением патрона и объекта, при их столкновении
        /// </summary>
        /// <param name="creatures">Общее состояние игры (Model) - в паттерне MVC</param>
        public void MoveMonsterCatridge(Creatures creatures)
        {
            foreach (var b in creatures.Bunkers)
                if (((X + Width >= b.X && X + Width <= b.X + b.Width)
                || (X <= b.X + b.Width) && X >= b.X) && (Y + Height >= b.Y && Y <= b.Y + b.Height))
                {
                    Y = 1000;
                    return;
                }
            if (((X + Width >= creatures.Player.X && X + Width <= creatures.Player.X + creatures.Player.Width) 
                || (X <= creatures.Player.X + creatures.Player.Width) && X >= creatures.Player.X) && (Y + Height >= creatures.Player.Y && Y <=  creatures.Player.Y + creatures.Player.Height))
            {
                creatures.Player = null;
                creatures.GameOver = true;
                Y = 1000;
            }
            Y += GameSetting.SpeedMonsterCatridge;
        }
    }
    #endregion
    public class Creatures
    {
        public List<Monster> Monsters;
        public Player Player;
        public List<Catridge> CatridgesMonsters;
        public List<Bunker> Bunkers;
        public Catridge CatridgePlayer;
        public bool IsNeedRowDown = false;
        public bool GameOver = false;
        private int Direction = 1;
        public Creatures()
        {
            ResetMonsters();
            Player = new Player(0, GameSetting.Height - Resource1.player.Height);
            CatridgesMonsters = new List<Catridge>();
            var temp = new Bunker(0,0);
            Bunkers = temp.MakeBunkers();
        }

        /// <summary>
        /// Функция, которая двигает монстров (двигает либо по оси X, либо по Y)
        /// </summary>
        /// <param name="creatures">Общее состояние игры (Model) - в паттерне MVC</param>
        public void MoveMosters(Creatures creatures)
        {
            foreach (var m in Monsters)
            {
                if (m.Y + m.Height >= creatures.Bunkers[0].Y)
                {
                    GameOver = true;
                    return;
                }
                if ((Direction > 0 && m.X + GameSetting.MoveMonsterX >= GameSetting.Width) ||
                        (Direction < 0 && m.X - GameSetting.MoveMonsterX < 0))
                {
                    IsNeedRowDown = true;
                    break;
                }
            }
            if (IsNeedRowDown)
            {
                foreach (var m in Monsters)
                    m.Y += GameSetting.MoveMonsterY;
                IsNeedRowDown = false;
                Direction *= -1;
            }
            else
            {
                foreach (var m in Monsters)
                    m.X += GameSetting.MoveMonsterX * Direction;
            }
        }

        /// <summary>
        /// Функция, которая создает всех монстров на своим первоначальных местах
        /// </summary>
        public void ResetMonsters()
        {
            var temp = new List<Monster>();
            for (int j = 0; j < GameSetting.Width - GameSetting.CountTick * GameSetting.MoveMonsterX; j += GameSetting.MoveMonsterX)
                for (int i = 1; i < 5; i++)
                    temp.Add(new Monster(j, i * GameSetting.MoveMonsterY));
            Monsters = temp;
        }
    }

    public class Bunker
    {
        public int X;
        public int Y;
        public int Width;
        public int Height;
        public Bunker(int x, int y)
        {
            X = x;
            Y = y;
            Width = Resource1.bunker.Width;
            Height = Resource1.bunker.Height;
        }

        /// <summary>
        /// Функция, которая создает на игровом поле 2 бункера
        /// </summary>
        /// <returns>Лист, состоящий из двух бункеров</returns>
        public List<Bunker> MakeBunkers()
        {
            var temp = new List<Bunker>();
            var bunker1 = new Bunker(GameSetting.Width / 2 - 2 * Width,
                GameSetting.Height - Height - GameSetting.MoveMonsterY);
            var bunker2 = new Bunker(GameSetting.Width / 2 + Width,
                GameSetting.Height - Height - GameSetting.MoveMonsterY);
            temp.Add(bunker1);
            temp.Add(bunker2);
            return temp;
        }
    }
}
