using System;
using System.Collections.Generic;
using System.Linq;


namespace AnagramSolver
{
    public static class AnagramHelper
    {
        internal static HashSet<string> GetAnagrams(Dictionary<char, byte> phraseIndex, IEnumerable<string> wordList)
        {
            var anagramsPermutation = new HashSet<string>();
            GetAnagrams(phraseIndex, new Stack<string>(), GetEveryCompatibleWord(wordList, phraseIndex), anagramsPermutation);

            return anagramsPermutation;
        }
        internal static Dictionary<char, byte> GetAllCharacterOcurrences(string word)
        {
            return word.GroupBy(c => c).OrderBy(c => c.Key).ToDictionary(group => group.Key, group => Convert.ToByte(group.Count()));
        }

        //We need to check all possible combinations, it's time consuming, the backtraking help but not too much since we have to
        //get to low on the tree to realize that we cannot form that anagram. Also we are taking solutions that was already dropped.
        private static void GetAnagrams(Dictionary<char, byte> phraseIndex, Stack<string> wordStack,
            Dictionary<string, Dictionary<char, byte>> compatibleWords, HashSet<string> anagrams)
        {
            //Do not take again those words which are already on the stack
            foreach (var anagram in compatibleWords
                .Where(p => wordStack.ToList().All(p2 => p.Key != p2) && SourceWordContainsTargetWord(phraseIndex, p.Value)))
            {
                wordStack.Push(anagram.Key);
                //We need to substract letters to see if we have already use all the letters, that will mean a successful anagram
                SubstractLetters(phraseIndex, anagram.Value);
                var phraseLenght = NumberOfCharacters(phraseIndex);

                //We only add the new anagram to the set when all it's letters have been used
                if (phraseLenght == 0)
                {
                    AddAnagram(wordStack.ToArray(), anagrams);
                }
                //There is no point in keep looking when the wordLengh is < 3 due the exercise restriction
                else if (phraseLenght >= 3)
                {
                    GetAnagrams(phraseIndex, wordStack, compatibleWords, anagrams);
                }
                //We need to recover the previous state
                AddLetters(phraseIndex, anagram.Value);
                //Since it's not a valid branch anymore we need to extrac it from the stack
                wordStack.Pop();
            }

        }
      
        private static void AddAnagram(string[] anagramArray, HashSet<string> anagrams)
        {
            Array.Sort(anagramArray);
            var finalstring = string.Join(" ", anagramArray);
            anagrams.Add(finalstring);
        }

        //O(N)
        private static Dictionary<string, Dictionary<char, byte>> GetEveryCompatibleWord(IEnumerable<string> wordlist, IReadOnlyDictionary<char, byte> phraseDictionary)
        {
            var wordsAnagram = new Dictionary<string, Dictionary<char, byte>>();

            foreach (var word in wordlist)
            {
                var wordDictionary = GetAllCharacterOcurrences(word);
                if (SourceWordContainsTargetWord(phraseDictionary, wordDictionary))
                {
                    wordsAnagram.Add(word, wordDictionary);
                }
            }
            return wordsAnagram;
        }

      
        private static bool SourceWordContainsTargetWord(IReadOnlyDictionary<char, byte> sourceWord, IReadOnlyDictionary<char, byte> targetWord)
        {
            //Look that the target is not bigger than the original
            if (NumberOfCharacters(targetWord) > NumberOfCharacters(sourceWord))
            {
                return false;
            }

            //Check character by character, every character on the targetWord must be present and at least in equal number of ocurrences
            foreach (var w in targetWord)
            {
                byte characterOcurrences;
                if (!sourceWord.TryGetValue(w.Key, out characterOcurrences) || characterOcurrences < w.Value)
                {
                    return false;
                }
            }
            return true;
        }

        private static int NumberOfCharacters(IReadOnlyDictionary<char, byte> phraseDictionary)
        {
            return phraseDictionary.Count(character => character.Value > 0);
        }

        private static void AddLetters(IDictionary<char, byte> phraseDictionary, Dictionary<char, byte> wordDictionary)
        {
            foreach (var w in wordDictionary)
            {
                phraseDictionary[w.Key] += w.Value;
            }
        }

        private static void SubstractLetters(IDictionary<char, byte> phraseDictionary, Dictionary<char, byte> wordDictionary)
        {
            foreach (var w in wordDictionary)
            {
                phraseDictionary[w.Key] -= w.Value;
            }
        }
    }
}
