using System.Collections.Generic;
using System.Linq;
using Asteroids;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SFML.System;

namespace Tests
{
    [TestClass]
    public class ExtensionTest
    {
        private static IEnumerable<object[]> SubArrayCombinations
        {
            get
            {
                return new[]
                {
                    new object[] {new [] {1, 2, 3, 4, 5}, 1, 2, new [] {2, 3}},
                    new object[] {new [] {1, 2, 3, 4, 5}, 2, 3, new [] {3, 4, 5}},
                    new object[] {new [] {1, 2, 3, 4, 5}, 0, 5, new [] {1, 2, 3, 4, 5}},
                };
            }
        }
        [TestMethod]
        [DynamicData(nameof(SubArrayCombinations))]
        public void Helper_SubArray_WorksCorrectly(int[] data, int index, int length, int[] result)
        {
            var calculatedResult = data.SubArray(index, length);

            Assert.AreEqual(result.Length, calculatedResult.Length);

            var areEqualNumbers = result.Zip(calculatedResult, (r, cr) => r == cr);
            foreach (var isEqualNumber in areEqualNumbers)
            {
                Assert.IsTrue(isEqualNumber);
            }
        }

        private static IEnumerable<object[]> DegRadCombinations
        {
            get
            {
                return new[]
                {
                    new object[] {45f, 0.785f},
                    new object[] {90f, 1.57f},
                    new object[] {270f, 4.712f},
                    new object[] {-180f, -3.141f},
                };
            }
        }
        [TestMethod]
        [DynamicData(nameof(DegRadCombinations))]
        public void MathExtension_DegToRads_WorksCorrectly(float deg, float rads)
        {
            Assert.AreEqual(rads, deg.degToRads(), 1e-3);
        }

        private static IEnumerable<object[]> DotProducts
        {
            get
            {
                return new[]
                {
                    new object[] {new Vector2f(5, 2), new Vector2f(10, 15), 80},
                    new object[] {new Vector2f(22, 12), new Vector2f(-5, 3), -74},
                    new object[] {new Vector2f(7, 5), new Vector2f(-8, 4), -36},
                    new object[] {new Vector2f(5, 2), new Vector2f(0, 0), 0},
                };
            }
        }
        [TestMethod]
        [DynamicData(nameof(DotProducts))]
        public void VectorExtension_DotProduct_WorksCorrectly(Vector2f a, Vector2f b, float dotProduct)
        {
            Assert.AreEqual(dotProduct, a.DotProduct(b), 1e-3);
        }

        private static IEnumerable<object[]> Magnitudes
        {
            get
            {
                return new[]
                {
                    new object[] {new Vector2f(1, 1), 1.414f},
                    new object[] {new Vector2f(5, 2), 5.385f},
                    new object[] {new Vector2f(4, -4), 5.656f},
                    new object[] {new Vector2f(0, 10), 10f},
                    new object[] {new Vector2f(10, 0), 10f},
                };
            }
        }

        [TestMethod]
        [DynamicData(nameof(Magnitudes))]
        public void VectorExtension_Magnitude_WorksCorrectly(Vector2f vector, float magnitude)
        {
            Assert.AreEqual(magnitude, vector.Magnitude(), 1e-3);
        }
    }
}