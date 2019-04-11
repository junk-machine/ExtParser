# ExtParser
**Prototype of an endless lookahead parallel parser.**

This is a prototype of a parser that spawns new branches (`Task`s) everytime there is an ambiguity in the grammar. In `LL(k)` parsers (i.e. **JavaCC**) you have to explicitly specify LOOKAHEAD to resolve ambiguity and it always gets me wondering why can't it figure out the `k` itself? Well, technically it can, like **ANTLR** adaptive prediction does, but I wanted to do my own investigation as to what can be potential problems with just trying to match input on all potential branches, until only one branch is left.

Another interesting thing I wanted to tackle is the necessity of lexer (aka tokenizer). If you look at it - lexer is nothing, but a parser of its own for a stream of characters, so why not just treat every character as a token and feed that stream to the parser directly? Gut feeling is that it's just way faster to process a stream of 10 tokens, rather than 100 characters, but you still have to process 100 characters to group them into lexemes, so it's not that obvious.

With this, I set on a mission to quickly craft simple parser that will deal with any `ITokenStream<TToken>` and won't require you to explicitly specify `LOOKAHEAD`. Instead it will create new parsing context (mainly token stream, that can be separately adjusted without affecting other branches) and start new `Task` to continue parsing from that context.

## Quick solution overview

`ExtParser.Core` implements common interfaces and generic matching rules that can operate on any token stream.
`ExtParser.Text` builds on-top of the `Core` and introduces concrete `CharacterStream` as well as base `TextParser` that can deal with stream of characters.
`ExtParser.Text.GrammarParser` is an implementation of the parser that can parse grammar for itself. That's it, you can parse a text file that defines a grammar for said text file.
`ExtParser.Test` is a console application that uses `GrammarParser` to parse self-grammar file and prints generated parser code on the console. Here I have two grammar files: `ExtParserGrammar.eg` and `ExtParserGrammarOptimized.eg`. Former one is the full clean grammar of the grammar files consumed by the `GrammarParser`, while the later is optimized version, where all *terminal* rules are implemented in the code to speed up the matching process.

Unlike traditional parser generators, one can notice that all of the matching rules (*zero-or-more*, *one-or-more*, *optional*, etc.) are implemented in the `Core` project as code. This is what differentiates it from other parsers, which compile into a static set of switch-case statements. With this approach, generated code is easily understandable and parser logic can be overriden in runtime. This allows you to generate an **extendable parser** (hence the project name), which can be extended with new rules after it ships. Another advantage of this approach is that grammar can easily be defined in code, rather than in the specific text format and text format for the grammar itself can be extended.

## Lessons learned

Here's what I learned along the way:
 - Things like *spaces*, *tabs* and *line feeds* that can appear at any point in the input stream are hard without proper parser support.
   With a very simple parser you will have to explicitly skip these characters in every rule, for example `Sum := '0'..'9' '+' '0'..'9' ;` will have to be augmented with `[' ']` before and after the `'+'` in order to allow spaces. To address this problem, in **ExtParser** I introduced the concept of *Global Rules*, which are similar to all other rules, but they can be matched anywhere. What this means is that infrastructure will automatically augment your rule as following: `Sum := [ GlobalRule1 | ... ] '0'..'9' [ GlobalRule1 | ... ] '+' [ GlobalRule1 | ... ] '0'..'9' [ GlobalRule1 | ... ] ;`. In the grammar, global rules are marked with `*` after the rule name. At the same time there are places where you don't want such global rules to match, for example when matching a string identifier `Identifier := ( 'a'..'z' | 'A'..'Z' )* ;`. In such cases you can exclude some or all global rules by overriding `ProductionParserRule.ExcludedGlobalRules` property or using `!` syntax in the grammar.
 - *Zero-or-more*, *one-or-more* quantifiers are very slow, if all branches have to be considered.
   Think about it this way: `( 'a'..'z' )* '0'..'9'` has to create branch after every *a-z* character match, even though most of these branches will just die out. I had this "perfect" implementation at first, but it turned out to be super slow (yeah, one could re-implement everything in assembly, but it would still be order of magnitude slower, than any other parser). Without having a better solution after 10 seconds of thinking, I updated it to perform a "greedy" match, where it basically tries to consume as many tokens as possible within the quantifier rule, before considering following tokens. As a result of greedy matching, you cannot easily define "ends with" rules, like `( 'a'..'z' | '0'..'9' )* '0'..'9'`. You can still implement them in the code, though.
 - Similar to the previous bullet point - matching single character in a separate branch is super slow.
   Rules like `Identifier := ( 'a'..'z' | 'A'..'Z' | '0'..'9' )* ;` will produce 3 branches for every iteration and the more alternatives you have - the slower it is. To address this, you can define rules without the body in your grammar, e.g. `Identifier ;`, which will allow you to reference it in other rules, but you will have to implement it in the code.
 - Bunch of other interesting things that you can learn by implementing something yourself, rather than reading a Wikipedia article or a white paper.

Of course there are better solutions to all of these problems, but my goal was not to create yet another parser generator. Instead, by doing this quick prototype I wanted to estimate what it will take to create such a "perfect" parser that can also be extended after it was generated.

Happy coding!
