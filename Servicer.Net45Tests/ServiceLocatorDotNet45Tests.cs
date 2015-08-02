namespace Servicer.Tests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using NUnit.Framework;

    [TestFixture]
    public class ServiceLocatorDotNet45Tests
    {
        [Test]
        public void GetPromise_TestSameType_TestAlreadyFulfilled_Keyed()
        {
            var expected = new List<string>()
            {
                "foo"
            };

            ServiceLocator.Register(expected, 1);

            var promise = ServiceLocator.GetPromisedService<List<string>>(1);

            var doneFired = false;

            promise.Done += (o, e) => doneFired = true;

            var actual = promise.Require();

            Assert.That(actual, Is.SameAs(expected));
            Assert.That(doneFired, Is.True);
        }

        [Test]
        public void GetPromise_TestSameType_TestAlreadyFulfilled_Unkeyed()
        {
            var expected = new List<string>()
            {
                "foo"
            };

            ServiceLocator.Register(expected);

            var promise = ServiceLocator.GetPromisedService<List<string>>();

            var doneFired = false;

            promise.Done += (o, e) => doneFired = true;

            var actual = promise.Require();

            Assert.That(actual, Is.SameAs(expected));
            Assert.That(doneFired, Is.True);
        }

        [Test]
        public void GetPromise_TestSameType_TestNotYetFulfilled_Keyed()
        {
            var expected = new List<string>()
            {
                "foo"
            };

            var promise = ServiceLocator.GetPromisedService<List<string>>(1);

            var doneFired = false;

            promise.Done += (o, e) =>
            {
                Assert.That(e.Value, Is.SameAs(expected));
                doneFired = true;
            };

            ServiceLocator.Register(expected, 1);

            Assert.That(doneFired, Is.True);
        }

        [Test]
        public void GetPromise_TestSameType_TestNotYetFulfilled_Unkeyed()
        {
            var expected = new List<string>()
            {
                "foo"
            };

            var promise = ServiceLocator.GetPromisedService<List<string>>();

            var doneFired = false;

            promise.Done += (o, e) =>
            {
                Assert.That(e.Value, Is.SameAs(expected));
                doneFired = true;
            };

            ServiceLocator.Register(expected);

            Assert.That(doneFired, Is.True);
        }

        [Test]
        [ExpectedException(typeof(PromiseUnfulfilledException))]
        public void GetPromise_TestSameType_TestRequireWhenKeyUnfulfilled_Keyed_ThrowsException()
        {
            var expected = new List<string>()
            {
                "foo"
            };

            var promise = ServiceLocator.GetPromisedService<List<string>>(1);
            ServiceLocator.Register(expected, 2);
            var actual = promise.Require();
        }

        [Test]
        [ExpectedException(typeof(PromiseUnfulfilledException))]
        public void GetPromise_TestSameType_TestRequireWhenUnfulfilled_Keyed_ThrowsException()
        {
            var promise = ServiceLocator.GetPromisedService<List<string>>(1);

            var actual = promise.Require();
        }

        [Test]
        [ExpectedException(typeof(PromiseUnfulfilledException))]
        public void GetPromise_TestSameType_TestRequireWhenUnfulfilled_ThrowsException()
        {
            var expected = new List<string>()
            {
                "foo"
            };

            var promise = ServiceLocator.GetPromisedService<List<string>>();

            var actual = promise.Require();
        }

        [Test]
        public void GetPromise_TestSuperClass_TestAlreadyFulfilled_Keyed()
        {
            var expected = new List<string>()
            {
                "foo"
            };

            ServiceLocator.Register(expected, 1);

            var promise = ServiceLocator.GetPromisedService<IEnumerable<string>>(1);

            var doneFired = false;

            promise.Done += (o, e) => doneFired = true;

            var actual = promise.Require();

            Assert.That(actual.Single(), Is.EqualTo(expected.Single()));
            Assert.That(doneFired, Is.True);
        }

        [Test]
        public void GetPromise_TestSuperClass_TestAlreadyFulfilled_Unkeyed()
        {
            var expected = new List<string>()
            {
                "foo"
            };

            ServiceLocator.Register(expected);

            var promise = ServiceLocator.GetPromisedService<IEnumerable<string>>();

            var doneFired = false;

            promise.Done += (o, e) => doneFired = true;

            var actual = promise.Require();

            Assert.That(actual.Single(), Is.EqualTo(expected.Single()));
            Assert.That(doneFired, Is.True);
        }

        [Test]
        public void GetPromise_TestSuperClass_TestNotYetFulfilled_Keyed()
        {
            var expected = new List<string>()
            {
                "foo"
            };

            var promise = ServiceLocator.GetPromisedService<IEnumerable<string>>(1);

            var doneFired = false;

            promise.Done += (o, e) =>
            {
                Assert.That(e.Value.First(), Is.EqualTo(expected.First()));
                doneFired = true;
            };

            ServiceLocator.Register(expected, 1);

            Assert.That(doneFired, Is.True);
        }

        [Test]
        public void GetPromise_TestSuperClass_TestNotYetFulfilled_Unkeyed()
        {
            var expected = new List<string>()
            {
                "foo"
            };

            var promise = ServiceLocator.GetPromisedService<IEnumerable<string>>();

            var doneFired = false;

            promise.Done += (o, e) =>
            {
                Assert.That(e.Value.First(), Is.EqualTo(expected.First()));
                doneFired = true;
            };

            ServiceLocator.Register(expected);

            Assert.That(doneFired, Is.True);
        }

        [Test]
        public void GetService_TestSameType_Keyed()
        {
            ServiceLocator.Explicit = true;

            var expected = new List<string>()
                {
                    "pass"
                };

            var unexpected = new List<string>()
                {
                    "fail"
                };

            ServiceLocator.Register(expected, 1);
            ServiceLocator.Register(unexpected);

            var actual = ServiceLocator.GetService<List<string>>(1);

            Assert.That(actual, Is.SameAs(expected));
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetService_TestSameType_Keyed_ItemDoesNotExist()
        {
            ServiceLocator.Explicit = true;

            var expected = new List<string>()
                {
                    "pass"
                };

            var unexpected = new List<string>()
                {
                    "fail"
                };

            ServiceLocator.Register(expected, 1);
            ServiceLocator.Register(unexpected);

            var actual = ServiceLocator.GetService<List<string>>(2);
        }

        [Test]
        public void GetService_TestSameType_NoKey()
        {
            ServiceLocator.Explicit = true;

            var expected = new List<string>()
                {
                    "pass"
                };

            var unexpected = new List<string>()
                {
                    "fail"
                };

            ServiceLocator.Register(expected);
            ServiceLocator.Register(unexpected, 1);

            var actual = ServiceLocator.GetService<List<string>>();

            Assert.That(actual, Is.SameAs(expected));
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetService_TestSameType_NoKey_ItemDoesNotExist()
        {
            ServiceLocator.Explicit = true;

            var expected = new List<string>()
                {
                    "pass"
                };

            var unexpected = new List<string>()
                {
                    "fail"
                };

            ServiceLocator.Register(expected, 2);
            ServiceLocator.Register(unexpected, 1);

            var actual = ServiceLocator.GetService<List<string>>();
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetService_TestSuperClass_Explicit_Keyed()
        {
            ServiceLocator.Explicit = true;

            var expected = new List<string>()
                {
                    "pass"
                };

            var unexpected = new List<string>()
                {
                    "fail"
                };

            ServiceLocator.Register(expected, 1);
            ServiceLocator.Register(unexpected);

            var actual = ServiceLocator.GetService<IEnumerable<string>>(1);

            Assert.That(actual, Is.SameAs(expected));
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetService_TestSuperClass_Explicit_NoKey()
        {
            ServiceLocator.Explicit = true;

            var expected = new List<string>()
                {
                    "pass"
                };

            var unexpected = new List<string>()
                {
                    "fail"
                };

            ServiceLocator.Register(expected);
            ServiceLocator.Register(unexpected, 1);

            var actual = ServiceLocator.GetService<IEnumerable<string>>();

            Assert.That(actual, Is.SameAs(expected));
        }

        [Test]
        public void GetService_TestSuperClass_Keyed()
        {
            var expected = new List<string>()
                {
                    "pass"
                };

            var unexpected = new List<string>()
                {
                    "fail"
                };

            ServiceLocator.Register(expected, 1);
            ServiceLocator.Register(unexpected);

            var actual = ServiceLocator.GetService<IEnumerable<string>>(1);

            Assert.That(actual, Is.SameAs(expected));
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetService_TestSuperClass_KeyedItemDoesNotExist()
        {
            var expected = new List<string>()
                {
                    "pass"
                };

            var unexpected = new List<string>()
                {
                    "fail"
                };

            ServiceLocator.Register(expected, 0);
            ServiceLocator.Register(unexpected, 2);

            var actual = ServiceLocator.GetService<IEnumerable<string>>(1);
        }

        [Test]
        public void GetService_TestSuperClass_NoKey()
        {
            var expected = new List<string>()
                {
                    "pass"
                };

            var unexpected = new List<string>()
                {
                    "fail"
                };

            ServiceLocator.Register(expected);
            ServiceLocator.Register(unexpected, 1);

            var actual = ServiceLocator.GetService<IEnumerable<string>>();

            Assert.That(actual, Is.SameAs(expected));
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void GetService_TestSuperClass_NoKey_UnkeyedItemDoesNotExist()
        {
            var expected = new List<string>()
                {
                    "pass"
                };

            var unexpected = new List<string>()
                {
                    "fail"
                };

            ServiceLocator.Register(expected, 2);
            ServiceLocator.Register(unexpected, 1);

            var actual = ServiceLocator.GetService<IEnumerable<string>>();
        }

        [Test]
        public async void GetServiceAsync_TestSameType_TestAlreadyFulfilled_Keyed()
        {
            var expected = new List<string>()
            {
                "foo"
            };

            ServiceLocator.Register(expected, 1);

            var actual = await ServiceLocator.GetServiceAsync<List<string>>(1);

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public async void GetServiceAsync_TestSameType_TestAlreadyFulfilled_Unkeyed()
        {
            var expected = new List<string>()
            {
                "foo"
            };

            ServiceLocator.Register(expected);

            var actual = await ServiceLocator.GetServiceAsync<List<string>>();

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public async void GetServiceAsync_TestSameType_TestNotYetFulfilled_Keyed()
        {
            var expected = new List<string>()
            {
                "foo"
            };

            var task = ServiceLocator.GetServiceAsync<List<string>>(1);

            ServiceLocator.Register(expected, 1);

            var actual = await task;

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public async void GetServiceAsync_TestSameType_TestNotYetFulfilled_Unkeyed()
        {
            var expected = new List<string>()
            {
                "foo"
            };

            var task =ServiceLocator.GetServiceAsync<List<string>>();

            ServiceLocator.Register(expected);

            var actual = await task;

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public async void GetServiceAsync_TestSuperClass_TestAlreadyFulfilled_Keyed()
        {
            var expected = new List<string>()
            {
                "foo"
            };

            ServiceLocator.Register(expected, 1);

            var actual = await ServiceLocator.GetServiceAsync<IEnumerable<string>>(1);

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public async void GetServiceAsync_TestSuperClass_TestAlreadyFulfilled_Unkeyed()
        {
            var expected = new List<string>()
            {
                "foo"
            };

            ServiceLocator.Register(expected);

            var actual = await ServiceLocator.GetServiceAsync<IEnumerable<string>>();

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public async void GetServiceAsync_TestSuperClass_TestNotYetFulfilled_Keyed()
        {
            var expected = new List<string>()
            {
                "foo"
            };

            var task= ServiceLocator.GetServiceAsync<IEnumerable<string>>(1);

            ServiceLocator.Register(expected, 1);

            var actual = await task;

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        public async void GetServiceAsync_TestSuperClass_TestNotYetFulfilled_Unkeyed()
        {
            var expected = new List<string>()
            {
                "foo"
            };

            var task = ServiceLocator.GetServiceAsync<IEnumerable<string>>();

            ServiceLocator.Register(expected);

            var actual = await task;

            Assert.That(actual, Is.EquivalentTo(expected));
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Register_TestItemAlreadyExists_Keyed()
        {
            var expected = new List<string>()
                {
                    "pass"
                };

            ServiceLocator.Register(expected, 1);

            ServiceLocator.Register(expected, 1);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Register_TestItemAlreadyExists_Unkeyed()
        {
            var expected = new List<string>()
                {
                    "pass"
                };

            ServiceLocator.Register(expected);

            ServiceLocator.Register(expected);
        }

        [Test]
        public void Register_TestSameItemDifferentKeys()
        {
            var expected = new List<string>()
                {
                    "pass"
                };

            ServiceLocator.Register(expected, 1);

            ServiceLocator.Register(expected, 2);

            Assert.Pass();
        }

        [TearDown]
        public void TearDown()
        {
            // We have to reset the service locator because static state is persisted between tests.
            ServiceLocator.Clear();
            ServiceLocator.Explicit = false;
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void Unregister_TestItemNotFound_Keyed()
        {
            var expected = new List<string>()
                {
                    "pass"
                };
            ServiceLocator.Register(expected, 1);
            ServiceLocator.Unregister(expected, 2);
        }

        [Test]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void Unregister_TestItemNotFound_NoKey()
        {
            var expected = new List<string>()
                {
                    "pass"
                };

            ServiceLocator.Unregister(expected);
        }

        [Test]
        public void Unregister_TestReAddDifferentItem_Keyed()
        {
            var expected = new List<string>()
            {
                "pass"
            };

            var unexpected = new List<string>()
            {
                "fail"
            };

            ServiceLocator.Register(unexpected, 0);
            ServiceLocator.Unregister(unexpected, 0);
            ServiceLocator.Register(expected, 0);

            Assert.That(ServiceLocator.GetService<List<string>>(0), Is.SameAs(expected));
        }

        [Test]
        public void Unregister_TestReAddDifferentItem_NoKey()
        {
            var expected = new List<string>()
            {
                "pass"
            };

            var unexpected = new List<string>()
            {
                "fail"
            };

            ServiceLocator.Register(unexpected);
            ServiceLocator.Unregister(unexpected);
            ServiceLocator.Register(expected);

            Assert.That(ServiceLocator.GetService<List<string>>(), Is.SameAs(expected));
        }

        [Test]
        public void Unregister_TestReAddSameType_Keyed()
        {
            var expected = new List<string>()
                {
                    "pass"
                };

            ServiceLocator.Register(expected, 0);
            ServiceLocator.Unregister(expected, 0);
            ServiceLocator.Register(expected, 0);

            Assert.Pass();
        }

        [Test]
        public void Unregister_TestReAddSameType_NoKey()
        {
            var expected = new List<string>()
                {
                    "pass"
                };

            ServiceLocator.Register(expected);
            ServiceLocator.Unregister(expected);
            ServiceLocator.Register(expected);

            Assert.Pass();
        }
    }
}