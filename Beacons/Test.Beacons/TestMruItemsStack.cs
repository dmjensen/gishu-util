// Copyright (c) 2010-11 Gishu Pillai
// See the file license.txt for copying permission

using System;
using System.Linq;
using ManiaX.Beacons;
using ManiaX.Beacons.Roles;
using Moq;
using NUnit.Framework;

namespace ManiaX.Test.Beacons
{
    [TestFixture]
    public class TestMruItemsStack
    {
        private MruItemsStack _stack;
        private Mock<SettingsRepository> _mockSettingsStore;

        [SetUp]
        public void CreateAStackOfSize3()
        {
            _stack = new MruItemsStack("StackName", 3);
            _mockSettingsStore = new Mock<SettingsRepository>();
        }
        [Test]
        public void CanLoadItemsFromARepository()
        {
            _mockSettingsStore.Setup(store => store["StackName1"]).Returns("A");
            _mockSettingsStore.Setup(store => store["StackName2"]).Returns("B");
            _mockSettingsStore.Setup(store => store["StackName3"]).Returns("C");
            
            _stack.LoadFrom(_mockSettingsStore.Object);

            Assert.That(_stack.Items.Count, Is.EqualTo(3), "should have loaded 3 items from store");
            _mockSettingsStore.Verify();
        }

        [Test]
        public void LoadsReturnsNonBlankValuesFromRepository()
        {
            _mockSettingsStore.Setup(store => store["StackName1"]).Returns("A");
            _mockSettingsStore.Setup(store => store["StackName2"]).Returns("B");
            _mockSettingsStore.Setup(store => store["StackName3"]).Returns(String.Empty);

            _stack.LoadFrom(_mockSettingsStore.Object);

            Assert.That(_stack.Items, Is.EqualTo(new []{"A", "B"}));
        }

        [Test]
        public void CanHaveItemsPushedOntoIt()
        {
            Assert.That(_stack.Items, Is.Empty);
            
            _stack.Push("ABC");
            _stack.Push("DEF");

            Assert.That(_stack.Items[0], Is.EqualTo("DEF"), "last pushed item should be on top");
            Assert.That(_stack.Items[1], Is.EqualTo("ABC"));
        }

        [Test]
        public void DiscardsOldestValueWhenAnItemIsPushedOntoFullStack()
        {
            _stack.Push("123");
            _stack.Push("456");
            _stack.Push("789");
            _stack.Push("101");
            
            Assert.That(_stack.Items.Count, Is.EqualTo(3), "Stack should have been full");
            Assert.That(_stack.Items, Contains.Item("101"), "latest item is missing from the stack");
            Assert.That(_stack.Items, Has.No.Contains("123"), "oldest items should have been discarded");
        }

        [Test]
        public void ItemIsPromotedToTopOfStackWhenAnExistingItemIsPushed()
        {
            _stack.Push("123");
            _stack.Push("456");
            _stack.Push("789");
            _stack.Push("456");

            Assert.That(_stack.Items[0], Is.EqualTo("456"), "latest item should be on top");
            Assert.That(_stack.Items.Count(item => item.Equals("456")), Is.EqualTo(1), "should not contain duplicates");
        }

        [Test]
        public void NotifiesObserversWhenItemsAreAdded()
        {
            var notificationCount = 0;
            _stack.Items.CollectionChanged += delegate { notificationCount++; };

            _stack.Push("123");
            _stack.Push("456");
            

            Assert.That(notificationCount, Is.EqualTo(notificationCount));
        }

        [Test]
        public void CanSaveItemsToARepository()
        {
            _mockSettingsStore.SetupSet(store => store["StackName1"] = "789");
            _mockSettingsStore.SetupSet(store => store["StackName2"] = "456");
            _mockSettingsStore.SetupSet(store => store["StackName3"] = "123");

            _stack.Push("123");
            _stack.Push("456");
            _stack.Push("789");
            _stack.SaveTo(_mockSettingsStore.Object);

            _mockSettingsStore.VerifyAll();
        }

        [Test]
        public void SavesExtraBlankValuesWhenSaveIsCalledAndStackIsNotFull()
        {
            _mockSettingsStore.SetupSet(store => store["StackName1"] = "456");
            _mockSettingsStore.SetupSet(store => store["StackName2"] = "123");
            _mockSettingsStore.SetupSet(store => store["StackName3"] = string.Empty);

            _stack.Push("123");
            _stack.Push("456");
            _stack.SaveTo(_mockSettingsStore.Object);

            _mockSettingsStore.VerifyAll();
        }
    }
}
