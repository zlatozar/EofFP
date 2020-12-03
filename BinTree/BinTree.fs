module BinTree

open Base.String
open Base.List
open CharPics

// Tip: We have now seen enough examples to get the general idea: when introducing
//      a new datatype, also define the generic 'fold' operation for that datatype. When
//      the datatype is parameterised, also introduce the appropriate 'mapping' operation.
//      Given these functions, a number of other useful functions can be quickly defined.

type 'a BinTree =
    | Leaf of 'a
    | Node of 'a BinTree * 'a BinTree
with
    static member (^^) ((x: 'a BinTree), (y: 'a BinTree)) =
        Node(x, y)

// same as ^^
let private join ((x: 'a BinTree), (y: 'a BinTree)) =
    Node(x, y)

let btreeop f g =
    let rec btfg = function
        | Leaf x        -> f x
        | Node (t1, t2) -> g (btfg t1, btfg t2)
    btfg

let btreemap f = btreeop (Leaf << f) join

let btreepic leafpicfun =
    let joinpics ((p, n1), (p', n1')) =
        let n = width p
        let dashn = (n - n1 + n1')

        let dashn2 = dashn / 2
        let dashn1 = dashn - dashn2 - 1

        let newn1 = n1 + dashn1 + 1
        let line1 = implode (copy newn1 " " @ ["|"])
        let line2 = implode (copy n1 " " @ ["."] @
                             copy dashn1 "-" @ ["^"] @
                             copy dashn2 "-" @ ["."])

        let arms = mkPic [line1; line2]
        let newpic = column [arms; rowwith ("", " ", "") [p; p']]

        (newpic, newn1)

    let doleaf x =
        let p = leafpicfun x
        let n = width p
        let n1 = n - n/2 - 1;

        let arm = mkPic [implode (copy n1 " " @ ["|"])]
        (column [arm; p],  n1)

    let picof t =
        btreeop doleaf joinpics t |> fst

    picof

let intpic (n: int) = mkPic [implode ["("; stringofint n; ")"]];

// let tree1 = ((( (Leaf 3 ^^ Leaf 4) ^^ (Leaf 5 ^^ (Leaf 6 ^^ Leaf 7)) ) ^^ Leaf 8) ^^ Leaf 9)
// let tree2 = tree1 ^^ tree1

// btreepic intpic tree1
//     |> showpic |> ignore
