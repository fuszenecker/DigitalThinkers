using System;
using System.Linq;
using ServiceTemplate.Domain.Entities;
using ServiceTemplate.Domain.Interfaces;
using ServiceTemplate.Domain.Services;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Tests
{
    [TestClass]
    public class UnitTests
    {
        private IMonetaryService monetaryService = null!;

        [TestInitialize]
        public void Initialize() {
            this.monetaryService = new InMemoryMonetaryService();
        }

        [TestMethod]
        public void EmptyMonetaryService()
        {
            Assert.AreEqual(0, monetaryService.GetCoins().Keys.Count);
        }

        [TestMethod]
        public void InsertMonetaryService()
        {
            monetaryService.StoreCoins(new CoinCollection {
                {5, 1},
                {1, 5},
                {11, 3}
            });

            Assert.AreEqual(3, monetaryService.GetCoins().Keys.Count);
            Assert.AreEqual(1u, monetaryService.GetCoins()[5]);
            Assert.AreEqual(5u, monetaryService.GetCoins()[1]);
            Assert.AreEqual(3u, monetaryService.GetCoins()[11]);
        }

        [TestMethod]
        public void CheckoutNoInsertedMonetaryService()
        {
            Assert.ThrowsException<ArgumentNullException>(() => monetaryService.Checkout(null!, 100));
        }

        [TestMethod]
        public void CheckoutEmptyInsertedMonetaryService()
        {
            var (errorMessage, change) = monetaryService.Checkout(new (), 100);

            Assert.IsNotNull(errorMessage);
            Assert.IsNull(change);
        }

        [TestMethod]
        public void CheckoutTrivialInsertedMonetaryService()
        {
            var (errorMessage, change) = monetaryService.Checkout(new CoinCollection {
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
            var (errorMessage, change) = monetaryService.Checkout(new CoinCollection {
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
            var (errorMessage, change) = monetaryService.Checkout(new CoinCollection {
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
            monetaryService.StoreCoins(new CoinCollection {
                { 10, 200 }
            });

            var (errorMessage, change) = monetaryService.Checkout(new CoinCollection {
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
            monetaryService.StoreCoins(new CoinCollection {
                { 10, 20 },
                { 50, 8 }
            });

            var (errorMessage, change) = monetaryService.Checkout(new CoinCollection {
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
