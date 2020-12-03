namespace Eager

module Stream =

    open Base.Common

    type Stream<'a> =
        | MkStream of (unit -> 'a * 'a Stream)
        | EndStream

    let sCons a s = let f = fun () -> (a, s)
                    MkStream f

    let sHead = function
        | MkStream f -> let (a, _) = f()
                        a
        | EndStream  -> error "head of EndStream"

    let sTail = function
        | MkStream f -> let (_, s) = f()
                        s
        | EndStream  -> error "tail of EndStream"

    let sEnd = function
        | EndStream  -> true
        | MkStream _ -> false

    //________________________________________________________
    //

    let rec from n = let f = fun () -> (n, from (n + 1))
                     MkStream f

    let nat = from 1

    // first N from stream
    let rec nForce n strm =
        match n, strm with
        | 0, s            -> ([ ], s)
        | _, EndStream    -> error "nForce of EndStream"
        | n, (MkStream f) -> let (a, s1) = f()
                             let (x, s2) = nForce (n - 1) s1
                             (a::x, s2)

    let front n s = fst (nForce n s)

    let drop n s = snd (nForce n s)

    let rec sList (step, isend) a0 =
        if isend a0 then EndStream
        else
            let f = fun () -> let (a1, b1) = step a0
                              (b1, (* next *) sList (step, isend) a1)
            MkStream f

    let fromUntil p a = let step n = (n, n + 1)
                        sList (step, p) a
