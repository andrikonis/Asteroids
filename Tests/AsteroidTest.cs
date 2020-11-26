using System.Collections.Generic;
using System.Reflection;
using Asteroids;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SFML.Graphics;
using SFML.System;

namespace Tests
{
    /// <summary>
    /// Summary description for Asteroid
    /// </summary>
    [TestClass]
    public class AsteroidTest
    {
        private readonly Vector2f position = new Vector2f(1, 1);
        private readonly Vector2f velocity = new Vector2f(3, 3);
        private const int Radius = 20;

        private readonly Vector2f positionShip = new Vector2f(0, 0);
        private const float SideLenght = 10;

        private readonly Vector2f positionProj = new Vector2f(1, 1);
        private readonly Vector2f velocityProj = new Vector2f(3, 3);
        private const int Direction = 90;

        [TestMethod]
        public void Init_CorrectId()
        {
            InitializeAsteroid(out var asteroid);

            var id = (string)asteroid.GetFieldOrProperty("Id");
            var count = typeof(Asteroid).GetField("count", BindingFlags.NonPublic | BindingFlags.Static)
                ?.GetValue(null);

            Assert.AreEqual($"A{(long)count - 1}", id, "Unexpected Id");
        }

        [TestMethod]
        public void Init_VelocityIsLessWithBiggerRadius()
        {
            InitializeAsteroid(out var asteroidTemp, rad: 1);
            var asteroidVelocityBefore = (Vector2f)asteroidTemp.GetFieldOrProperty("velocity");
            for (int i = 2; i < Radius * 3; i++)
            {
                InitializeAsteroid(out var asteroid, rad: i);
                var asteroidVelocity = (Vector2f)asteroid.GetFieldOrProperty("velocity");
                Assert.IsTrue(asteroidVelocityBefore.X > asteroidVelocity.X, "Unexpected velocity");
                Assert.IsTrue(asteroidVelocityBefore.Y > asteroidVelocity.Y, "Unexpected velocity");
                asteroidVelocityBefore = asteroidVelocity;
            }
        }

        [TestMethod]
        public void Init_CorrectShape()
        {
            InitializeAsteroid(out var asteroid);

            var shape = asteroid.GetFieldOrProperty("shape") as Shape;

            Assert.AreEqual(new Vector2f(Radius, Radius), shape?.Origin, "Unexpected origin");
            Assert.AreEqual(Color.Black, shape?.FillColor, "Unexpected fill color");
            Assert.AreEqual(Color.Yellow, shape?.OutlineColor, "Unexpected outline color");
            Assert.AreEqual(-2, shape?.OutlineThickness, "Unexpected outline thickness");
            Assert.AreEqual(position, shape?.Position, "Unexpected position");
        }

        [TestMethod]
        public void GetPosition_ReturnsShapesPosition()
        {
            var asteroid = InitializeAsteroid(out var privateObject);

            var shape = privateObject.GetFieldOrProperty("shape") as Shape;

            Assert.AreEqual(shape?.Position, asteroid.GetPosition(), "Unexpected position");
        }

        //Realiai same test kaip pozition, nes du beveik vienodi metodai
        [TestMethod]
        public void GetCenterVertex_ReturnsShapesCenterVertex()
        {
            var asteroid = InitializeAsteroid(out var privateObject);

            var shape = privateObject.GetFieldOrProperty("shape") as Shape;

            Assert.AreEqual(shape?.Position, asteroid.getCenterVertex(), "Unexpected center vertex");
        }

        [TestMethod]
        public void Update_ConsistentlyChangesPosition()
        {
            var asteroid = InitializeAsteroid(out _);
            var previousPosition = asteroid.GetPosition();
            asteroid.Update(3);
            var newPosition = asteroid.GetPosition();
            var previousDiff = newPosition - previousPosition;

            for (var i = 0; i < 30; i++)
            {
                previousPosition = newPosition;
                asteroid.Update(3);
                newPosition = asteroid.GetPosition();
                var newDiff = newPosition - previousPosition;

                Assert.AreEqual(previousDiff.X, newDiff.X, 0.1, "Inconsistent position change on X axis after update");
                Assert.AreEqual(previousDiff.Y, newDiff.Y, 0.1, "Inconsistent position change on Y axis after update");
                previousDiff = newDiff;
            }
        }

