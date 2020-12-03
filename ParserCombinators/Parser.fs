module Parser

open Base.Common
open Base.String
open Base.List

open Types
open Lexer
open Combinators

/// Make tokens readable
let lit = function
    | Symbol s -> s
    | Number n -> stringofint n
    | Ident s  -> s

let report = function
    | Failure _        -> error "Parse Error"
    | Success (c, [ ]) -> c
    | Success (_, x)   -> error (stringwith (
                                     "Syntax Error\nUnparsed:-\n", " ", "\n") (map lit x))

let parseFromString parser (program: string) =
    program
      |> explode
      |> lexanal
      |> RUN parser
      |> report
