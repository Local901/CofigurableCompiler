# ConfigurableCompiler (ConC)
This repository will contain a compiler that can be configured to read any sort of code if provided the corresponding configuration file for the language used.

## Current state

At the moment the library contain's a lexer and a parser that use the configured tokens and constructs to know what to lex and parse. 


### SimpleLexer

The Lexer has a few options to define its behavior. The options are flags that can be OR'ed together.

* `CompleteLanguage`: This flag makes sure that all the tokens in the language of the provided references are considered. This is done first.
* `FromAliasRoot`: This flag insures that the root parent(s) of tokens are used for lexing. This has as advantage that it would require less tokens have to be checked, but a disadvantage is that the parser would have a harder time knowing if it should use the token. At the moment it is best to also have `ResolveAlias` enabled together with this flag.
* `ResolveAlias`: This flag will try to add all valid child alias tokens in the result. If the value of the lexed block corresponds with the alias it will be added to the output.

The lexer returns `IValueBlock` objects that hold all related data to the lexed token.

### SimpleParser

The parser uses the lexer and constructs to combine tokens together. The SimpleParser can't handle looping constructs at the start of the construct. It will not break but it will not have he desired result. There won't be any errors to tell you wat went wrong at the moment. And if it fails to parse it will return NULL.

## Get started

First a language has to be configured.

### Add tokens

Tokens need a unique name and a regular expression to function. Adding it to the language returns the reference used in most interaction with other keys (Token and Construct are Keys). The regex follows the c# regex standard for now.

```cs
using ConCore.Key;
using ConCore.Key.Collections;

var language = new LangCollection("example_lang");

// Add tokens
var tIdentifierRef = language.Add(new Token("identifier", "[a-zA-Z_][^\\s]"));
.
.
.
```

### Add constructs

Constructs are combinations of tokens, Constructs and/or groups. It uses components to know in what order to construct itself.

```cs
using ConCore.Key.Components;

var cFunctionRef = language.Add(new Construct("example_function", new OrderComponent(new List<IComponent> {
  new ValueComponent(tFunction),
  new ValueComponent(tIdentifier, "function_name"),
  new ValueComponent(tOpenBracket),
  ...
})));
```

Types of components:
* ValueComponent
* OrderComponent
* AnyComponent
* RepeatComponent (Repeats can't be directly nested in each other)

### Add Groups

Groups can be collections of tokens, constructs and groups to combine them together.

```cs
var groupTest = new KeyGroup("test", new List<KeyLangReference> {
  tTrue,
  cTestOperation,
});

groupTest.Add(tFalse); // References can be added afterwards as well.

var gTest = language.Add(groupTest);
```

# Plans

* [x] Make a simple language representation to be used in the lexer and parser.

  * Make a key to represent a certain part of the lex/parsing process.

    * `Token`: for representing string values from the input code.

    * `Construct`: for representing larger structures made of `Tokens`.

  * Add a `KeyCollection`.
  * Add `LanguageCollection`.

    * This is what is saved in the `KeyCollection`.

    * Hold the language specific `Tokens` and `Constructs`.

* [X] Make simple Lexer.

  * Uses `Tokens` to lex a string.

  * When finding the next block Return one that is closesd to the last.

* [X] Make simple Parser.

  * Expect the code to be correct. No errors will be created / returns null.

  * Can tollerate multiple branching interpretations of the code.

* [X] Be able to parse multiple files using import like statements.

  * Add filters to a language.

    * A filter to find the start of a language for parsing. Might be better just have a reference to it in the language collection.

    * A filter to find file references in the parsed content.

  * Add file data and link related files and prevent recurive importing. It will make a relation to the parsed file.

* [X] Add aliases for tokens and constructs.

  * Make Lexer hadle Token aliases.

* [X] Clean up Library.

* [X] Make library publicly available.

* [ ] Create configurable pipeline.

* [ ] Make it able to transpile.

* [ ] Make a language loader.

* [ ] Make it able to compile.

* [ ] Make my own regex resolver. To make sure it behaves the way that it should.

* [ ] Decide how to use Contruct aliasses, if not remove behaviour.

* [ ] Deside on a way to configure the compiler outside of code with a config file.