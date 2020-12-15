[<AutoOpen>]
module Trampoline

    type Trampoline<'a> =
        | Return of 'a
        | Suspend of (unit -> 'a Trampoline)
        | Flatten of {| m : 'a Trampoline; f : 'a -> 'a Trampoline |}
        with
            static member (<|>) ((this : 'a Trampoline), (f : 'a -> 'a)) =
                Flatten {| m = this; f = (f >> Return) |}

            static member (>>=) ((this : 'a Trampoline), (f : 'a -> 'a Trampoline)) =
                Flatten {| m = this; f = f |}

    let execute (head : 'a Trampoline) : 'a =
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
