using System;
using System.Collections.Generic;
using System.Linq;
using DigitalThinkers.Domain.Interfaces;
using DigitalThinkers.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class UnitTests
    {
        private IMonetaryService monetaryService;

        [TestInitialize]
        public void Initialize() {
            this.monetaryService = new InMemoryMonetaryService();
        }

        [TestMethod]
        public void EmptyMonetaryService()
        {
            Assert.AreEqual(0, monetaryService.GetNotes().Keys.Count);
        }

        [TestMethod]
        public void InsertMonetaryService()
        {
            monetaryService.StoreNotes(new Dictionary<uint, uint> {
                {5, 1},
                {1, 5},
                {11, 3}
            });

            Assert.AreEqual(3, monetaryService.GetNotes().Keys.Count);
            Assert.AreEqual(1u, monetaryService.GetNotes()[5]);
            Assert.AreEqual(5u, monetaryService.GetNotes()[1]);
            Assert.AreEqual(3u, monetaryService.GetNotes()[11]);
        }

        [TestMethod]
        public void CheckoutNoInsertedMonetaryService()
        {
            Assert.ThrowsException<ArgumentNullException>(() => monetaryService.Checkout(null, 100));
        }

        [TestMethod]
        public void CheckoutEmptyInsertedMonetaryService()
        {
            var (errorMessage, change) = monetaryService.Checkout(new Dictionary<uint, uint>(), 100);

            Assert.IsNotNull(errorMessage);
            Assert.IsNull(change);
        }

        [TestMethod]
        public void CheckoutTrivialInsertedMonetaryService()
        {
            var (errorMessage, change) = monetaryService.Checkout(new Dictionary<uint, uint> {
                { 100, 1}
            }, 100);

            Assert.IsNull(errorMessage);
            Assert.IsNotNull(change);

            // Nothing is paid back.
            Assert.AreEqual(0, change.Count);
        }

        [TestMethod]
        public void CheckoutEqualInsertedMonetaryService()
        {
            var (errorMessage, change) = monetaryService.Checkout(new Dictionary<uint, uint> {
                { 10, 1},
                { 90, 1}
            }, 100);

            Assert.IsNull(errorMessage);
            Assert.IsNotNull(change);

            // Nothing is paid back.
            Assert.AreEqual(0, change.Count);
        }

        [TestMethod]
        public void CheckoutMoreInsertedMonetaryService()
        {
            var (errorMessage, change) = monetaryService.Checkout(new Dictionary<uint, uint> {
                { 10, 1},
                { 90, 1}
            }, 90);

            Assert.IsNull(errorMessage);
            Assert.IsNotNull(change);

            // Nothing is paid back.
            Assert.AreEqual(1, change.Count);
            Assert.AreEqual(10u, change.First().Key);
            Assert.AreEqual(1u, change.First().Value);
        }

        [TestMethod]
        public void CheckoutAndStoreMonetaryService()
        {
            monetaryService.StoreNotes(new Dictionary<uint, uint> {
                { 10, 200 }
            });

            var (errorMessage, change) = monetaryService.Checkout(new Dictionary<uint, uint> {
                { 1000, 1}
            }, 200);

            Assert.IsNull(errorMessage);
            Assert.IsNotNull(change);

            // Nothing is paid back.
            Assert.AreEqual(1, change.Count);
            Assert.AreEqual(10u, change.First().Key);
            Assert.AreEqual(80u, change.First().Value);
        }

        [TestMethod]
        public void CheckoutAndStoreComplexMonetaryService()
        {
            monetaryService.StoreNotes(new Dictionary<uint, uint> {
                { 10, 20 },
                { 50, 8 }
            });

            var (errorMessage, change) = monetaryService.Checkout(new Dictionary<uint, uint> {
                { 500, 1},
                { 100, 1}
            }, 200);

            Assert.IsNull(errorMessage);
            Assert.IsNotNull(change);

            // Nothing is paid back.
            Assert.AreEqual(2, change.Count);
            Assert.AreEqual(100u, change.First().Key);
            Assert.AreEqual(1u, change.First().Value);
            Assert.AreEqual(50u, change.Last().Key);
            Assert.AreEqual(6u, change.Last().Value);
        }
    }
}
