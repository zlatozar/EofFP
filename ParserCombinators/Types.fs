module Types

//_____________________________________________________________________________
//                                                                     Grammar

type variable = Var of string

type expression =
    | Constant of int
    | Contents of variable
    | Minus of (expression * expression)
    | Greater of (expression * expression)
    | Times of (expression * expression)

type command =
    | Assign of (variable * expression)
    | Sequence of (command * command)
    | Conditional of (expression * command * command)
    | While of (expression * command)

//_____________________________________________________________________________
//                                                                       Lexer

type token =
    | Ident of string
    | Symbol of string
    | Number of int

//_____________________________________________________________________________
//                                                                   Interpret

type intvalue = Intval of int

type store = Store of (variable -> intvalue)
