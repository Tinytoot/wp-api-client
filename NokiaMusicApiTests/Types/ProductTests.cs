﻿// -----------------------------------------------------------------------
// <copyright file="ProductTests.cs" company="NOKIA">
// Copyright (c) 2012, Nokia
// All rights reserved.
// </copyright>
// -----------------------------------------------------------------------
using System;
using System.Text;
using Newtonsoft.Json.Linq;
using Nokia.Music.Phone.Tests.Properties;
using Nokia.Music.Phone.Types;
using NUnit.Framework;

namespace Nokia.Music.Phone.Tests.Types
{
    /// <summary>
    /// Product tests
    /// </summary>
    [TestFixture]
    public class ProductTests
    {
        private const string TestId = "id";
        private const string TestName = "name";

        [Test]
        public void TestProperties()
        {
            Product product = new Product() { Id = TestId, Name = TestName };

            Assert.AreEqual(TestId, product.Id, "Expected the property to persist");
            Assert.AreEqual(TestName, product.Name, "Expected the property to persist");
        }

        [Test]
        public void TestOverrides()
        {
            Product product = new Product() { Id = TestId, Name = TestName };
            Assert.IsNotNull(product.GetHashCode(), "Expected a hash code");
            Assert.IsTrue(product.Equals(new Product() { Id = TestId }), "Expected equality");
            Assert.IsFalse(product.Equals(TestId), "Expected inequality");
        }

        [Test]
        public void TestJsonParsing()
        {
            JObject json = JObject.Parse(Encoding.UTF8.GetString(Resources.product_parse_tests));

            JArray items = json.Value<JArray>("items");

            // Test a full representation
            Product fullItem = Product.FromJToken(items[0]) as Product;
            Assert.IsNotNull(fullItem, "Expected a product object");
            Assert.IsNotNull(fullItem.TakenFrom, "Expected an album");
            Assert.IsNotNull(fullItem.Genres, "Expected genres");
            Assert.Greater(fullItem.Genres.Length, 0, "Expected genres");
            Assert.IsNotNull(fullItem.Id, "Expected an id");
            Assert.IsNotNull(fullItem.Name, "Expected a name");
            Assert.Greater(fullItem.Performers.Length, 0, "Expected performers");
            Assert.IsNotNull(fullItem.Price, "Expected a price");
            Assert.IsNotNull(fullItem.Thumb100Uri, "Expected a 100x100 thumb");
            Assert.IsNotNull(fullItem.Thumb200Uri, "Expected a 200x200 thumb");
            Assert.IsNotNull(fullItem.Thumb320Uri, "Expected a 320x320 thumb");
            Assert.AreEqual(fullItem.Category, Category.Track, "Expected a track");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestIdPropertyIsRequiredForShow()
        {
            Product product = new Product();
            product.Show();
        }

        [Test]
        public void TestShowGoesAheadWhenItCan()
        {
            Product product = new Product() { Id = TestId };
            product.Show();
            Assert.Pass();
        }
    }
}