        //ship pos 0,0 sileLenght 10
        private static IEnumerable<object[]> AsteroidCollectionThatCollideWithShip
        {
            get
            {
                return new[]
                {
                    new object[] {9, new Vector2f(1, 1)},
                    new object[] {10, new Vector2f(10, 10)},
                    new object[] {7, new Vector2f(3, 3)},
                    new object[] {8, new Vector2f(2, 2)}
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(AsteroidCollectionThatCollideWithShip))]
        public void HasCollided_CorrectCollisionWithShip(int rad, Vector2f pos)
        {
            var ship = InitializeShip(out _);
            var asteroid = InitializeAsteroid(out _, rad: rad, pos: pos);

            Assert.IsTrue(asteroid.HasCollided(ship) , "Asteroid collision with ship not registered");
        }

        //ship pos 0,0 sileLenght 10
        private static IEnumerable<object[]> AsteroidCollectionThatNotCollideWithShip
        {
            get
            {
                return new[]
                {
                    new object[] {5, new Vector2f(1, 1)},
                    new object[] {5, new Vector2f(0, 0)},
                    new object[] {3, new Vector2f(3, 3)},
                    new object[] {2, new Vector2f(2, 2)}
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(AsteroidCollectionThatNotCollideWithShip))]
        public void HasCollided_CorrectNotCollisionWithShip(int rad, Vector2f pos)
        {
            var ship = InitializeShip(out _);
            var asteroid = InitializeAsteroid(out _, rad: rad, pos: pos);

            Assert.IsFalse(asteroid.HasCollided(ship), "Asteroid collision with ship not registered");
        }

        [TestMethod]
        public void ShouldExplode_AsteroidHasCollidedWithProjectile()
        {
            var projectile = InitializeProjectile(out _);
            Vector2f pos = new Vector2f(50, 50);
            var projectileFarAway = InitializeProjectile(out _, pos, velocityProj, Direction);
            var asteroid = InitializeAsteroid(out _);

            Assert.IsTrue(asteroid.ShouldExplode(projectile), "Asteroid collision with projectile not detected");
            Assert.IsFalse(asteroid.ShouldExplode(projectileFarAway), "Asteroid collision with projectile detection error");
        }

        [TestMethod]
        public void WillBreakApart_AsteroidShouldBreaklApart()
        {
            var breakRadius = (float)typeof(Asteroid).GetField("MIN_BREAK_APART_RADIUS", BindingFlags.NonPublic | BindingFlags.Static)
                ?.GetValue(null);

            for (int i = (int)breakRadius + 1; i < (int)breakRadius * 3; i++)
            {
                var asteroid = InitializeAsteroid(out _, position, velocity, i);
                Assert.IsTrue(asteroid.WillBreakApart(), "Asteroid not breaking apart when radius is more then min break radius");
            }
            for (int i = 0; i < (int)breakRadius + 1; i++)
            {
                var asteroid = InitializeAsteroid(out _, position, velocity, i);
                Assert.IsFalse(asteroid.WillBreakApart(), "Asteroid breaking apart when radius is less then min break radius");
            }

        }

        private Asteroid InitializeAsteroid(out PrivateObject privateObject, Vector2f? pos = null, Vector2f? vel = null, int? rad = null)
        {
            if (pos is null)
            {
                pos = position;
            }
            if (vel is null)
            {
                vel = velocity;
            }
            if (rad is null)
            {
                rad = Radius;
            }

            var asteroid = new Asteroid(pos.Value, vel.Value, rad.Value);

            privateObject = new PrivateObject(asteroid);

            return asteroid;
        }

        private Ship InitializeShip(out PrivateObject privateObject, Vector2f? pos = null, float? sidLen = null)
        {
            if (pos is null)
            {
                pos = positionShip;
            }
            if (sidLen is null)
            {
                sidLen = SideLenght;
            }

            var ship = new Ship(pos.Value, sidLen.Value);

            privateObject = new PrivateObject(ship);

            return ship;
        }

        private Projectile InitializeProjectile(out PrivateObject privateObject, Vector2f? pos = null, Vector2f? vel = null, int? dir = null)
        {
            if (pos is null)
            {
                pos = positionProj;
            }
            if (vel is null)
            {
                vel = velocityProj;
            }
            if (dir is null)
            {
                dir = Direction;
            }

            var projectile = new Projectile(pos.Value, vel.Value, dir.Value);

            privateObject = new PrivateObject(projectile);

            return projectile;
        }
        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion
    }
}
