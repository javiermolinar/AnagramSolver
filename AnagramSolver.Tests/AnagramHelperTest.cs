using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AnagramSolver.Tests
{
    [TestClass]
    public class AnagramHelperTest
    {
        [TestMethod]
        public void TestGetAllAnagramsShouldReturnAllThePossibleAnagrams()
        {
            IEnumerable<string> wordlist = File.ReadAllLines("wordlist.txt").Where(x => x.Length >= 3);
            var anagrams = AnagramHelper.GetAnagrams(AnagramHelper.GetAllCharacterOcurrences("best secret"), wordlist);

            Assert.IsNotNull(anagrams);
            Assert.AreEqual(16, anagrams.Count);
        }
    }
}
