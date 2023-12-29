# CofigurableCompiler
This repository will contain a compiler that can be configured to read any sort of code if provided the corresponding configuration file for the language used.



* [x] Make a simple language representation to be used in the lexer and parser.

  * Make a key to represent a sertain part of the lex/parsing process.

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

* [ ] Add aliases for tokens and constructs.

* [ ] Make the behaviour of the compiler configurable from outside.

* [ ] Deside on a way to configure the compiler outside of code with a config file.

* [ ] Make a language loader.