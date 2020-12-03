module IOMonad2

type IO<'a> =
    private
    | Return of (unit -> 'a)
    | Suspend of (unit -> IO<'a>)

let rec run x =
    match x with
    | Return v  -> v()
    | Suspend s -> s() |> run

[<RequireQualifiedAccess>]
module IOMonad =
    let unit x = Return (fun () -> x)
    let bind f io = Suspend (fun () -> f(io |> run))
    let map f io = bind (f >> unit) io
    let wrap r = Return r

type IOBuilder() =
    member __.Bind(x, f) = IOMonad.bind f x
    member __.Return(x) = IOMonad.unit x
    member __.ReturnFrom(io): IO<_> = io

let io = IOBuilder()

//____________________________________________________________
//                                               Simple tests

// let readLine = IOMonad.wrap (fun () -> stdin.ReadLine())
// let print x = IOMonad.wrap (fun () -> printfn "%A" x)

// let foo = io {
//     let! cs1 = readLine
//     let! cs2 = readLine

//     let x = cs1 + cs2
//     do! print x

//     return x
// }

// io {
//     let! x = foo |> IOMonad.map (fun (s:string) -> s.ToUpper())
//     do! print x

//     return ()
// } |> run
