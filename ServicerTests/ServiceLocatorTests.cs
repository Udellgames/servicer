using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
namespace Tests
{
    [TestFixture()]
    public class ServiceLocatorTests
    {
        [TearDown]
        public void TearDown()
        {
            //We have to reset the service locator because static state is persisted between tests.
            ServiceLocator.Clear();
            ServiceLocator.Explicit = false;
        }

        [Test()]
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

        [Test()]
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

        [Test()]
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

        [Test()]
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

        [Test()]
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

        [Test()]
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

        [Test()]
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

        [Test()]
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

        [Test()]
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

        [Test()]
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

        [Test()]
        [ExpectedException(typeof(KeyNotFoundException))]
        public void Unregister_TestItemNotFound_NoKey()
        {
            var expected = new List<string>()
                {
                    "pass"
                };

            ServiceLocator.Unregister(expected);
        }

        [Test()]
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

        [Test()]
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

        [Test()]
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

        [Test()]
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

        [Test()]
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
    }
}
