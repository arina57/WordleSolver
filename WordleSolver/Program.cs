using Result = WordleSovlerLibrary.Enums.Result;

// See https://aka.ms/new-console-template for more information
Console.WriteLine("Hello, World!");


WordleSovlerLibrary.WordAnalyzer wordAnalyzer = new WordleSovlerLibrary.WordAnalyzer();
while(true)
{
    Console.WriteLine();
    Console.WriteLine("Try one of these:");
    Console.WriteLine(String.Join(", ", wordAnalyzer.SuggestGuess()));
    Console.WriteLine();
    Console.Write("Enter Result: ");
    var input = Console.ReadLine().Split(' ');
    var guess = input[0];
    try
    {
        var result = input[1].Select(value => (Result)Char.GetNumericValue(value)).ToList(); 
        wordAnalyzer.AddGuess(guess, result);
    }
    catch(Exception ex)
    {
        Console.WriteLine("Invalid Input");
    }
}

