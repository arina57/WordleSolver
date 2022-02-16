using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace WordleSovlerLibrary
{
    public class WordAnalyzer
    {

        private char[] letters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        private List<char> letterRanking = new List<char>();
        private HashSet<char> notUsedLetters = new HashSet<char>();
        private char?[] knownPositionLetters = new char?[5];
        private List<char>[] knownNotUsedPositions = Enumerable.Range(1, 5).Select(x => new List<char>()).ToArray();
        private HashSet<char> knownLetters = new HashSet<char>();

        private static string FilePath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\words_alpha.txt");
        private List<string> wordList = File.ReadAllLines(FilePath).Where(word => word.Length == 5).Select(word => word.ToLower()).ToList();

        private Dictionary<char,int> letterCount = new Dictionary<char,int>();

        public void RankLetters()
        {
            letterCount = letters.ToDictionary(item => item, item => 0);
            
            foreach(var word in wordList)
            {
                     foreach(var key in letterCount.Keys)
                    {
                    letterCount[key] += word.Count(word => word == key);
                    }

            }

            var letterRank = letterCount.OrderByDescending(keyPair => keyPair.Value);
            letterRanking = letterRank.Select(keyPair => keyPair.Key).ToList();
            
        }


        public void AddGuess(string guess, List<Enums.Result> result)
        {
            if(guess.Length != 5) { throw new ArgumentOutOfRangeException("Guess must be 5 chars long"); }
            if(result.Count != 5) { throw new ArgumentOutOfRangeException("result must be 5 long"); }

            for (int i = 0; i < result.Count; i++)
            {
                switch(result[i])
                {
                    case Enums.Result.Green:
                        knownPositionLetters[i] = guess[i];
                        knownLetters.Add(guess[i]);
                        break;
                    case Enums.Result.Yellow:
                        knownLetters.Add(guess[i]);
                        knownNotUsedPositions[i].Add(guess[i]);
                        break;
                    case Enums.Result.Grey:
                        notUsedLetters.Add(guess[i]);
                        break;
                    default: throw new ArgumentOutOfRangeException(result[i] + " not valid value");

                }
            }
        }

        public string SuggestGuess()
        {
            wordList = wordList.Where((word) =>
            {
                var hasCorrectPos = true;
                for(int i = 0; i < knownLetters.Count && hasCorrectPos; i++)
                {
                    if(knownNotUsedPositions[i].Contains(word[i]))
                    {
                        hasCorrectPos = false;
                    }
                    if(knownPositionLetters[i] != null)
                    {
                        if (knownPositionLetters[i] != word[i])
                        {
                            hasCorrectPos = false;
                        }
                    }
 
                }
                var doesntContainWrongLetters = !word.Any(letter => notUsedLetters.Contains(letter)); 
                var containsNeededLetters = knownLetters.All(letter => knownLetters.Count == 0 || word.Contains(letter));
                var canditate = hasCorrectPos && doesntContainWrongLetters && containsNeededLetters;
                return canditate;
            }).ToList();
            RankLetters();
            var topLetters = letterRanking.Take(5);
            var wordScore = wordList.ToDictionary(word => word, word => word.Sum(letter => word.Count(l => letter == l) == 1 ? letterCount[letter] : 0)).OrderByDescending(score => score.Value);
            return wordScore.First().Key;
        }

        public static bool ContainsAllItems<T>(IEnumerable<T> a, IEnumerable<T> b)
        {
            return !b.Except(a).Any();
        }
    }

 
}