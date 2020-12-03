### Base operations notes

- `revonto` is very confinient to use with accumulators e.g.

```fsharp
let collect f list =
    let rec accumf sofar lst =
        match lst with
        | []   -> sofar
        | a::x -> accumf (revonto sofar (f a)) x
    accumf [] list
```

or with `fold` e.g.

```fsharp
let link llst = rev (fold revonto [] llst)
```