using Result = WordleSovlerLibrary.Enums.Result;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


WordleSovlerLibrary.WordAnalyzer wordAnalyzer = new WordleSovlerLibrary.WordAnalyzer();
Console.WriteLine(wordAnalyzer.SuggestGuess());
wordAnalyzer.AddGuess("aesir", new List<Result> { Result.Yellow, Result.Grey, Result.Grey, Result.Grey, Result.Grey });
Console.WriteLine(wordAnalyzer.SuggestGuess());
wordAnalyzer.AddGuess("notal", new List<Result> { Result.Grey, Result.Grey, Result.Grey, Result.Yellow, Result.Yellow });
Console.WriteLine(wordAnalyzer.SuggestGuess());
wordAnalyzer.AddGuess("dault", new List<Result> { Result.Grey, Result.Green, Result.Green, Result.Green, Result.Grey });
Console.WriteLine(wordAnalyzer.SuggestGuess());
wordAnalyzer.AddGuess("baulk", new List<Result> { Result.Grey, Result.Green, Result.Green, Result.Green, Result.Green });
Console.WriteLine(wordAnalyzer.SuggestGuess());
