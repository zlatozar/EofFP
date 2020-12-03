module Tests

open FsUnit.Xunit
open Xunit

open Lazy

[<Fact>]
let ``Check elements are equal`` () =
    [lazy (0 + 1); lazy (0 + 2); lazy (0 + 3)] <=> [lazy (0 + 1); lazy (0 + 3); lazy (0 + 3)] |> should be False

[<Fact>]
let ``Check if leafs are equal`` () =
    eqleaves (Leaf (lazy (3 + 3)) ^^ (Leaf (lazy (1 + 2)) ^^ Leaf (lazy (6 + 3))))
             (Leaf (lazy (2 + 3)) ^^ (Leaf (lazy (1 + 2)) ^^ Leaf (lazy (6 + 3)))) |> should be False

open LList

[<Fact>]
let ``Lazy check if element exist`` () =
    [1; 2; 42; 3; 4; 5]
    |> Seq.ofList
    |> exists (fun x -> x = 42)
    |> should be True

[<Fact>]
let ``Check lazy list appending`` () =
    seq [1; 2] <%> (seq [4; 5])
    |> Seq.toList
    |> should equal [1; 2; 4; 5]

open Hamming

[<Fact>]
let ``Check Hamming numbers algorithm`` () =
    Seq.item 30 hamming |> should equal 81

open LDict
open FSharpx.Collections

[<Fact>]
let ``Check processing`` () =
    let requests = LazyList.ofList [Lookup 3; Entry (1, "x"); Entry (3, "z"); Lookup 1;
                    Lookup 2; Entry (2, "y")]
    dictprocess requests |> LazyList.toList |> should equal ["z"; "x"; "y"]

let ``Ordered tree shoudl be builded`` () =
    builddict (LazyList.ofList [(1, "x"); (3, "z"); (2, "y")])
    |> should equal (Item (1, "x", Empty, Item (3, "z", Item (2, "y", Empty, Empty), Empty)))
