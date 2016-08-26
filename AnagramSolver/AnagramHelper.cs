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
        internal static Dictionary<char, byte> GetAllCharOcurrences(string word)
        {
            //Calculate for each character the number of ocurrences on the given string
            return word.GroupBy(c => c).OrderBy(c => c.Key).ToDictionary(group => group.Key, group => Convert.ToByte(group.Count()));
        }

        //We need to check all possible combinations, it's time consuming, the backtraking help but not too much since we have to
        //get to low on the tree to realize that we cannot form that anagram. There are a better way to prune?
        //Another problem is try branches already seen. 
        private static void GetAnagrams(Dictionary<char, byte> phraseIndex, Stack<string> currentWords,
            Dictionary<string, Dictionary<char, byte>> compatibleWords, HashSet<string> anagrams)
        {
            if (RemainLettersToCompleteAnagram(phraseIndex) != 0)
            {
                //Do not take again those words which are already on the stack
                var possibleOptions = compatibleWords.Where(p => currentWords.ToList().All(p2 => p.Key != p2));
                foreach (var anagram in possibleOptions)
                {
                    //There is no point on keeping searching if the phraseIndex's lenght is less than 3
                    if (RemainLettersToCompleteAnagram(phraseIndex) < 3 || !SourceWordContainsTargetWord(phraseIndex, anagram.Value))
                    {
                        continue;
                    }

                    currentWords.Push(anagram.Key);
                    AddPossibleWordToAnagram(phraseIndex, anagram.Value);

                    GetAnagrams(phraseIndex, currentWords, compatibleWords, anagrams);

                    RemoveFromAnagram(phraseIndex, anagram.Value);
                    currentWords.Pop();
                }
            }
            else
            {
                AddCompleteAnagram(currentWords.ToArray(), anagrams);
            }
        }

        //We added the string to the HashSet, since same strings will have same hash repeated ones are not going to be inserted.
        private static void AddCompleteAnagram(string[] anagramArray, HashSet<string> anagrams)
        {
            //We first sort alphabetically
            Array.Sort(anagramArray);
            var finalstring = string.Join(" ", anagramArray);
            anagrams.Add(finalstring);
        }

        //O(N), we need to traverse the whole list of words and check for each one if are contained inside the given phrase
        private static Dictionary<string, Dictionary<char, byte>> GetEveryCompatibleWord(IEnumerable<string> wordlist, IReadOnlyDictionary<char, byte> phraseDictionary)
        {
            var wordsAnagram = new Dictionary<string, Dictionary<char, byte>>();

            foreach (var word in wordlist)
            {
                var wordDictionary = GetAllCharOcurrences(word);
                if (SourceWordContainsTargetWord(phraseDictionary, wordDictionary))
                {
                    wordsAnagram.Add(word, wordDictionary);
                }
            }
            return wordsAnagram;
        }

        //How to know if a word contains another word
        private static bool SourceWordContainsTargetWord(IReadOnlyDictionary<char, byte> sourceWord, Dictionary<char, byte> targetWord)
        {
            //First we check if the target word is bigger.
            if (targetWord.Count > sourceWord.Count)
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

        private static int RemainLettersToCompleteAnagram(Dictionary<char, byte> phraseDictionary)
        {
            return phraseDictionary.Count(character => character.Value > 0);
        }

        private static void AddPossibleWordToAnagram(IDictionary<char, byte> phraseDictionary, Dictionary<char, byte> wordDictionary)
        {
            foreach (var word in wordDictionary)
            {
                phraseDictionary[word.Key] += word.Value;
            }
        }

        private static void RemoveFromAnagram(IDictionary<char, byte> phraseDictionary, Dictionary<char, byte> wordDictionary)
        {
            foreach (var word in wordDictionary)
            {
                phraseDictionary[word.Key] -= word.Value;
            }
        }
    }
}
