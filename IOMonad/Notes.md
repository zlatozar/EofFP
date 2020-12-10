## Monads in a low level languages

The data structure have to represent nicely the state of the program and to
handle the ability of the program to continue. That's why we have to start from
them. The date structure stores program context so logic is around them. In a
high level languages we do not bother about memory structure - everything is
virtual, but what about if we want to have monads in C? We have invent generic
structure(kind of optimized stack) that should handle all kind of monads and to
implement API there all common operations. They creation is bottom-up.
My be [someone](https://github.com/koka-lang/libhandler) did it already?

**All data structures are based on array (memory cells).**

## Catalog

Monad Name  | Description
---------   | --------------
`Maybe`     | Computations which may not return a result
`Error`     | Computations which can fail or throw exceptions
`List`      | Non-deterministic computations which can return multiple results
`IO`        | Computations which perform I/O
`State`     | Compute in a given state and prepare it for the next in chain
`Reader`    | Computations which depends from value(env.) we don't know yet
`Writer`    | Computations which write data in addition to computing values
`Continuation` | Computations which can be interrupted and restarted
               | (captures the core of how asynchronous workflows work)
               |

**Continuations are the heart of monads implementation!**

```fsharp
let bind n cont =
    match n with
    | None   -> None
    | Some r -> cont r
```
Notice how we pass the continuation function(`cont` parameter) in and call it with the result (`r`).

## Bind and Return notes

`let x = m in n`

To compute this first compute `m`(and perform its side effects) then bind `x` to the
value yielded then compute `n` (and perform its side effects). The key difference is
that `>>=` preserves equational reasoning(referential transparency) while the F# `let`
with side effects does not.
Because of this similarity one may wish to introduce a variant `let!` expression
such as `let x <- m in n` (`m >>= fun (x) -> n`)

`>>` is a special case of `>>=`: `m >> n` <=> `m >>= fun () -> n`

## Monad laws

`let x=v in m` <=> `m[x:=v]` <br/>
`let x=m in x` <=> `m`       <br/>
`let y=(let x=m in n) in o` <=> `let x=m in (let y=n in o)` <br/>

**Monad is what take care of side effects automatically when you compose functions.**

## Example

How to convert sorting to monadic function.

```fsharp
// Not tail recursive
let someSort list =
    let rec loop list =
        match list with
        | [] -> []
        | [x] -> [x]
        | x::xs ->
            let l, r = List.partition ((>) x) xs
            let ls = loop l
            let rs = loop r
            ls @ (x::rs)
    loop list
```

```fsharp
// Converted to tail recursive using CPS
let someSortCont list =
    let rec loop list cont =
        match list with
        | [] -> cont []
        | x::[] -> cont [x]
        | x::xs ->
            let l, r = List.partition ((>) x) xs
            loop l (fun ls ->
            loop r (fun rs ->
            cont (ls @ (x::rs))))
    loop list (fun x -> x)
```

```fsharp
// More readable and faster using monads

type ContinuationBuilder() =
    member this.Bind (m, f) = fun c -> m (fun a -> f a c)
    member this.Return x = fun k -> k x

let cont = ContinuationBuilder()
```

```fsharp
let someSortMonad list =
    let rec loop list =
        cont {
            match list with
            | [] -> return []
            | x::xs ->
                let l, r = List.partition ((>) x) xs
                let! ls = loop l
                let! rs = loop r
                return (ls @ (x::rs))
        }
    loop list (fun x -> x)
```

Quick sort example:

```fsharp
let quickSortCont list =
    let rec loop list acc cont =
        match list with
        | [] -> cont acc
        | x::[] -> cont (x::acc)
        | x::xs ->
            let l, r = List.partition ((>) x) xs
            loop r acc (fun rs ->
            loop l (x :: rs) cont)
    loop list [] (fun x -> x)

let quickSortMonad list =
    let rec loop list acc=
        cont {
            match list with
                | [] -> return acc
                | x::[] -> return x::acc
                | x::xs ->
                    let l, r = List.partition ((>) x) xs
                    let! rs = loop r acc
                    let! s = loop l (x::rs)
                    return s
            }
    loop list [] (fun x -> x)
```
