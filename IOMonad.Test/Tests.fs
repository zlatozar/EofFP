module Tests

open FsUnit.Xunit
open Xunit

open IOMonad

[<Fact>]
let ``Test print string`` () =
    let action = printStr "Hello world"
    run action |> should equal () // print is side-effect

let echoDup =
    readChar    >>= fun c ->
    printChar c >>
    unit (printChar c)

let makePrompt =
    printStr ">>>"             >>
    readStr                    >>= fun line ->
    printStr "The line is :\n" >>
    printStr line              >>
    unit (String.length line)

// run (mapM_ printStr ["a"; "b"; "c"; "d"] )
// run (replicateM 4 readStr)
