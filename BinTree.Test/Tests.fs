module Tests

open FsUnit.Xunit
open Xunit

open BinTree
open Base.Int

let bt1 = Leaf 6
let bt2 = Leaf 2 ^^ Leaf 8
let bt3 = bt1 ^^ (bt2 ^^ bt2)
let bt4 = Leaf true ^^ Leaf false
let bt5 = Leaf "def" ^^ (Leaf "xyz" ^^ Leaf "c")

[<Fact>]
let ``Test 'btreemap' function`` () =
    btreemap (plus 1) (Leaf 5 ^^ (Leaf 2 ^^ Leaf 8))
        |> should equal (Leaf 6 ^^ (Leaf 3 ^^ Leaf 9))
