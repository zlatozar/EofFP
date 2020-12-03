module Tests

open FsUnit.Xunit
open Xunit

open Eager.Stream

[<Fact>]
let ``Test streams`` () =
    front 5 nat |> should equal [1; 2; 3; 4; 5]
