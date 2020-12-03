module MaybeIOMonad

open IOMonad2

type MaybeIO<'a> = IO<Option<'a>>

let runMaybe (x: MaybeIO<'a>): 'a option = run x

[<RequireQualifiedAccess>]
module MaybeIOMonad =
    let unit x: MaybeIO<'a> = IOMonad.unit (Some x)
    let bind f io = IOMonad.bind (function
                                  | Some x -> f x
                                  | None   -> IOMonad.unit(None)) io
    let map f io = bind (f >> unit) io
    let lift io = IOMonad.map Some io

type MaybeIOBuilder() =
    member __.Bind(x, f) = MaybeIOMonad.bind f x
    member __.Return(x) = MaybeIOMonad.unit x
    member __.ReturnFrom(e): MaybeIO<_> = e

let maybeIo = MaybeIOBuilder()

//____________________________________________________________
//                                               Simple tests

// let readLine = IOMonad.wrap (fun () -> stdin.ReadLine())
// let print x = IOMonad.wrap (fun () -> printfn "%A" x)

// let tryParse (str: string) =
//     match System.Int32.TryParse(str) with
//     | (true, int) -> Some(int)
//     | _           -> None

// let readInt = readLine |> IOMonad.map tryParse
// let maybePrint = print >> MaybeIOMonad.lift

// let bar = maybeIo {
//     let! x = readInt
//     let! y = io { return 20 } |> MaybeIOMonad.lift

//     let result = x + y
//     do! maybePrint result

//     return result
// }

// bar |> runMaybe |> ignore
