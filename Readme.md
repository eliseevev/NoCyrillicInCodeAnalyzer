# NoCyrillicInCodeAnalyzer

This project provides Roslyn analyzers and code fixes to prevent the use of Cyrillic characters in C# code identifiers.  
It helps maintain code readability and consistency, especially in multilingual teams.

## Rules

| Rule ID  | Description                                 | Documentation                |
|----------|---------------------------------------------|------------------------------|
| NC0001   | No Cyrillic characters in type names        | [NC0001](documentation/rules/NC0001.md) |
| NC0002   | No Cyrillic characters in namespace names   | [NC0002](documentation/rules/NC0002.md) |
| NC0003   | No Cyrillic characters in method names      | [NC0003](documentation/rules/NC0003.md) |
| NC0004   | No Cyrillic characters in field names       | [NC0004](documentation/rules/NC0004.md) |
| NC0005   | No Cyrillic characters in property names    | [NC0005](documentation/rules/NC0005.md) |
| NC0006   | No Cyrillic characters in local variable names | [NC0006](documentation/rules/NC0006.md) |
| NC0007   | No Cyrillic characters in parameter names   | [NC0007](documentation/rules/NC0007.md) |
| NC0008   | No Cyrillic characters in type parameter names | [NC0008](documentation/rules/NC0008.md) |
| NC0009   | No Cyrillic characters in enum member names | [NC0009](documentation/rules/NC0009.md) |
| NC0010   | No Cyrillic characters in event names       | [NC0010](documentation/rules/NC0010.md) |

## Usage

Add this analyzer to your project or as a Visual Studio extension.  
When you use Cyrillic characters in identifiers, you will receive a warning and a code fix suggestion.

## License

MIT
