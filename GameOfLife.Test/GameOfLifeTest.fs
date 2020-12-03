module GameOfLife.Test

open FsUnit.Xunit
open Xunit

[<Fact>]
let ``Test 'collect' function`` () =
    let wrap elm = [elm + 1]
    collect wrap [1; 2; 3] |> should equal [4; 3; 2]

[<Fact>]
let ``Test 'occure3' function`` () =
    occurs3 [(2, 2)] |> should be Empty
    occurs3 [(2, 2); (2, 2)] |> should be Empty
    occurs3 [(2, 2); (2, 2); (2, 2)] |> should equal [2, 2]
    occurs3 [(2, 2); (2, 2); (2, 2); (2, 2)] |> should be Empty
