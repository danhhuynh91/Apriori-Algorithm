﻿using AprioriAlgorithm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace UnitTests
{
    [TestClass()]
    public class AprioriTest
    {
        #region Member Variables

        readonly double _minSupport;
        readonly double _minConfidence;
        readonly IEnumerable<string> _items;
        readonly string[] _transactions;
        readonly Apriori_Accessor _target;

        #endregion

        #region  Constructor

        public AprioriTest()
        {
            _minSupport = .5;
            _minConfidence = .8;
            _items = new string[5] { "a", "b", "c", "d", "e" };
            _transactions = new string[4] { "acd", "bce", "abce", "be" };
            _target = new Apriori_Accessor();
        }

        #endregion

        #region Test Methods

        [TestMethod()]
        public void ProcessTransactionTest()
        {
            //Arrange
            IApriori target = new Apriori();

            //Act
            Output actual = target.ProcessTransaction(_minSupport, _minConfidence, _items, _transactions);

            //Assert
            Assert.AreEqual(9, actual.FrequentItems.Count);
            Assert.AreEqual(2, actual.FrequentItems["a"].Support);
            Assert.AreEqual(3, actual.FrequentItems["b"].Support);
            Assert.AreEqual(3, actual.FrequentItems["c"].Support);
            Assert.AreEqual(3, actual.FrequentItems["e"].Support);
            Assert.AreEqual(2, actual.FrequentItems["ac"].Support);
            Assert.AreEqual(2, actual.FrequentItems["bc"].Support);
            Assert.AreEqual(3, actual.FrequentItems["be"].Support);
            Assert.AreEqual(2, actual.FrequentItems["ce"].Support);
            Assert.AreEqual(2, actual.FrequentItems["bce"].Support);

            Assert.AreEqual(4, actual.ClosedItemSets.Count, "ClosedItemSets calculation is wrong");
            Assert.IsTrue(actual.ClosedItemSets.ContainsKey("c"));
            Assert.IsTrue(actual.ClosedItemSets.ContainsKey("be"));
            Assert.IsTrue(actual.ClosedItemSets.ContainsKey("ac"));
            Assert.IsTrue(actual.ClosedItemSets.ContainsKey("bce"));

            Assert.AreEqual(2, actual.MaximalItemSets.Count);
            Assert.AreEqual("ac", actual.MaximalItemSets[0]);
            Assert.AreEqual("bce", actual.MaximalItemSets[1]);

            Assert.AreEqual(5, actual.StrongRules.Count);
        }

        [TestMethod()]
        public void GetL1FrequentItemsTest()
        {
            //Act
            var actual = _target.GetL1FrequentItems(_minSupport, _items, _transactions);

            //Assert
            Assert.AreEqual(4, actual.Count);
            Assert.AreEqual(2, actual[0].Support);
            Assert.AreEqual(3, actual[1].Support);
            Assert.AreEqual(3, actual[2].Support);
            Assert.AreEqual(3, actual[3].Support);
        }

        [TestMethod()]
        public void GetSupportTest()
        {
            //Arrange
            string candidate = "a";

            //Act
            var actual = _target.GetSupport(candidate, _transactions);

            //Assert
            Assert.AreEqual(2, actual);
        }

        [TestMethod()]
        public void CheckIsSubsetTest()
        {
            //Arrange
            string child = "abc";
            string parent = "abcde";

            //Act
            var actual = _target.CheckIsSubset(child, parent);

            //Assert
            Assert.IsTrue(actual);
        }

        [TestMethod()]
        public void GenerateCandidatesTest()
        {
            //Arrange
            var frequentItems = new List<Item>
            {
             new Item   {Name= "a",Support=2},
                new Item   {Name= "b",Support=3},
                new Item   {Name= "c",Support=3},
                new Item   {Name= "e",Support=3},
            };

            //Act
            var actual = _target.GenerateCandidates(frequentItems, _transactions);

            //Assert
            Assert.AreEqual(actual.Count, 6);

            Assert.AreEqual(actual["ab"], 1);
            Assert.AreEqual(actual["ac"], 2);
            Assert.AreEqual(actual["ae"], 1);
            Assert.AreEqual(actual["bc"], 2);
            Assert.AreEqual(actual["be"], 3);
            Assert.AreEqual(actual["ce"], 2);
        }

        [TestMethod()]
        public void GenerateCandidate_SameFirstElementTest()
        {
            //Act
            var actual = _target.GenerateCandidate("be", "bc");

            //Assert
            Assert.AreEqual(actual, "bec");
        }

        [TestMethod()]
        public void GenerateCandidate_SingleElementsTest()
        {
            //Act
            var actual = _target.GenerateCandidate("a", "b");

            //Assert
            Assert.AreEqual(actual, "ab");
        }

        [TestMethod()]
        public void GenerateCandidate_DifferentFirstElementTest()
        {
            //Act
            var actual = _target.GenerateCandidate("ce", "be");

            //Assert
            Assert.AreEqual(actual, string.Empty);
        }

        [TestMethod()]
        public void GetFrequentItemsTest()
        {
            //Arrange
            var candidates = new Dictionary<string, double>
            {
                {"ab", 1},
                {"ac", 2},
                {"ae", 1},
                {"bc", 2},
                {"be", 3},
                {"ce", 2} 
            };

            //Act
            var actual = _target.GetFrequentItems(candidates, _minSupport, _transactions.Count());

            //Assert
            Assert.AreEqual(actual.Count, 4);

            Assert.AreEqual(actual[0].Support, 2);
            Assert.AreEqual(actual[1].Support, 2);
            Assert.AreEqual(actual[2].Support, 3);
            Assert.AreEqual(actual[3].Support, 2);
        }

        [TestMethod()]
        public void GenerateSubsetsRecursiveTest()
        {
            //Arrange
            string item = "abcd";
            int subsetLength = 2;
            char[] temp = new char[item.Length];
            IList<string> subsets = new List<string>();

            //Act
            _target.GenerateSubsetsRecursive(item, subsetLength, temp, subsets);

            //Assert
            Assert.AreEqual(6, subsets.Count);
            Assert.AreEqual<string>("ab", subsets[0]);
            Assert.AreEqual<string>("ac", subsets[1]);
            Assert.AreEqual<string>("ad", subsets[2]);
            Assert.AreEqual<string>("bc", subsets[3]);
            Assert.AreEqual<string>("bd", subsets[4]);
            Assert.AreEqual<string>("cd", subsets[5]);

        }

        [TestMethod()]
        public void GenerateSubsetsTest()
        {
            //Arrange
            string item = "abcd";

            //Act
            IEnumerable<string> actual = _target.GenerateSubsets(item);

            //Assert
            Assert.AreEqual(10, actual.Count());
        }

        #endregion
    }
}