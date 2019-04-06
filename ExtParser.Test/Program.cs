using ExtParser.Text.GrammarParser;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading;

namespace ExtParser.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var timer = Stopwatch.StartNew();

            var parseTree =
                new ExtGrammarParser()
                    .Parse(
                        File.ReadAllText("ExtParserGrammarOptimized.eg"),
                        CancellationToken.None)
                    .Result;

            if (parseTree == null)
            {
                Console.WriteLine("Parsing failed!");
            }
            else
            {
                new ExtGrammarCodeGenWalker("ExtParser2.Text", "ExtGrammar", Console.Out)
                    .Walk(parseTree);
            }

            Console.WriteLine(timer.Elapsed);
            Console.ReadLine();
        }
    }
}
