using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;

namespace Lexical.Localization.Tests
{
    public class TestUtils
    {
        /// <summary>
        /// Test Equals both ways and hashcode.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void AreEqual(object x, object y)
        {
            Assert.AreEqual(x, y, "x.Equals(y) == false");
            Assert.AreEqual(y, x, "y.Equals(x) == false");
            Assert.IsTrue(x.GetHashCode() == y.GetHashCode(), "x.GetHashCode() != y.GetHashCode()");
        }

        /// <summary>
        /// Test Equals both ways and hashcode.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void AreEqual(object x, object y, object z)
        {
            Assert.AreEqual(x, y, "x.Equals(y) == false");
            Assert.AreEqual(x, z, "x.Equals(z) == false");
            Assert.AreEqual(y, x, "y.Equals(x) == false");
            Assert.AreEqual(y, z, "y.Equals(z) == false");
            Assert.AreEqual(z, x, "z.Equals(x) == false");
            Assert.AreEqual(z, y, "z.Equals(y) == false");
            int xhash = x.GetHashCode(), yhash = y.GetHashCode(), zhash = z.GetHashCode();
            Assert.IsTrue(xhash == yhash, "x.GetHashCode() != y.GetHashCode()");
            Assert.IsTrue(xhash == zhash, "x.GetHashCode() != z.GetHashCode()");
        }

        /// <summary>
        /// Test NotEquals both ways and hashcode.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public static void AreNotEqual(object x, object y)
        {
            Assert.AreNotEqual(x, y, "x.Equals(y) == true");
            Assert.AreNotEqual(y, x, "y.Equals(x) == true");
            Assert.IsFalse(x.GetHashCode() == y.GetHashCode(), "x.GetHashCode() == y.GetHashCode()");
        }

        /// <summary>
        /// Test NotEquals both ways and hashcode.
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <param name="z"></param>
        public static void AreNotEqual(object x, object y, object z)
        {
            Assert.AreNotEqual(x, y, "x.Equals(y) == true");
            Assert.AreNotEqual(x, z, "x.Equals(z) == true");
            Assert.AreNotEqual(y, x, "y.Equals(x) == true");
            Assert.AreNotEqual(y, z, "y.Equals(z) == true");
            Assert.AreNotEqual(z, x, "z.Equals(x) == true");
            Assert.AreNotEqual(z, y, "z.Equals(y) == true");
            int xhash = x.GetHashCode(), yhash = y.GetHashCode(), zhash = z.GetHashCode();
            Assert.IsFalse(xhash == yhash, "x.GetHashCode() == y.GetHashCode()");
            Assert.IsFalse(xhash == zhash, "x.GetHashCode() == z.GetHashCode()");
        }


    }
}
