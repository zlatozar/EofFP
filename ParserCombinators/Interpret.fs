module Interpret

open Base.Combinator
open Base.Common
open Base.List

open Types

/// All variables are init with 0
let store0 = Store (K (Intval 0))

/// Finds undeclared v
let nullstore = let f (Var str) = error "uninitialized variable: %s" str
                Store f

let fetch (v, Store f) = f v

let update (Store f, v, a) = Store (assoc [(v, a)] f)

let switch i T F store =
    match i, T, F with
    | (Intval 1), f, g -> f store
    | (Intval 0), f, g -> g store
    | (Intval n), f, g -> error "non boolean valued test"

let rec evalexp exp s =
    match exp with
    | (Constant n) -> Intval n
    | (Contents v) -> fetch (v, s)

    | (Minus (e1, e2))   -> let (Intval n1) = evalexp e1 s
                            let (Intval n2) = evalexp e2 s
                            Intval (n1 - n2)

    | (Times (e1, e2))   -> let (Intval n1) = evalexp e1 s
                            let (Intval n2) = evalexp e2 s
                            Intval (n1 * n2)

    | (Greater (e1, e2)) -> let (Intval n1) = evalexp e1 s
                            let (Intval n2) = evalexp e2 s
                            if n1 > n2 then
                                Intval (1) // true
                            else
                                Intval (0)

let rec interpret exp store =
    match exp with
    | Assign (v, e)           -> let a = evalexp e store
                                 update (store, v, a)

    | Sequence (c1, c2)       -> interpret c2 (interpret c1 store) // note the order
    | Conditional (e, c1, c2) -> switch (evalexp e store) (interpret c1) (interpret c2) store
    | While (e, c)            -> switch (evalexp e store) (interpret (While (e, c)) << interpret c) I store
