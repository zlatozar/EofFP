## Trampoline

The `Trampoline` itself is very simple - there are really only two cases to work with:

- We have the terminal case - in this case, we return a value of type `Return`
- A tail call position function call - in this case we suspend the function and return a value of type `Suspend`
- The `Flatten` type is used to compose computations, as well as when we try to execute the Trampoline value.

As you’d expect, the `Return` and `Suspend` are handled in a straightforward way.
The `Flatten` value is used to chain computed values to their continuations monadically.

```fsharp

let execute (head: 'a Trampoline) :'a =
    let rec execute' = function
        | Return v -> v
        | Suspend f ->
                f ()
                |> execute'
        | Flatten b ->
            match b.m with
            | Return v ->
                b.f v
                |> execute'
            | Suspend f ->
                Flatten {| m = f (); f = b.f |}
                |> execute'
            | Flatten f ->
                let fm = f.m
                let ff a = Flatten {| m = f.f a ; f= b.f |}
                Flatten {| m = fm;  f = ff |}
                |> execute'
        execute' head
```

### Trampoline example

```fsharp
module Fact =
    open System.Numerics
    let fact n =
        let rec fact' n accum =
            if n = 0 then
                Return accum
            else
                Suspend (fun () -> fact' (n-1) (accum * (BigInteger n)))

        if (n < 0) then invalidArg "n" "should be > 0"

        fact' n BigInteger.One
        |> execute
```
