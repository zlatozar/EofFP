module Base.Test

open System

open NUnit.Framework

open FsUnit.Xunit
open Xunit

[<Fact>]
let ``Test 'equal'`` () =
    Common.equal "a" "a" |> should be True
    Common.equal 1 1 |> should be True

open Base.Combinator

[<Fact>]
let ``Test combinators`` () =
    repeat (String.concat "zl") 3 "" |> should equal "zlzlzl"

    let double n = n + n
    repeat double 3 1 |> should equal 8

open Base.Bool

[<Fact>]
let ``Test 'non' function`` () =
    let positive n = n > 0
    non positive -1 |> should be True
    non positive 1 |> should be False

let positive n = n > 0
let even n =
    n % 2 = 0
let throwException n = n/0 > 0

[<Fact>]
let ``Test 'ou' function`` () =
    ou positive throwException 1 |> should be True

[<Fact>]
let ``Test 'et' function`` () =
    et positive even 2 |> should be True
    et positive even 3 |> should be False
    et positive even -1 |> should be False

open Base.List

[<Fact>]
let ``Test 'assoc' function`` () =
    let example1 = newAssoc [(1, "a"); (2, "b"); (3, "c")]
    let example2 = assoc [(2, "e"); (4, "xxx")] example1

    example2 3 |> should equal "c"
    example2 4 |> should equal "xxx" // replaced

[<Fact>]
let ``Test 'splice' function`` () =
    splice (+) [1; 2] [4; 5; 6] |> should equal [5; 7; 6]

[<Fact>]
let ``Test 'exists' function`` () =
    exists positive [1; 2; -3] |> should be True
    exists positive [-1; -2; -3] |> should be False

[<Fact>]
let ``Test 'forall' function`` () =
    forall positive [1; 2; -3] |> should be False
    forall positive [1; 2; 3] |> should be True

[<Fact>]
let ``Test 'upto' function. Pseudo infix`` () =
    1 |> upto <| 10 |> should equal [1..10]

[<Fact>]
let ``Test 'revonto' function`` () =
    revonto [1; 2] [3; 4] |> should equal [4; 3; 1; 2]

[<Fact>]
let ``Test 'linkwith' function`` () =
    linkwith (["start"], [","], ["end"]) (map (fun el -> append el ["z"]) [["--"]; ["--"]])
        |> should equal ["start"; "--"; "z"; ","; "--"; "z"; "end"]

[<Fact>]
let ``Test 'transpose' function`` () =
    // one row
    transpose [ [1]; [2; 3]; [4; 5; 6] ] |> should equal [[1; 2; 4]]
    // tree rows
    transpose [ [1; 2; 3]; [4; 5; 6]; [7; 8; 9] ] |> should equal [[1; 4; 7]; [2; 5; 8]; [3; 6; 9]]
    // if there is shortest
    transpose [ [1; 2; 3]; [4; 5; 6]; [7; 8; 9]; [10] ] |> should equal [[1; 4; 7; 10]]
    // if exist empty
    transpose [ [1]; [2; 3]; [4; 5; 6]; [] ] |> should be Empty

[<Fact>]
let ``Test 'split' function`` () =
    split 1 [1; 2; 3; 4] |> should equal ([1], [2; 3; 4])
    split 2 [1; 2; 3; 4] |> should equal ([1; 2], [3; 4])
    split 3 [1; 2; 3; 4] |> should equal ([1; 2; 3], [4])
    split 4 [1; 2; 3; 4] |> snd |> should be Empty // ([1; 2; 3; 4], [])

[<Fact>]
let ``Test 'sublist' function`` () =
    sublist 2 3 [1; 2; 3; 5] |> should equal [2; 3; 5]

[<Fact>]
let ``Test 'partition' function`` () =
    partition (fun x -> x > 0) [-1; -2; 1; 2]
    |> should equal ([1; 2], [-1; -2])

[<Fact>]
let ``Test 'span' function`` () =
    span (fun x -> x < 0) [-1; -2; 1; 2]
    |> should equal ([-1; -2], [1; 2])

[<Fact>]
let ``Test 'mergeSorted' function`` () =
    mergeSorted (<) [1; 2; 3; 4] [6; 7; 8]
    |> should equal [1; 2; 3; 4; 6; 7; 8]

[<Fact>]
let ``Test 'divide' function`` () =
    divide [1; 2; 3; 4; 5]
    |> should equal ([5; 3; 1], [4; 2])
