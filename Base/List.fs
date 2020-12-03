module Base.List

open Base.Common
open Base.Combinator
open Base.Bool
open Base.Int

// The datatype of lists is the workhorse of functional programming.

/// Associative list implementation.
let assoc newassocs assocf arg =
    let rec search lst =
        match lst with
        | []           -> assocf arg
        | (k, v)::rest -> if arg = k then v else search rest
    search newassocs

let newAssoc newPairs =
    let emptyAssoc _ = error "no association found"
    assoc newPairs emptyAssoc

// just to be close to book's code
let map f x = List.map f x
let rev lst = List.rev lst

let fold f =
    let rec foldf a lst =
        match lst with
        | []   -> a
        | b::x -> foldf (f a b) x
    foldf

let accumulate = fold

// Revert list argument so revert and function arguments.
// See fold 'duality theorem': foldr f e xs <=> foldl (flip f) e (reverse xs)
let foldRight f a x =
    accumulate (C f) a (rev x)

let zip f =
    let rec zf lst1 lst2 =
        match lst1, lst2 with
        | a::x, b::y -> (f a b) :: (zf x y)
        | [], []     -> []
        |  _,  _     -> error "'zip' with different lengthed lists"
    zf

/// Same as 'zip' but if second element is not found
/// return first one.
let splice f =
    let rec sf lst1 lst2 =
        match lst1, lst2 with
        | a::x, b::y -> (f a b) :: (sf x y)
        | x, []      -> x
        | [], x      -> x
    sf

/// Use given predicate to filter list elements.
let filter p =
    let consifp x a =
        if p a then a::x else x
    accumulate consifp [] << rev

/// Check if given predicate has element in the list
/// that satisfy it. Breaks when first is found.
let exists p =
    let rec existsp lst =
        match lst with
        | []   -> false
        | a::x -> if p a then true
                  else existsp x
    existsp

/// Check if given predicate is satified for all elements
/// Express: non o exists o non
let forall p = non (exists (non p))

// 'member' is a keyword in F#
let mem x a = exists (equal a) x

let contains x y = forall (mem y) x

// 'null' is a keyword in F#
let nil = function
    | [] -> true
    | _  -> false

let hd = function
    | a::_ -> a
    | []   -> error "'hd' of [] is undefined"

let tl = function
    | _::x -> x
    | []   -> error "'tl' of [] is undefined"

// Note: One small advantage of the switch of arguments
//       is that some functions can be defined more succinctly;

/// Concatenate 'a' in front of list 'lst'. Prefer prefix notation.
let cons a lst = a::lst

/// Use when you need to 'cons' but parameters are reverted (aka 'snoc')
let consonto lst a = C cons lst a

// See 'fold map fusion'
let map' g = foldRight (cons << g) []

/// Append two lists. Prefer prefix notation.
let append x y = x @ y

/// In F# is imposible to make infix 'upto' name so use pseudo infix
/// e.g 1 |> upto <| 10. This is just to keep close to ML book code.
/// F# use ranges [1..10]
let rec upto n m = if n > m then []
                   else n::(n + 1 |> upto <| m)

/// Create a list as revert second list in front of first one (accumulated).
/// It is very useful in 'fold/accumulate' as initial parameter is [].
let revonto lst1 lst2 = accumulate (C cons) lst1 lst2

/// Revert given list
let reverse lst = revonto [] lst

/// Flatten nested lists e.g [[1; 2]; [3; 4]] -> [1; 2; 3; 4]
let link llst = rev (accumulate revonto [] llst)

// Tip: 'linkwith' is the working horse for list printing :)

/// Execute given fuction and the results are in following format
/// [ [<front>] [function result1] [<sep>]....[function resultN] [<back>] ]
let linkwith (front, sep, back) llst =
    let rec f lst =
        match lst with
        | []   -> [back]
        | [a]  -> [a; back]
        | a::x -> a::sep::(f x)
    link (front::(f llst))

/// Create a list with pairs using given lists with equal size.
let pairlists x y = zip pair x y

/// Create a list as copy n times given element x
let copy n x = repeat (cons x) n []

/// Return the sum of the integer list elements
let sumlist = accumulate plus 0

/// Return the multiplication of the integer list elements
let mullist = accumulate times 1

let maxlist = function
    | a::x -> accumulate max a x
    | []   -> error "'maxlist' of [] is undefined"

let maxposlist x = accumulate max 0 x

/// Create list as take first element from the netested lists.
/// Return [] when [] element exist.
let rec transpose = function
    | [] -> []
    | x  -> if exists nil x then []
            else (map hd x) :: transpose (map tl x)

let length lst = let count n _ = n + 1
                 accumulate count 0 lst

let drop n = repeat tl n

/// Create tuple (<list with n-th elements>, <list with rest elements>)
let split n =
    if n < 0 then error "negative subscript error ('split' failed)"
    else
        let rec shunt n x1 x2 =
            match n, x1, x2 with
            | 0, x1, x2      -> (rev x1, x2)
            | n, x1, (a::x2) -> shunt (n - 1) (a::x1) x2
            | _,  _,  _      -> error "list subscript error ('split' failed)"
        shunt n []

/// Create a list with the first n-th elements
let front n x = fst (split n x)

/// Create a list with the last n-th elements
let back n x = drop ((length x) - n) x

/// Point to the n-th element(counting from 1) from the given list.
let select n = hd << (drop (n - 1))

/// Select 'm' elements starting from n-th element(including).
let sublist n m x = front m (drop (n - 1) x)

/// Select element from list with a given index
/// e.g. [0; 1; 2]<!>1
let rec (<!>) lst n =
    match lst, n with
    | (x::_, n) when n = 0 -> x
    | (_::xs, n)           -> xs <!> n
    | ([], _)              -> error "out of bound"

/// Generates all permutations of a given list.
let rec perms = function
    | [] -> [[]]
    | x  -> [ for a in x do
                 for y in perms(x--a) do
                     yield a::y ]

and (--) lst elm =
    match lst, elm with
    | [], _       -> []
    | (b :: x), a -> if a = b then x
                     else b :: (x--a)

/// Partitions a list ([true], [false]) according to a given predicate
let partition pred lst =
    let rec loop l cont =
        match l with
        | []                -> cont ([], [])
        | x::xs when pred x -> loop xs (fun (ys, zs) -> cont (x::ys, zs))
        | x::xs             -> loop xs (fun (ys, zs) -> cont (ys, x::zs))
    loop lst I

let span pred lst =
    let rec loop l cont =
        match l with
        | []                -> cont ([], [])
        | x::xs when pred x -> loop xs (fun (ys, zs) -> cont (x::ys, zs))
        | x::xs             -> loop xs (fun (_, _) -> cont ([], x::xs))
    loop lst I

let mergeSorted less lst1 lst2 =
    let rec merge (l1, l2) =
        match (l1, l2) with
        | ([], x) | (x, []) -> x
        | (x::xs, y::ys)    -> if less x y then x::merge(xs, y::ys)
                               else y::merge(ys, x::xs)
    merge (lst1, lst2)

// First two are the first elements of the lists
let divide lst =
    let rec loop (p, q, r) =
        match (p, q, r) with
        | (p, q, [])         -> (p, q)
        | (p, q, [a])        -> (a::p, q)
        | (p, q, a::b::rest) -> loop (a::p, b::q, rest)
    loop ([], [], lst)
