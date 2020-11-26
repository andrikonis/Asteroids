using System.Collections.Generic;
using System.Reflection;
using Asteroids;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SFML.Graphics;
using SFML.System;

namespace Tests
{
    [TestClass]
    public class ProjectileTest
    {
        private readonly Vector2f position = new Vector2f(1, 1);
        private readonly Vector2f velocity = new Vector2f(3, 3);
        private const int Direction = 90;

        [TestMethod]
        public void Init_CorrectId()
        {
            InitializeProjectile(out var projectile);

            var id = (string) projectile.GetFieldOrProperty("Id");
            var count = typeof(Projectile).GetField("count", BindingFlags.NonPublic | BindingFlags.Static)
                ?.GetValue(null);

            Assert.AreEqual($"P{(long) count - 1}", id, "Unexpected Id");
        }

        [TestMethod]
        public void Init_CorrectShape()
        {
            InitializeProjectile(out var projectile);

            var radius = (float) typeof(Projectile).GetField("PROJECTILE_RADIUS", BindingFlags.NonPublic | BindingFlags.Static)
                ?.GetValue(null);
            var shape = projectile.GetFieldOrProperty("shape") as Shape;

            Assert.AreEqual(new Vector2f(radius, radius), shape?.Origin, "Unexpected origin");
            Assert.AreEqual(Color.Cyan, shape?.FillColor, "Unexpected color");
            Assert.AreEqual(position, shape?.Position, "Unexpected position");
        }

        private static IEnumerable<object[]> DirectionVelocityCombinations
        {
            get
            {
                return new[]
                {
                    new object[] {0, new Vector2f(0, -10)},
                    new object[] {90, new Vector2f(10, 0)},
                    new object[] {180, new Vector2f(0, 10)},
                    new object[] {270, new Vector2f(-10, 0)}
                };
            }
        }
        [TestMethod]
        [DynamicData(nameof(DirectionVelocityCombinations))]
        public void Init_CorrectVelocity(int direction, Vector2f expectedVelocity)
        {
            InitializeProjectile(out var projectile, dir: direction);

            var projectileVelocity = (Vector2f) projectile.GetFieldOrProperty("velocity");

            Assert.AreEqual(expectedVelocity.X, projectileVelocity.X, 0.1,  "Unexpected velocity");
            Assert.AreEqual(expectedVelocity.Y, projectileVelocity.Y, 0.1, "Unexpected velocity");
        }

        [TestMethod]
        public void GetPosition_ReturnsShapesPosition()
        {
            var projectile = InitializeProjectile(out var privateObject);

            var shape = privateObject.GetFieldOrProperty("shape") as Shape;

            Assert.AreEqual(shape?.Position, projectile.GetPostion(),  "Unexpected position");
        }

        [TestMethod]
        public void Update_ConsistentlyChangesPosition()
        {
            var projectile = InitializeProjectile(out _);
            var previousPosition = projectile.GetPostion();
            projectile.Update(5);
            var newPosition = projectile.GetPostion();
            var previousDiff = newPosition - previousPosition;

            for (var i = 0; i < 10; i++)
            {
                previousPosition = newPosition;
                projectile.Update(5);
                newPosition = projectile.GetPostion();
                var newDiff = newPosition - previousPosition;

                Assert.AreEqual(previousDiff.X, newDiff.X, 0.1, "Inconsistent position change on X axis after update");
                Assert.AreEqual(previousDiff.Y, newDiff.Y, 0.1, "Inconsistent position change on Y axis after update");
                previousDiff = newDiff;
            }
        }

        private Projectile InitializeProjectile(out PrivateObject privateObject, Vector2f? pos = null, Vector2f? vel = null, int? dir = null)
        {
            if (pos is null)
            {
                pos = position;
            }
            if (vel is null)
            {
                vel = velocity;
            }
            if (dir is null)
            {
                dir = Direction;
            }

            var projectile = new Projectile(pos.Value, vel.Value, dir.Value);

             privateObject = new PrivateObject(projectile);

            return projectile;
        }
    }
}