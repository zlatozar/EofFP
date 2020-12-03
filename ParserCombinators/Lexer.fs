module Lexer

open Base.Common
open Base.String
open Base.Int
open Base.List

open Types
open Grammar

type position = {
    line: int
    column: int
 }

/// Define an initial position
let position0 = { line=0; column=0 }

/// Increment the column number
let incrCol (pos: position) =
    { pos with column=pos.column + 1 }

/// Increment the line number and set the column to 0
let incrLine pos =
    { line=pos.line + 1; column=0 }

/// Define the current input state
type inputstate = {
    position: position
    lines: string list
}

let keycheck s = if keyword s then
                     Symbol s
                 else
                     Ident s

let letdigetc a = if letter a then true
                  else
                      if digit a then true
                          else
                              mem ["`"; "_"] a

// symbols to skip
let layout = mem [" "; "\n"; "\t"]

// state machine see p.220
let rec lexanal explword =
    match explword with
    | []              -> []
    | a::rest as line -> if layout a then
                             lexanal rest
                         else
                             if a = "(" then
                                 (Symbol "(") :: lexanal rest
                             else
                                 if a = ")" then
                                     (Symbol ")") :: lexanal rest
                                 else
                                     if letter a then
                                         getword [a] rest
                                     else
                                         if digit a then
                                             getnum (intofdigit a) rest
                                         else
                                             if symbolchar a then
                                                 getsymbol [a] rest
                                             else
                                                 error (sprintf "Lexical Error: unrecognized token %s" (implode line))

and getword I restOf =
    match restOf with
    | []      -> [keycheck (implode (rev I))]

    | a::rest -> if letdigetc a then
                     getword (a::I) rest
                 else
                     keycheck (implode (rev I)) :: (lexanal (a::rest))

and getsymbol I restOf =
    match restOf with
    | []      -> [Symbol (implode (rev I))]
    | a::rest -> if symbolchar a then
                     getsymbol (a::I) rest
                 else Symbol (implode (rev I)) :: (lexanal (a::rest))

and getnum n restOf =
    match restOf with
    | []      -> [Number n]
    | a::rest -> if digit a then
                     getnum (n * 10 + intofdigit a) rest
                 else
                    (Number n) :: (lexanal (a::rest))
