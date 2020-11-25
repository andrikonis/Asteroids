using Microsoft.VisualStudio.TestTools.UnitTesting;
using Asteroids;
using SFML.Graphics;
using System;
using SFML.System;
using System.Collections.Generic;
using System.Reflection;

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
        public void UpdateTest(float dt)
        {
            //var ship = InitShip(out var privateObject);
            //var shape = privateObject.GetFieldOrProperty("shape") as Shape;
            //var hasThrust = ship.
            //ship.Thrust(-1);
            //ship.
            throw new NotImplementedException();

        }

        [TestMethod]
        public void ThrustTest()
        {
            //InitShip(out var pObject);
            //var velocity = pObject.GetFieldOrProperty("velocity");
            //var shape = pObject.GetFieldOrProperty("shape") as Shape;

            //var headingRads = shape.Rotation.degToRads();
            //var x = velocity.X + (float)Math.Sin(headingRads) * 30 * -1;
            //pObject.Invoke("Thrust", -1);
            //Assert.AreEqual(new Vector2f())
            throw new NotImplementedException();
        }

        [TestMethod]
        public void RotateTest()
        {
            //var ship = InitShip(out var privateObject);
            //var rotationPower = (float) typeof(Ship).GetField("rotationPower", BindingFlags.NonPublic | BindingFlags.Static)?.GetValue(null);
            ////privateObject.Invoke("Rotate", 1);
            //ship.Rotate(1);
            //var angularVelocity = (float) typeof(Ship).GetField("angularVelocity", BindingFlags.NonPublic).GetValue(null);
            ////Assert.AreEqual(angularVelocity, angularVelocity2, "Incorrect Velocity");
            throw new NotImplementedException();
        }


        [TestMethod]
        public void ShootTest()
        {
            throw new NotImplementedException();
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
