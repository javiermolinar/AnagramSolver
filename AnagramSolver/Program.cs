using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;


namespace AnagramSolver
{
    static class Program
    {
        private static void Main(string[] args)
        {
            if (args.Length != 2 || string.IsNullOrEmpty(args[0]) || string.IsNullOrEmpty(args[1]))
            {
                Console.WriteLine("Proper arguments must be used");
            }
            IEnumerable<string> wordlist = File.ReadAllLines(args[0]).Where(x => x.Length >= 3);

            Stopwatch watch = Stopwatch.StartNew();

            Regex rgx = new Regex("[^a-z]");
            HashSet<string> anagrams = AnagramHelper.GetAnagrams(AnagramHelper.GetAllCharacterOcurrences(rgx.Replace(args[1].ToLower(), "")), wordlist);

            watch.Stop();

            foreach (var anagram in anagrams)
            {
                Console.WriteLine(anagram);
            }

            Console.WriteLine($"Time elapsed: {watch.Elapsed}");
        }
    }
}
