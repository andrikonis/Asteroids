using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asteroids;
using SFML.Graphics;
using System;
using SFML.System;
using System.Collections.Generic;
using System.Reflection;
using SFML.Window;

namespace Tests
{
    /// <summary>
    /// Summary description for ShipTest
    /// </summary>
    [TestClass]
    public class ShipTest
    {
        private Vector2f position = new Vector2f(5, 5);
        private float shipLength = 20;
        private float elapsedTime = 1.0f / 30;
        public ShipTest()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
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
        [TestMethod]
        public void DrawTest()
        {
            RenderWindow window = new RenderWindow(new VideoMode(5, 5), "Test") { };
            var ship = InitShip(out var privateObject);
            ship.Thrust(1); // hasThrust turns true
            ship.Rotate(1); // hasSpin turns true
            ship.Draw(window); // should reset hasThrust and hasSpin to false
            Assert.AreEqual(false, privateObject.GetField("hasThrust"), "Incorrect Thrust");
            Assert.AreEqual(false, privateObject.GetField("hasSpin"), "Incorrect Spin");
        }


        [TestMethod]
        public void UpdateTest()
        {
            var ship = InitShip(out var privateObject);
            var shape = privateObject.GetFieldOrProperty("shape") as Shape;
            var thrusterRotationOffset = (float)privateObject.GetField("THRUSTER_ROTATION_OFFSET", BindingFlags.NonPublic | BindingFlags.Static);
            var expected = shape.Rotation + thrusterRotationOffset;
            ship.Update(elapsedTime);
            var jetShape = privateObject.GetFieldOrProperty("jetShape") as Shape;
            Assert.AreEqual(expected, jetShape.Rotation, "Incorrect Jet Shape Rotation");
            // ChargeShot, Kinematics and GetThrusterPosition tested seperatly

        }

        [TestMethod]
        public void ThrustTest()
        {
            var ship = InitShip(out var privateObject);
            ship.Thrust(1);
            var velocity = (Vector2f)privateObject.GetFieldOrProperty("velocity");
            var hasThrust = (bool)privateObject.GetField("hasThrust");
            var shape = privateObject.GetFieldOrProperty("shape") as Shape;
            var thrustPower = (float)privateObject.GetField("thrustPower", BindingFlags.NonPublic | BindingFlags.Static);
            var headingRads = shape.Rotation.degToRads();
            var x = (float)Math.Sin(headingRads) * thrustPower;
            var y = (float)Math.Cos(headingRads) * thrustPower;
            var expected = new Vector2f(0+x, 0+y);
            Assert.AreEqual(expected, velocity, "Incorrect Velocity");
            Assert.AreEqual(true, hasThrust, "Unexpected Thrust");
        }

        [TestMethod]
        public void RotateTest()
        {
            var ship = InitShip(out var privateObject);
            ship.Rotate(1);
            var rotationPower = (float) privateObject.GetField("rotationPower", BindingFlags.NonPublic | BindingFlags.Static);
            var angularVelocity = (float)privateObject.GetField("angularVelocity");
            var hasSpin = (bool)privateObject.GetField("hasSpin");

            Assert.AreEqual(rotationPower, angularVelocity, "Incorrect Rotation");
            Assert.AreEqual(true, hasSpin, "Unexpected Spin");
        }


        [TestMethod]
        public void ShootTest()
        {
            Dictionary<string, Projectile> dictProjectiles = new Dictionary<string, Projectile> { };
            var ship = InitShip(out var privateObject);
            var shape = privateObject.GetFieldOrProperty("shape") as Shape;
            // GetGunPosition tested in another method 
            for (int i = 0; i < 11; i++) // makes isShotCharged true and shotCounter not 9
            {
                privateObject.Invoke("ChargeShot");
            }
            Byte expected = 0;
            ship.Shoot(dictProjectiles); // should reset isShotCharged and shotCounter
            Assert.AreEqual(false, ship.IsShotCharged, "Incorrect Charge");
            Assert.AreEqual(expected, privateObject.GetField("shotCounter"), "Incorrect Shot Counter");
        }

        [TestMethod]
        public void CorrectColorTest()
        {
            InitShip(out var ship);
            var shape = ship.GetFieldOrProperty("shape") as Shape;
            var jetShape = ship.GetFieldOrProperty("jetShape") as Shape;

            Assert.AreEqual(Color.Black, shape.FillColor, "Incorrect Shape Color");
            Assert.AreEqual(Color.Cyan, jetShape.FillColor, "Incorrect Jet Shape Color");
        }
        [TestMethod]
        public void ChargeShotTest()
        {
            var ship = InitShip(out var privateObject);
            privateObject.Invoke("ChargeShot");
            var shotCounter = (byte)privateObject.GetField("shotCounter");
            var wantsToShoot = (bool)privateObject.GetField("wantsToShoot");
            Assert.AreEqual(1, shotCounter, "Incorrect shot counter");
            Assert.AreEqual(false, wantsToShoot, "Incorrect Shot Intention");
            Assert.AreEqual(false, ship.IsShotCharged, "Incorrect Shot Charge");
            for (int i = 0; i < 10; i++)
            {
                privateObject.Invoke("ChargeShot");
            }
            shotCounter = (byte)privateObject.GetField("shotCounter");
            Assert.AreEqual(10, shotCounter, "Incorrect shot counter");
            Assert.AreEqual(true, ship.IsShotCharged, "Incorrect Shot Charge");
        }


