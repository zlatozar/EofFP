## Some useful F# tricks

- Define functions to select from tuple

```fsharp
let depth (Pic(d, _, _)) = d
let width (Pic(_, w, _)) = w
let linesof (Pic(_, _, sl)) = sl
```

- Represent 2D objects as list and use `map` to manipulate them.

- Overlay

First, however, we need to decide how the overlaying can be done. Roughly speaking, it
will involve taking an appropriate row from each picture and splicing the characters of
one row into an appropriate place in the other row, displacing any characters already
there.

### Modules and namesapace

- Use `namesapace` for logical dividing
  (they can be used to organize modules and types to avoid name collisions)

Namespace naming [convention](https://docs.microsoft.com/en-us/dotnet/standard/design-guidelines/names-of-namespaces)

- Modules are close to _static classes_

A module is just a set of functions that are grouped together, typically because
they work on the same data type or types. Just like static classes, modules can
contain child modules nested within them. Access control keywords are `public`,
`private`, and `internal`. Everything is **public** by default.

_Namespaces can directly contain type declarations, but not function declarations._

There are two common patterns for mixing types and functions together:
    - having the type declared separately from the functions
    - having the type declared in the same module as the functions (more common in F#)
