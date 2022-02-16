using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace WordleSovlerLibrary
{
    public class WordAnalyzer
    {

        private char[] letters = "abcdefghijklmnopqrstuvwxyz".ToCharArray();
        private HashSet<char> notUsedLetters = new HashSet<char>();
        private char?[] knownPositionLetters = new char?[5];
        private List<char>[] knownNotUsedPositions = Enumerable.Range(1, 5).Select(x => new List<char>()).ToArray();
        private HashSet<char> knownLetters = new HashSet<char>();

        private char[][] letterPositionRanking;
        private static string FilePath => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), @"Resources\words_alpha.txt");
        private IEnumerable<string> validWords = File.ReadAllLines(FilePath).Where(word => word.Length == 5).Select(word => word.ToLower());
        public Dictionary<char,int> LetterWordCount(IEnumerable<string> wordList) => letters.Select(letter => (letter, wordList.Sum(word => word.Contains(letter) ? 1 : 0))).ToDictionary(item => item.letter, item => item.Item2);
        

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


        public Dictionary<char, int>[] LetterPositionRank(IEnumerable<string> words)
        {
            var letterRanks = Enumerable.Range(1, 5).Select(x => letters.ToDictionary(letter => letter, letter => 0)).ToArray();
            foreach (var word in words)
            {
                for (var i = 0; i < word.Length; i++)
                {
                    letterRanks[i][word[i]]++;
                }
            }    
            return letterRanks;
        }

        public IEnumerable<string> FindValidWords(IEnumerable<string> wordList) => 
            wordList.Where((word) => !word.Where((letter, i) =>
                            knownNotUsedPositions[i].Contains(word[i]) || (knownPositionLetters[i] != null && knownPositionLetters[i] != word[i])).Any()
                            && !word.Any(letter => notUsedLetters.Contains(letter))
                            && knownLetters.All(letter => knownLetters.Count == 0 || word.Contains(letter)));


        public Dictionary<string, int> RankWordByLetterCount(IEnumerable<string> wordsToRank)
        {
            var letterCount = LetterWordCount(wordsToRank);
            var wordScore = wordsToRank.ToDictionary(word => word, word => word.Sum(letter => word.Count(l => letter == l) == 1 ? letterCount[letter] : 0));
            return wordScore;
        }

        public Dictionary<string, int> RankWordsByLetterPosition(IEnumerable<string> wordsToRank)
        {
            var positionRank = LetterPositionRank(wordsToRank);
            var wordPosScore = wordsToRank.ToDictionary(word => word, word => word.Select((letter, index) => (letter, index)).Sum(item => positionRank[item.index][item.letter]));
            return wordPosScore;
        }
        public List<string> SuggestGuess()
        {
            validWords = FindValidWords(validWords);
            var wordsWithoutTwoOfSameLetter = RankWordsByLetterPosition(validWords).Where(word => word.Key.All(letter => word.Key.Count(l => l == letter) == 1));
            return wordsWithoutTwoOfSameLetter.OrderByDescending(score => score.Value).Select(word => word.Key).Take(10).ToList(); 
        }
      }
}