using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpaceInviders
{
    public partial class Form1 : Form
    {
        public Bitmap MonsterTexture = Resource1.monster,
                      PlayerTexture = Resource1.player,
                      CatridgeTexture = Resource1.patron,
                      BunkerTexture = Resource1.bunker;
        public Creatures Creatures;


        public Form1(Creatures creatures)
        {
            InitializeComponent();

            SetStyle(ControlStyles.OptimizedDoubleBuffer |
                ControlStyles.AllPaintingInWmPaint |
                ControlStyles.UserPaint, true);

            UpdateStyles();
            Creatures = creatures;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            
        }

        /// <summary>
        /// Функция, отвечающая за тик таймера, связанного с управлением монстров, а также за их перерисовку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tick1_Tick(object sender, EventArgs e)
        {
            if (!Creatures.GameOver)
            {
                var rnd = new Random();
                foreach (var m in Creatures.Monsters)
                    m.MakeShot(Creatures, rnd);
                if (Creatures.Monsters.Count == 0)
                    Creatures.ResetMonsters();
                Creatures.MoveMosters(Creatures);
                Refresh();
            }
        }

        /// <summary>
        /// Функция, отвечающая за создание действий игрока при нажатии клавиши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyDown(object sender, KeyEventArgs e)
        {
            if (!Creatures.GameOver)
            {
                if (e.KeyValue == (char)Keys.A || e.KeyValue == (char)Keys.Left)
                    Creatures.Player.Direction = -1;
                else if (e.KeyValue == (char)Keys.Right || e.KeyValue == (char)Keys.D)
                    Creatures.Player.Direction = 1;
                else if (e.KeyValue == (char)Keys.Space)
                    if (Creatures.CatridgePlayer == null || Creatures.CatridgePlayer.Y < 0)
                        Creatures.CatridgePlayer = new Catridge(Creatures.Player);
                    else
                        Creatures.Player.Direction = 0;
            }
            else if (Creatures.GameOver && e.KeyValue == (char)Keys.Space)
                NewGame();
            else if (Creatures.GameOver && e.KeyValue == (char)Keys.Escape)
                Application.Exit();
        }

        /// <summary>
        /// Функция, отвечающая за тик таймера, связанного с управлением игрока и патронов, а также за их перерисовку
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timer2_Tick(object sender, EventArgs e)
        {
            if (!Creatures.GameOver)
            {
                Creatures.Player.MovePlayer();
                if (Creatures.CatridgesMonsters != null)
                    foreach (var m in Creatures.CatridgesMonsters)
                    {
                        m.MoveMonsterCatridge(Creatures);
                        if (Creatures.GameOver)
                        {
                            Thread.Sleep(800);
                            break;
                        }
                    }
                var temp = new List<Catridge>();
                foreach (var m in Creatures.CatridgesMonsters)
                    if (m.Y > GameSetting.Height)
                        temp.Add(m);
                foreach (var m in temp)
                    Creatures.CatridgesMonsters.Remove(m);
                if (Creatures.CatridgePlayer != null)
                    Creatures.CatridgePlayer.MovePlayerCatridge(Creatures);
                UpdateScore();
                Refresh();
            }
            else
                GameOver();
        }

        /// <summary>
        /// Функция, которая обновляет счетчик, отвечающего за показ очков
        /// </summary>
        private void UpdateScore()
        {
            if (!Creatures.GameOver)
            {
                var score = Creatures.Player.Score;
                var lastScore = int.Parse(label4.Text);
                if (score != lastScore)
                    label4.Text = score.ToString();
            }
        }

        /// <summary>
        /// Функция, которая показывает сообщение о проигрыше
        /// </summary>
        private void GameOver()
        {
            label1.Show();
            label2.Show();
        }

        /// <summary>
        /// Функция, которая обновляет состояние игрового поля и экрана
        /// </summary>
        private void NewGame()
        {
            Creatures = new Creatures();
            label1.Hide();
            label2.Hide();
        }

        /// <summary>
        /// Функция, отвечающая за прекращение действий игрока при отжатии клавиши
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_KeyUp(object sender, KeyEventArgs e)
        {
            if ((e.KeyValue == (char)Keys.A || e.KeyValue == (char)Keys.Left
                || e.KeyValue == (char)Keys.Right || e.KeyValue == (char)Keys.D) && !Creatures.GameOver)
                Creatures.Player.Direction = 0;
        }

        /// <summary>
        /// Функция, отвечающая за отрисовку игрового поля
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            if (!Creatures.GameOver)
            {
                var g = e.Graphics;
                foreach (var b in Creatures.Bunkers)
                    g.DrawImage(BunkerTexture, new Rectangle(b.X, b.Y, BunkerTexture.Width, BunkerTexture.Height));
                foreach (var m in Creatures.Monsters)
                    g.DrawImage(MonsterTexture, new Rectangle(m.X, m.Y, MonsterTexture.Width, MonsterTexture.Height));

                g.DrawImage(PlayerTexture, new Rectangle(Creatures.Player.X, Creatures.Player.Y,
                                                PlayerTexture.Width, PlayerTexture.Height));

                if (Creatures.CatridgePlayer != null)
                    g.DrawImage(CatridgeTexture, new Rectangle(Creatures.CatridgePlayer.X, Creatures.CatridgePlayer.Y, 
                        CatridgeTexture.Width, CatridgeTexture.Height));
                if (Creatures.CatridgesMonsters != null)
                    foreach (var c in Creatures.CatridgesMonsters)
                        g.DrawImage(CatridgeTexture, new Rectangle(c.X, c.Y, CatridgeTexture.Width, CatridgeTexture.Height));
            }

        }
    }
}
