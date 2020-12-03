module GameOfLife

open Base.String
open Base.List

open CharPics

type generation =
    GEN of (int * int) list

/// Sort and remove pair repetitions by a slight
/// adaptation of the quicksort algorithm.
let rec lexordset = function
    | []   -> []
    | a::x -> lexordset (filter (lexless a) x) @ [a] @ lexordset (filter (lexgreater a) x)

and lexless (a1: int, b1: int) (a2, b2) =
    a2 < a1 || (a2 = a1 && b2 < b1)

and lexgreater pr1 pr2 =
    lexless pr2 pr1

/// Collect function 'f' results in reverce order.
/// In this way (survivors @ newborn) is more effective for lexordset.
let collect f list =
    let rec accumf sofar lst =
        match lst with
        | []   -> sofar
        | a::x -> accumf (revonto sofar (f a)) x
    accumf [] list

/// Finds elements which occur exactly 3 times in coordinates list
let occurs3 (coordList: (int * int) list) =
    let rec sieve moreTreeTimes x3 x2 x1 = function
        | []      -> diff x3 moreTreeTimes
        | a::rest -> if mem moreTreeTimes a then
                         sieve moreTreeTimes x3 x2 x1 rest
                     else
                         if mem x3 a then
                             sieve (a::moreTreeTimes) x3 x2 x1 rest
                         else
                             if mem x2 a then
                                 sieve moreTreeTimes (a::x3) x2 x1 rest
                             else
                                 if mem x1 a then
                                     sieve moreTreeTimes x3 (a::x2) x1 rest
                                 else
                                     sieve moreTreeTimes x3 x2 (a::x1) rest

    and
        // return list of elements that are not part of y
        diff x y = filter (not << mem y) x
    sieve [] [] [] [] coordList

/// Produce the list of coordinates of live squares of a generation
/// in lexical order (with no repetitions).
let rec alive (GEN livecoords) = livecoords

    and mkgen coordlist = GEN(lexordset coordlist)

    // Used to produce a 'nextgeneration' function of type generation -> generation.
    // It should be supplied with an argument function which calculates the neighbours of a coordinate.
    and mk_nextgen_fn neighbours gen =
        let living = alive gen
        let isalive = mem living
        let liveneighbours = length << filter isalive << neighbours

        let twoorthree n =
            n = 2 || n = 3

        let survivors = filter (twoorthree << liveneighbours) living
        let newnbrlist = collect (filter (not << isalive) << neighbours) living
        let newborn = occurs3 newnbrlist

        mkgen (survivors @ newborn)

/// Calculate neighbours
let neighbours (i, j) = [(i - 1, j - 1); (i - 1, j); (i - 1, j + 1);
                         (i, j - 1); (i, j + 1); (i + 1, j - 1);
                         (i + 1, j); (i + 1, j + 1)]

let private xstart = 0
let private ystart = 0

let visulize offset str = sprintf "%s%sx" str (spaces offset)

let rec plotfrom curPos (line: string) next =
    match curPos, line, next with
    | (x, y), str, (x1, y1)::rest -> if x = x1 then
                                         // same line so extend str and continue from (y1 + 1)
                                         plotfrom (x, y1 + 1) (visulize (y1 - y) str) rest
                                     else
                                        // flush current line and start a new line
                                        str :: plotfrom (x + 1, ystart) "" ((x1, y1)::rest)
    | (_, _), str, []             -> [str]

let visible (x, y) =
    x >= xstart && y >= ystart

let plot genlist =
    plotfrom (xstart, ystart) "" (filter visible genlist)

let (<&>) coordlist (x: int, y: int) =
    map (fun (a, b) -> (a + x, b + y)) coordlist

let rotate =
    map (fun (x: int, y: int) -> (y, -x))

let rec nthgen gen nth =
    match nth with
    | 0 -> gen
    | i -> nthgen (mk_nextgen_fn neighbours gen) (i - 1)

//_____________________________________________________________________________
//                                                                 Experiments

let glider = [(0, 0); (0, 2); (1, 1); (1, 2); (2, 1)]

let bail = [(0, 0); (0, 1); (1, 0); (1, 1)]

let barberpole n =
    let rec loop i =
        if i = n then
            (n + n - 1, n + n)::[(n + n, n + n)]
        else
            (i + i, i + i + 1)::(i + i + 2, i + i + 1)::(loop (i + 1))
    (0, 0)::(1, 0)::(loop 0)

let genB = mkgen ((glider <&> (2, 2))
                  @ (bail <&> (2, 12))
                  @ (rotate (barberpole 4) <&> (5, 20)))

let gun = mkgen [(2, 20); (3, 19); (3, 21); (4, 18); (4, 22); (4, 23); (4, 32); (5, 7);
                 (5, 8);  (5, 18); (5, 22); (5, 23); (5, 29); (5, 30); (5, 31); (5,32);
                 (5, 36); (6, 7);  (6, 8);  (6, 18); (6, 22); (6, 23); (6, 28); (6, 29);
                 (6, 30); (6, 31); (6, 36); (7, 19); (7, 21); (7, 28); (7, 31); (7,40);
                 (7, 41); (8, 20); (8, 28); (8, 29); (8, 30); (8, 31); (8, 40); (8, 41);
                 (9, 29); (9, 30); (9, 31); (9, 32)]

let print =
    let printpic picture =
        show (stringwith ("", "\n", "\n") (linesof picture))
    printpic << frame << mkPic << plot << alive

// Show result after 50 generations
// print (nthgen gun 50)

let doit times step thing =
    let rec loop n gen =
        match n with
        | 0     -> ()
        | count -> print gen
                   let nextThing = nthgen gen step
                   loop (count - 1) nextThing
    loop times thing

// doit 150 1 gun
