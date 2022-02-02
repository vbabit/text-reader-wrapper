TextReaderWrapper - Reads Text by Specified Pattern
===================================================

A wrapper on top of the standard [`System.IO.TextReader`](https://docs.microsoft.com/en-us/dotnet/api/system.io.textreader),
which supports sequential asynchronous reading according to a given pattern, and extracts the required pieces of text.

This reader can be useful for parsing texts of custom predefined formats, for automating manual time-consuming copy-paste tasks,
or if you are going to implement some code that should have non-trivial reading logic.

Besides, the reader provides an API for extending its [internal operations](#abstract-syntax-tree) and
[the pattern syntax](#extending-pattern-syntax).

## Getting Started ##

The pattern can essentially contain two commands: `R` and `S` (leaving all their varieties aside for now).
`R` stands for a **Read** operation. Such an operation reads characters from the source text,
converts them into a single string, and adds the string to the output collection.
`S`, in its turn, stands for a **Skip** operation, which simply skips the required characters.

**Example #1**

*Input*

```
Hello World
```

*Pattern*

```csharp
// R[5] - read 5 chars
// S.   - skip 1 char
// R>   - read remaining chars
const string pattern = @"R[5] S. R>";
```

*Output*

```
["Hello", "World"]
```

**Example #2**

*Input*

```
* Apple
* Lemon
* Pear
* Kiwi
```

*Pattern*

```csharp
// S>     - skip line
// S+'* ' - skip * and space
// R>     - read remaining chars in the line
// {2}    - repeat twice
const string pattern = @"(S> S+'* ' R>){2}";
```

*Output*

```
["Lemon", "Kiwi"]
```

**Example #3**

*Input*

```
* 7:00  Wake-up
* 9:00  At work
* 10:00 Stand-up meeting
* 12:00 Lunch
* 16:00 Yet another meeting
...
```

*Pattern*

```csharp
// S|/rgx/ - skip all characters up to the string that matches the specified regex
// {&R}    - and read the matched string
// S+/\s*/ - skip whitespace
// R>      - read remaining chars in the line
// *       - repeat till the end of the text
const string pattern = @"
(
    S| /\d{1,2}:\d{2}/ {&R}
    S+ /\s*/
    R>
)*
";
```

*Output*

```
[
    "7:00",
    "Wake-up",
    "9:00",
    "At work",
    "10:00",
    "Stand-up meeting",
    "12:00",
    "Lunch",
    "16:00",
    "Yet another meeting",
    ...
]
```

### TextReaderWrapper ###

This is the "entry-point" class that wraps `System.IO.TextReader` using the following constructor:

```csharp
public TextReaderWrapper(System.IO.TextReader underlyingReader)
```

#### ReadAsync() ####

```csharp
// `pattern` - defines how to read the source text.
// `output` - contains the strings read from the source text.
public async Task ReadAsync(string pattern, ICollection<string> output, CancellationToken cancelToken = default)
```

```csharp
const string pattern = @"(R.)*";
const string text = "Lorem ipsum";
var output = new List<string>();
var stringReader = new StringReader(text);
using (var textReaderWrapper = new TextReaderWrapper(stringReader))
{
    textReaderWrapper.ReadAsync(pattern, output).Wait();
}

// output: ["L", "o", "r", "e", "m", " ", "i", "p", "s", "u", "m"]
```

## Full Syntax ##

**Notations**

`<...>` - placeholder (not a literal pattern token).

### Read/Skip Fixed-Length String ###

| Pattern    | Operation                   |
|------------|-----------------------------|
| `R[<num>]` | Reads `<num>` character(s). |
| `S[<num>]` | Skips `<num>` character(s). |

`<num>` must be greater than **0**.

**Shorthands**

* `R.` == `R[1]`
* `S.` == `S[1]`

**Examples**

| Input    | Pattern   | Output          |
|----------|-----------|-----------------|
| `FooBar` | `R[3]`    | `["Foo"]`       |
| `FooBar` | `R.R.R.`  | `["F","o","o"]` |
| `FooBar` | `S[3]R[3]`| `["Bar"]`       |

### Read/Skip Line ###

| Pattern | Operation                               |
|---------|-----------------------------------------|
| `R>`    | Reads remaining characters in the line. |
| `S>`    | Skips remaining characters in the line. |

The reader position is set to the next line. The line break is **not** included in the output.

**Examples**

| Input              | Pattern  | Output    |
|------------------- |----------|-----------|
| `FooBar`<br/>`Baz` | `S[3]R>` | `["Bar"]` |
| `FooBar`<br/>`Baz` | `S>R>`   | `["Baz"]` |

### Read/Skip up to Boundary String ###

| Pattern      | Operation                                                                             |
|--------------|---------------------------------------------------------------------------------------|
| `R\|'<str>'` | Reads all characters preceding the specified string (stops **before** `<str>`).       |
| `R+'<str>'`  | Reads all characters up to the end of the specified string (stops **after** `<str>`). |
| `S\|'<str>'` | Skips all characters preceding the specified string (stops **before** `<str>`).       |
| `S+'<str>'`  | Skips all characters up to the end the specified string (stops **before** `<str>`).   |

**Boundary Strings**

| Boundary string | Description                  | Character escapes |
|-----------------|------------------------------|-------------------|
| `'<str>'`       | **Case-sensitive** string.   | `\'`&nbsp;(to&nbsp;escape&nbsp;`'`)<br/>`\\`&nbsp;(to&nbsp;escape&nbsp;`\`)<br/>`\r`&nbsp;(carriage&nbsp;return)<br/>`\n`&nbsp;(line&nbsp;feed) |
| `~<str>~`       | **Case-insensitive** string. | `\~`&nbsp;(to&nbsp;escape&nbsp;`~`)<br/>`\\`&nbsp;(to&nbsp;escape&nbsp;`\`)<br/>`\r`&nbsp;(carriage&nbsp;return)<br/>`\n`&nbsp;(line&nbsp;feed) |
| `/<rgx>/`       | Regular expression.          | `\/`&nbsp;(to&nbsp;escape&nbsp;`/`)<br/>[Other&nbsp;escaping&nbsp;rules](https://docs.microsoft.com/en-us/dotnet/standard/base-types/character-escapes-in-regular-expressions) |

Boundary strings **must not be empty**.

**NOTE**

In order to bypass C#'s own escaping rules, it's better to define the pattern as a
[verbatim string literal](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/tokens/verbatim)
(a string with the `@` prefix):

```csharp
@"S+'file:\\'"
```

Otherwise (without the `@` prefix), the same pattern would have to be defined as follows:

```csharp
"S+'file:\\\\'"
```

The `@` prefix can also allow you to define a multiline pattern to improve readability, e.g.:

```csharp
@"(
    R[10]
    S|'foo'
    R+'bar'
    S>
)"
```

**Read & Skip**

```
R|'<...>'{&S}
```

Reads all preceding characters & **skips** the specified string.

**Skip & Read**

```
S|'<...>'{&R}
```

Skips all preceding characters & **reads** the specified string.

**Boundary String Sequence**

```
R| ['<str1>'? ~<str2>~? ... /<rgx>/]

R+ ['<str1>'? ~<str2>~? ... /<rgx>/]

S| ['<str1>'? ~<str2>~? ... /<rgx>/]

S+ ['<str1>'? ~<str2>~? ... /<rgx>/]
```

The boundary string sequence may include any number and different types of boundary strings.
The items inside the sequence must be separated by `?`.

The operation searches for a piece of the source text that matches one of the strings (or regular expressions)
specified in the sequence.

❗ The operation is not "greedy", i.e. when it finds the first piece of text that matches a boundary string in the sequence,
any subsequent boundary strings are not taken into account. Therefore, the order of the sequence matters.

**Examples**

| Input       | Pattern              | Output          |
|-------------|----------------------|-----------------|
| `FooBarBaz` | `R\|'Bar'`           | `["Foo"]`       |
| `FooBarBaz` | `R\|'Bar'{&S}R>`     | `["Foo","Baz"]` |
| `FooBarBaz` | `R\|['Qux'?'Bar']`   | `["Foo"]`       |
| `FooBarBaz` | `R+'Bar'`            | `["FooBar"]`    |
| `FooBarBaz` | `R+['Qux'?'Bar']`    | `["FooBar"]`    |
| `FooBarBaz` | `S\|'Bar'R>`         | `["BarBaz"]`    |
| `FooBarBaz` | `S\|'Bar'{&R}`       | `["Bar"]`       |
| `FooBarBaz` | `S\|['Qux'?'Bar']R>` | `["BarBaz"]`    |
| `FooBarBaz` | `S+'Bar'R>`          | `["Baz"]`       |
| `FooBarBaz` | `S+['Qux'?'Bar']R>`  | `["Baz"]`       |


### Operation Blocks ###

#### Default Operation Block ####

`(<operations>){<num>}` - executes a sequence of operations `<num>` time(s).

**Examples**

| Input       | Pattern       | Output                  |
|-------------|---------------|-------------------------|
| `FooBarBaz` | `(S[2]R.){3}` | `["o","r","z"]`         |
| `FooBarBaz` | `(R[3]){3}`   | `["Foo", "Bar", "Baz"]` |

**Shorthands**

The pattern in the last example could be written even shorter, without `()`:

```
R[3]{3}
```

This shorthand is **not** supported by the `S|` and `R|` operations (because it doesn't make sense for them).
However, if these operations have the `&{R}` or `&{S}` parameters respectively, the number of repetitions may
be appended to the very end of the operation, e.g.:

```
R|'foobar'{&S}{5}
```

**NOTE**

`{<num>}` can be omitted (the default value is `1`). This may be useful for improving readability, e.g.:

```
// A real-world example
(
    S+'^'{2}
    R|'^'{&S}{2}
    S>
)
(
    S+'^'{2}
    R|'^'{&S}{3}
    S+'^'
    R|'^'{&S}
    S+'^'{2}
    R|'^'
    S>
)*  
```

#### Continuous Operation Block ####

`(<operations>)*` - the sequence of operations is repeated until the end of the text.

**Examples**

| Input       | Pattern   | Output                                     |
|-------------|-----------|--------------------------------------------|
| `<file>`    | `(R>)*`   | Equivalent&nbsp;to&nbsp;`File.ReadLines()` |
| `FooBarBaz` | `(R[3])*` | `["Foo","Bar","Baz"]`                      |

❗ No pattern token (except for whitespace) can follow after `()*`.
In other words, this operation block (if present) must complete the pattern.

## ReaderOptions ##

This class is designed to configure various reading options. At the moment, it defines only one property:

```csharp
public sealed class ReaderOptions
{
    public TextComparison TextComparison { get; set; }
}
```

These options can be passed to one of the `TextReaderWrapper.ReadAsync()` overloads:

```csharp
public Task ReadAsync(string pattern, ICollection<string> output, ReaderOptions readerOptions, CancellationToken cancelToken)
```

### TextComparison ###

`enum` specifying the culture to be used by the operations that are based on [boundary strings](#readskip-up-to-boundary-string).

```csharp
public enum TextComparison
{
    IgnoreCulture, // default value
    CurrentCulture
}
```

| Boundary string | TextComparison   | .NET analogue                               |
|-----------------|------------------|---------------------------------------------|
| `'<str>'`       | `IgnoreCulture`  | `StringComparison.Ordinal` <sup>\*️⃣ </sup>  |
| `'<str>'`       | `CurrentCulture` | `StringComparison.CurrentCulture`           |
| `~<str>~`       | `IgnoreCulture`  | `StringComparison.OrdinalIgnoreCase`        |
| `~<str>~`       | `CurrentCulture` | `StringComparison.CurrentCultureIgnoreCase` |
| `/<rgx>/`       | `IgnoreCulture`  | `RegexOptions.CultureInvariant`             |
| `/<rgx>/`       | `CurrentCulture` | Default behavior of `Regex`                 |


\*️⃣  And what about `StringComparison.InvariantCulture`?
> *[Do not use](https://docs.microsoft.com/en-us/dotnet/standard/base-types/best-practices-strings)
> string operations based on `StringComparison.InvariantCulture` in most cases.*


## ExceptionHandlingOptions ##

`TextReaderWrapper` can handle exceptions in one of the following ways:

```csharp
// namespace: DevSpatium.IO.TextReader.ExceptionHandling
public enum OnException
{
    Throw,
    WrapAndThrow,
    StopReading
}
```

| OnException     | Reader behavior | 
|-----------------|-----------------|
| `Throw`         | No action is required. The original exception bubbles up to the calling code. |
| `WrapAndThrow`  | Throws custom `TextReaderException`, the `InnerException` property of which will contain the original exception, and additional details about the current state of the reader will be included in the `Message` property. <sup>\*️⃣ </sup> |
| `StopReading`   | Supresses the exception and stops reading. |

\*️⃣  For example, when `TextReaderWrapper` reaches the end of the text,
while not all the commands in the pattern are completed, it throws custom `EndOfTextException`.
By default, this exception is bound to the `OnException.WrapAndThrow` option.
So the final exception that will be thrown by the reader will look like this:

**`TextReaderException`**

```
InnerException: EndOfTextException
Message: Unexpected end of text. Operation: "R+'foobar'". Position in pattern: Line: 5, Column: 10. Position in source text: 27. 
```

An alternative option could be to just stop reading.

These options can be set for `EndOfTextException` as well as for any other exceptions as follows:

```csharp
using DevSpatium.IO.TextReader.ExceptionHandlers;
...
ExceptionHandlingOptions.Instance
    .On<EndOfTextException>(OnException.StopReading)
    .On<OperationCanceledException>(OnException.StopReading)
    .On<IOException>(OnException.WrapAndThrow)
    ...
```

❗ The more specific exceptions should be registered before less specific ones (as it were for `catch` clauses).

❗ The exception handling options **do not** take effect if the reading cannot even be started.
This may happen in one of the following scenarios:

* You passed an incorrect parameter to one of the `TextReaderWrapper` constructors.
* You passed an incorrect parameter to one of the `ReadAsync()` overloads.
* The `TextReaderWrapper` object has already been disposed.

## Customization ##

`TextReaderWrapper` expsoses an API for extending its internal operations and the pattern sytax.

### Abstract Syntax Tree ###

The pattern is parsed into an [Abstract Syntax Tree](https://en.wikipedia.org/wiki/Interpreter_pattern),
which is essentially a composite object that represents a recursive set of operations
(see [Composite Pattern](https://en.wikipedia.org/wiki/Composite_pattern)).

This tree can be built "manually". The `DevSpatium.IO.TextReader.Operations` namespace contains
all supported operations and their auxiliary types.

#### Operations ####

Every operation in the tree implements the following interface:

```csharp
public interface IOperation
{
    // `reader` - an object holding the instance of `System.IO.TextReader` passed to one of the
    // `TextReaderWrapper` constructors and providing some extended functionality (see the next section).
    // `output` - a collection to be filled with read strings.
    Task ReadAsync(ITextReader reader, ICollection<string> output, CancellationToken cancelToken);
}
```

The outermost (root) operation is `OperationTree`. All other operations should be added to an object of this class.

```csharp
var operationTree = new OperationTree().Add(<operation>).Add(<operation>)<...>;
using var textReaderWrapper = new TextReaderWrapper(<...>);
var output = new List<string>();
textReaderWrapper.ReadAsync(operationTree, output); // one of the `ReadAsync()` overloads
```

`OneCharOperation`

```csharp
// R.
new OneCharOperation(OperationType.Read)

// S.
new OneCharOperation(OperationType.Skip)
```

`CharBlockOperation`

```csharp
// R[<num>]
new CharBlockOperation(OperationType.Read, <num>)

// S[<num>]
new CharBlockOperation(OperationType.Skip, <num>)
```

`RemainingLineOperation`

```csharp
// R>
new RemainingLineOperation(OperationType.Read)

// S>
new RemainingLineOperation(OperationType.Skip)
```

`IBoundaryString`

```csharp
// '<str>'
new DefaultBoundaryString(str, ignoreCase: false, <TextComparision>)

// ~<str>~
new DefaultBoundaryString(str, ignoreCase: true, <TextComparision>)

// /<rgx>/
new RegexBoundaryString(rgx, <TextComparision>)
```

`IBoundaryStringSequence`

```csharp

// ['<str1>'? ~<str2>~? ... /<rgx>/]
new BoundaryStringSequence(<IBoundaryStrings>)
```

`BoundaryStringOperation`

```csharp
// R+[<...>]
new RemainingLineOperation(
    OperationType.Read,
    BoundaryStringOperationBehavior.WithOverstepping,
    <IBoundaryStringSequence>)

// R|[<...>]
new RemainingLineOperation(
    OperationType.Read,
    BoundaryStringOperationBehavior.NoOverstepping,
    <IBoundaryStringSequence>)

// R|[<...>]{&S}
new RemainingLineOperation(
    OperationType.Read,
    BoundaryStringOperationBehavior.WithOversteppingFromCounterpart,
    <IBoundaryStringSequence>)

// S+[<...>]
new RemainingLineOperation(
    OperationType.Skip,
    BoundaryStringOperationBehavior.WithOverstepping,
    <IBoundaryStringSequence>)

// S|[<...>]
new RemainingLineOperation(
    OperationType.Skip,
    BoundaryStringOperationBehavior.NoOverstepping,
    <IBoundaryStringSequence>)

// S|[<...>]{&R}
new RemainingLineOperation(
    OperationType.Skip,
    BoundaryStringOperationBehavior.WithOversteppingFromCounterpart,
    <IBoundaryStringSequence>)
```

`DefaultOperationBlock`

```csharp
// <operation>{<num>}
new DefaultOperationBlock(<operation>, <num>)

// (<operations>){<num>}
var operations = new CompositeOperation().Add(<operation>).Add(<operation>)<...>
new DefaultOperationBlock(operations, <num>)
```

`ContinuousOperationBlock`

```csharp
// (<operation>)*
new ContinuousOperationBlock(<operation>, <num>)

// (<operations>)*
var operations = new CompositeOperation().Add(<operation>).Add(<operation>)<...>
new ContinuousOperationBlock(operations)
```

#### ITextReader ####

This reader is used by operations that implement the `IOperation` interface.
It defines adapted versions of some of the  `System.IO.TextReader` members,
and also provides other extended functionality (like `PeekBlockAsync`).

```csharp
public interface ITextReader : IDisposable
{
    int Position { get; }
    bool EndOfText { get; }
    char? Peek();
    char? Read();
    Task<string> PeekBlockAsync(int count, CancellationToken cancelToken);
    Task<string> ReadBlockAsync(int count, CancellationToken cancelToken);
    Task<string> PeekLineAsync(CancellationToken cancelToken);
    Task<string> ReadLineAsync(CancellationToken cancelToken);
    Task<string> PeekToEndAsync(CancellationToken cancelToken);
    Task<string> ReadToEndAsync(CancellationToken cancelToken);
    Task<int> SetPositionAsync(int position);
}
```

**NOTE**

The class implementing this interface (`InternalTextReader`) represents an actual wrapper on top of `System.IO.TextReader`.
It  contains the instance of `System.IO.TextReader` that was passed to one of the `TextReaderWrapper` constructors:

```csharp
public TextReaderWrapper(System.IO.TextReader underlyingReader)
```

However, `TextReaderWrapper` has another constructor that can accept a custom implementation of `ITextReader`:

```csharp
// `leaveUnderlyingReaderOpen` - if set to `true`, the `underlyingReader` won't be disposed
//  after the current instance of `TextReaderWrapper` is disposed.
public TextReaderWrapper(ITextReader underlyingReader, bool leaveUnderlyingReaderOpen = false)
```

#### Example of Custom Operation ####

Suppose we want to create a new operation that will read all the remaining characters in the source text.
Below is an example of such an operation.
 
```csharp
public sealed class MyCustomOperation : BaseOperation
{
    public MyCustomOperation(OperationType operationType)
        : base(operationType)
    {
    }

    protected override async Task ReadInternalAsync(
        ITextReader reader,
        ICollection<string> output,
        CancellationToken cancelToken)
    {
        var result = await reader.ReadToEndAsync(cancelToken).ConfigureAwait(false);
        if (result == null)
        {
            throw new EndOfTextException();
        }

        if (OperationType == OperationType.Read)
        {
            output.Add(result);
        }
    }
}
```

### Extending Pattern Syntax ###

The pattern syntax can also be extended, although there are some limitations:

* Your pattern command should represent an operation derived from  the `BaseOperation` class.
* The command must start either with `R` or `S`.
* The next (*qualifying*) token **cannot** be one of the following: `[`, `.`, `>`, `|`, `+`.

In order to include the custom pattern command in the full parsing process,
you should implement the following interface residing in the `DevSpatium.IO.TextReader.Parsing` namespace:

```csharp
// See the section called "Example of Custom Pattern Command" below.
public interface ICustomOperationParser
{
    // Returns `true` if the `qualifyingToken` is a char representing the custom operation, otherwise: `false`.
    bool IsOperationSupported(char qualifyingToken);
    
    // Reads the qualifying token and any other tokens required to create an instance of the operation.
    // Parameters:
    // * `reader` - an object providing access to the pattern tokens (see the "IPatternReader" section below).
    // * `operationType` - if the preceding token is "R", the parameter will be set to `OperationType.Read`.
    //    Otherwise, if the preceding token is "S", the parameter will get value `OperationType.Skip`.
    BaseOperation Parse(IPatternReader reader, OperationType operationType);
}
```

...and register your implementation as follows:

```csharp
using DevSpatium.IO.TextReader.Parsing;
...
CustomPatternParsers.Instance.Register(myCustomOperationParser);
```

##### IPatternReader #####

The interface below defines members allowing all internal (as well as custom) parsers to read the pattern tokens.

```csharp
public interface IPatternReader : IDisposable
{
    // The current position of the pattern.
    PatternPosition Position { get; }

    // Returns `true` if the end of the pattern has already been reached, otherwise: `false`.
    bool EndOfPattern { get; }

    // Skips any whitespace and peeks the next token.
    // If the end of the pattern has already been reached, returns `null`.
    char? Peek();

    // Skips any whitespace and reads the next token.
    // If the end of the pattern has already been reached, throws `ParsingException`.
    char Read();

    // Peeks the token being directly next to the current position (does not skip whitespace).
    // If the end of the pattern has already been reached, returns `null`.
    char? PeekRaw();

    // Reads the token being directly next to the current position (does not skip whitespace).
    // If the end of the pattern has already been reached, throws `ParsingException`.
    char ReadRaw();
}

public static class PatternReaderExtensions
{
    // Skips any whitespace, reads the next token, and validates it using `validationFunc`.
    // If the validation fails, throws `ParsingException` containing details about the next token.
    public static char ReadAndAssert(this IPatternReader reader, Func<char, bool> validationFunc);

    // Skips any whitespace, reads the next token, and checks if the token is equal to `expectedToken`.
    // If the validation fails, throws `ParsingException` containing details about the failed token.
    public static char ReadAndAssert(this IPatternReader reader, char expectedToken);
    
    // Skips any whitespace, reads the next token and validates it using `validationFunc`.
    // Returns `true` if the validation succeeds, otherwise: `false`.
    public static bool CheckNextToken(this IPatternReader reader, Func<char, bool> validationFunc);

    // Skips any whitespace, reads the next token and checks if the token is equal to `expectedToken`.
    // Returns `true` if the validation succeeds, otherwise: `false`.
    public static bool CheckNextToken(this IPatternReader reader, char expectedToken);

    // Throws `ParsingException` containing details about the next token.
    public static Exception FailNextToken(this IPatternReader reader);
}
``` 

##### Example of Custom Pattern Command #####

Suppose our [custom operation](#example-of-custom-operation) is going to have the following syntax: `R*`.
Here is an example of the corresponding parsing implementation:

```csharp
public sealed class MyCustomOperationParser : ICustomOperationParser
{
    private const char SupportedQualifyingToken = '*';
    public bool IsOperationSupported(char qualifyingToken) => qualifyingToken == SupportedQualifyingToken;

    public BaseOperation Parse(IPatternReader reader, OperationType operationType)
    {
        reader.ReadAndAssert(SupportedQualifyingToken);
        return new MyCustomOperation(operationType);
    }
}
```

### Pattern Building ###

TBD
