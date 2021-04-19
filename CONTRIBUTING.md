# Coding style

Please ensure that any code you add to this repository satifies the coding style, the root of this repo contains a 
[editor config](https://github.com/beto-rodriguez/LiveCharts2/blob/master/.editorconfig) file that will warn you in 
visual studio if you are violating the coding style.

The coding style of this repository is based on [dot net runtime coding style](https://github.com/dotnet/runtime/blob/main/docs/coding-guidelines/coding-style.md) 
but there are a few exceptions:

~~10. We only use var when the type is explicitly named on the right-hand side, typically due to either new or an explicit cast, e.g. 
`var stream = new FileStream(...)` not `var stream = OpenStandardInput()`.~~

10.Feel free to use `var` anywhere.

18.When using a single-statement if, we follow these conventions:

* ~~Never use single-line form (for example: if (source == null) throw new ArgumentNullException("source");)~~

* Please use single line `if` expressions, if the line is too long please create a new line everytime the width of the line could exceeds the editor window size, for example, the following line is valid: 
```
if (model == null)
    throw new Exception(
        $"A {nameof(ObservablePoint)} can not be null, instead set to null to " +
        $"any of its properties.");
```


* Using braces is always accepted, and required if any block of an if/else if/.../else compound statement uses braces ~~or if a single statement body spans multiple lines.~~ (just brake the lines as in the previous sample)

* Braces may be omitted only if the body of every block associated with an if/else if/.../else compound statement is placed on a single line.

Early `return`, `continue` and `break` should be used when possible.

# Naming files

The documentation is generated autmatically, it is important that all the files are named the same as the object name.

for the following class, the file must be named `Hello.cs`

```
public class Hello
{
  public string World { get; set; }
}
```

When using generics just ignore them, for example, the following class must also be named `Hello.cs` 

```
public class Hello<T>
{
  public T World { get; set; }
}
```

If You have generic and a no generic definition that share the same name, please place both classes in the same file, this should only 
happen when both objects are related to each other by inheritance.

The following `Hello.cs` must contain both, inheritance is required.

```
public class Hello
{
  public string World { get; set; }
}

public class Hello<T> : Hello
{
  public T World2 { get; set; }
}
```