        [TestMethod] 
        public void IsShotChargedTest()
        {
            var ship = InitShip(out var privateObject);
            Assert.AreEqual(false, ship.IsShotCharged, "Unecpected Shot Charge");
            for (int i = 0; i < 11; i++) // takes 10 shot counters to turn to charged shot 
            {
                privateObject.Invoke("ChargeShot");
            }
            
            Assert.AreEqual(true, ship.IsShotCharged, "Unexpected Shot Discharge");
        }

        [TestMethod]
        public void KinematicsTest()
        {
            var ship = InitShip(out var privateObject);

            var shape = privateObject.GetFieldOrProperty("shape") as Shape;
            var velocity = (Vector2f)privateObject.GetFieldOrProperty("velocity");
            var angularVelocity = (float)privateObject.GetField("angularVelocity");
            var decayRate = (float)privateObject.GetField("decayRate", BindingFlags.NonPublic | BindingFlags.Static);
            var angularDecayRate = (float)privateObject.GetField("angularDecayRate", BindingFlags.NonPublic | BindingFlags.Static);

            var expectedPos = shape.Position;
            expectedPos += velocity * elapsedTime;
            var expectedRot = shape.Rotation;
            expectedRot += angularVelocity * elapsedTime;
            var expectedVelocity = velocity * decayRate;
            var expectedAngularVelocity = angularVelocity * angularDecayRate;
            privateObject.Invoke("Kinematics", elapsedTime);
            Assert.AreEqual(expectedPos, shape.Position, "Incorrect Position");
            Assert.AreEqual(expectedRot, shape.Rotation, "Incorrect Rotation");
            velocity = (Vector2f)privateObject.GetFieldOrProperty("velocity");
            angularVelocity = (float)privateObject.GetField("angularVelocity");
            Assert.AreEqual(expectedVelocity, (Vector2f)privateObject.GetFieldOrProperty("velocity"), "Incorrect Velocity");
            Assert.AreEqual(expectedAngularVelocity, (float)privateObject.GetFieldOrProperty("angularVelocity"), "Incorrect Angular Velocity");
            ship.Thrust(1);
            ship.Rotate(1);
            Assert.AreNotEqual(expectedVelocity, (Vector2f)privateObject.GetFieldOrProperty("velocity"), "Incorrect Velocity");
            Assert.AreNotEqual(expectedAngularVelocity, (float)privateObject.GetFieldOrProperty("angularVelocity"), "Incorrect Angular Velocity");
        }

        [TestMethod]
        public void CorrectShapeTest()
        {
            InitShip(out var ship);
            var shape = ship.GetFieldOrProperty("shape") as Shape;
            var jetShape = ship.GetFieldOrProperty("jetShape") as Shape;

            Assert.AreEqual(new Vector2f(shipLength, shipLength), shape.Origin, "Incorrect Shape Origin");
            Assert.AreEqual(new Vector2f(shipLength/2, shipLength/1.5f), jetShape.Origin, "Incorrect Jet Shape Origin");
            Assert.AreEqual(position, shape.Position, "Incorrect Shape Position");
            Assert.AreEqual(position, jetShape.Position, "Incorrect Jet Shape Position");
        }

        // Assert not possible for vector2f list, because equals methos is not implemented,  
        // and it fails because objects are in different locations in memory 
        [TestMethod]
        public void GetVerticesTest()
        {
            var ship = InitShip(out var privateObject);
            var shape = privateObject.GetFieldOrProperty("shape") as Shape;
            var points = new List<Vector2f> { };
            for (Byte i = 0; i < shape.GetPointCount(); i++)
            {
                points.Add((shape.Transform.TransformPoint(shape.GetPoint(i))));
            }

            for (var i = 0; i < points.Count; i++)
            {
                Assert.AreEqual(points[i], ship.GetVertices()[i], "Incorrect Vertices");
            }
            //Assert.AreEqual(points, ship.GetVertices(), "bad");
        }

        [TestMethod]
        public void GetGunPositionTest()
        {
            InitShip(out var privateObject);
            var shape = privateObject.GetFieldOrProperty("shape") as Shape;
            var expected = shape.Transform.TransformPoint(shape.GetPoint(3));
            var actual = privateObject.Invoke("GetGunPosition");
            Assert.AreEqual(expected, actual, "Incorrect Gun Position");
        }

        [TestMethod]
        public void GetThrusterPositionTest()
        {
            InitShip(out var privateObject);
            var shape = privateObject.GetFieldOrProperty("shape") as Shape;
            var p1 = shape.Transform.TransformPoint(shape.GetPoint(1));
            var p2 = shape.Transform.TransformPoint(shape.GetPoint(2));
            var expected = p2 + ((p1 - p2) / 2);
            var actual = privateObject.Invoke("GetThrusterPostion");
            Assert.AreEqual(expected, actual, "Incorrect Thruster Position");
        }


        private Ship InitShip(out PrivateObject privateObject, Vector2f? pos = null, float? sideLength = null)
        {
            if(pos is null)
            {
                pos = position;
            }
            if(sideLength is null)
            {
                sideLength = shipLength;
            }


            var ship = new Ship(pos.Value, sideLength.Value);

            privateObject = new PrivateObject(ship);

            return ship;
        }
    }
}
