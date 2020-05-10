using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace SpaceInviders.Tests
{
    [TestClass]
    public class SpaceInvadersTests
    {
        [TestMethod]
        public void Test_MakeShot()
        {
            return;
        }

        [TestMethod]
        public void Test_PlayerOnRange()
        {
            var monster1 = new Monster(0, 0);
            var monster2 = new Monster(200, 0);
            var player1 = new Player(0, 560);
            var player2 = new Player(200, 560);

            var res1 = monster1.PlayerOnRange(player1);
            var res2 = monster1.PlayerOnRange(player2);
            var res3 = monster2.PlayerOnRange(player1);
            var res4 = monster2.PlayerOnRange(player2);

            Assert.AreEqual(true, res1);
            Assert.AreEqual(false, res2);
            Assert.AreEqual(false, res3);
            Assert.AreEqual(true, res4);
        }

        [TestMethod]
        public void Test_MovePlayer()
        {
            var player1 = new Player(0, 560);
            var player2 = new Player(0, 560)
            {
                Direction = 100
            };
            var player3 = new Player(200, 560)
            {
                Direction = -100
            };

            player1.MovePlayer();
            player2.MovePlayer();
            player3.MovePlayer();

            Assert.AreEqual(0, player1.X);
            Assert.AreEqual(100, player2.X);
            Assert.AreEqual(100, player3.X);
        }

        [TestMethod]
        public void Test_MovePlayerCatridge()
        {
            return;
        }

        [TestMethod]
        public void Test_MoveMonsterCatridge()
        {
            return;
        }

        public void Test_MoveMosters()
        {
            return;
        }

        public void Test_MakeBunkers()
        {
            var bunker = new Bunker(0, 560);

            var t = bunker.MakeBunkers();

            Assert.AreEqual(2, t.Count);
        }
    }
}
