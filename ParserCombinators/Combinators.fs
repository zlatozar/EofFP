module Combinators

open Base.List
open Types

type ParserResult<'a> =
    | Success of 'a * list<token>
    | Failure

type Parser<'a> =
    list<token> -> ParserResult<'a>

/// Bind operator. Applies f to the result of parser p.
let (>>=) (p: Parser<'a>) (f: 'a -> Parser<'b>) :Parser<'b> =
    fun stream ->
        match p stream with
        | Success(x, rest) -> (f x) rest
        | Failure          -> Failure

/// Always returns a Success with x as result
let preturn x :Parser<'a> =
    fun stream -> Success(x, stream)

/// Always fails.
let pzero :Parser<'a> =
    fun _ -> Failure

/// Computation expression builder.
type ParserBuilder() =
    member __.Bind(p, f) = p >>= f
    member __.Return(y)  = preturn y

let parse = ParserBuilder()

/// Set the result of a parser to the specified value x.
let (>>%) p1 x :Parser<'b> =
    p1 >>= (fun _ -> preturn x)

/// Applies p1 and p2 returning the result of p2.
let (>>.) p1 p2 :Parser<'b> =
    p1 >>= (fun _ -> p2)

/// Applies p1 and p2 returning the result of p1.
/// Example: manyChars digit .>> skipChar ',' .>>. ...
let (.>>) p1 p2 :Parser<'a> =
    p1 >>= (fun x -> p2 >>% x)

/// Applies the parsers popen, p and pclose in sequence. It returns the result of p.
let BETWEEN popen pclose p =
    popen >>. p .>> pclose

/// Applies p1 and p2 returning as a result a tuple with both results.
let (.>>.) p1 p2 :Parser<'a * 'b> =
    p1 >>= (fun x -> p2 >>= (fun y -> preturn (x, y)))

/// Applies the first parser and if it fails, applies the second one.
let (<|>) (p1: Parser<'a>) (p2: Parser<'a>) :Parser<'a> =
    let p stream =
        match p1 stream with
        | Failure -> p2 stream
        | res     -> res
    in p

/// If p is successful applies function f to the result of a parser.
let (|>>) p f :Parser<'b> =
    // p >>= (fun x -> preturn (f x))
    p >>= (preturn << f)

let EMPTY s =
    Success ([ ], s)

/// Many, one or nothing
let OPTIONAL p =
    p |>> (consonto [ ]) <|> preturn []

/// Runs p as many times as posible(end or fail), returning a list
/// with the results.
let rec MANY p :Parser<list<'a>> =
    parse {
        let! x = p
        let! xs = (MANY p)
        return x :: xs
    } <|> preturn []

/// Runs p as many times as possible with at least one Succcess.
let MANY_1 p :Parser<list<'a>> =
    parse {
        let! x = p
        let! xs = (MANY p)
        return x :: xs
    }

/// Looks for a 'front' followed by a list of 'p's separated by 'sep's
/// and ending with a 'back'. Only the items returned by pr are kept in the result.
let SEQ_WITH (front, sep, back) p =
    let sep_p = sep .>>. p          |>> snd
    let items = p   .>>. MANY sep_p |>> cons

    front .>>. OPTIONAL items .>>. back |>> (link << fst << snd)

/// Runs p n times.
let rec TIMES n p :Parser<list<'a>> =
    parse {
        if n <= 0 then return []
        else
            let! x = p
            let! xs = (TIMES (n - 1) p)
            return x::xs
    }

/// Returns the first successful result of the given parser sequence.
let CHOICE ps :Parser<'a> =
    // Seq.reduce (fun (p1: Parser<'a>) (p2: Parser<'a>) -> p1 <|> p2) ps
    Seq.reduce (<|>) ps

//_____________________________________________________________________________
//

/// Runs the given parser against the given string.
let RUN (p: Parser<'a>) s =
    p (Seq.toList s)

//_____________________________________________________________________________
//                                                               Basic parsers

let number = function
    | (Number n)::s -> Success(n, s)
    | _             -> Failure

let variable = function
    | (Ident x)::s -> Success(Var x, s)
    | _            -> Failure

let literal a syms =
    match syms with
    | (Symbol x)::s -> if a = x then Success (x, s) else Failure
    | _             -> Failure
