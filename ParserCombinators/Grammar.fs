module Grammar

open Base.Common
open Base.List
open Types

open Combinators

let symbolchar = mem ["*"; "+"; "/"; "-"; ">"; ":"; "="; ";"]

let keyword = mem ["IF"; "THEN"; "ELSE"; "ENDIF"; "WHILE"; "DO"; "END"]

//     exp      ::= aexp [">" aexp]
//     aexp     ::= bexp ["-" aexp]
//     bexp     ::= cexp ["*" bexp]
//     cexp     ::= "(" exp ")"
//               | number
//               | variable

//     command  ::= unitcom [";" command]
//     unitcom  ::= whilecom
//               | ifcow
//               | assign
//     whilecom ::= "WHILE" exp "DO"   command "END"
//     ifcom    ::= "IF"    exp "THEN" command "ELSE" command "ENDIF'
//     assign   ::= variable ":=" exp

//_____________________________________________________________________________
//                                                         Expressions grammar

// '+', '/' are not defined in book
let rec exp  s = (aexp .>>. OPTIONAL (literal ">" .>>. aexp) |>> opt_compare) s
and aexp     s = (bexp .>>. OPTIONAL (literal "-" .>>. aexp) |>> opt_sub    ) s
and bexp     s = (cexp .>>. OPTIONAL (literal "*" .>>. bexp) |>> opt_mul    ) s
and cexp     s = (
                  (literal "(" .>>. exp .>>. literal ")"     |>> unparenth)
                   <|> (number                               |>> Constant )
                   <|> (variable                             |>> Contents ) ) s
//_____________________________________________________________________________
//

and unparenth ((lbr, e), rbr) = e
and opt_compare = function
        | (e1, [ ])       -> e1
        | (e1, [(_, e2)]) -> Greater (e1, e2)
        | _               -> error "impossible"
and opt_sub = function
        | (e1, [ ])       -> e1
        | (e1, [(_, e2)]) -> Minus (e1, e2)
        | _               -> error "impossible"
and opt_mul = function
        | (e1, [ ])       -> e1
        | (e1, [(_, e2)]) -> Times (e1, e2)
        | _               -> error "impossible";

//_____________________________________________________________________________
//                                                            Commands grammar

let rec command s = (unitcom .>>. OPTIONAL (literal ";" .>>. command)
                                                       |>> opt_seq        ) s

and unitcom     s = (whilecom
                     <|> ifcom
                     <|> assign                                           ) s

and whilecom    s = (literal "WHILE" .>>. exp     .>>.
                     literal "DO"    .>>. command .>>.
                     literal "END"                     |>> mk_while_node  ) s

and ifcom       s = (literal "IF"   .>>. exp     .>>.
                     literal "THEN" .>>. command .>>.
                     literal "ELSE" .>>. command .>>.
                     literal "ENDIF"                   |>> mk_if_node     ) s

and assign      s = (variable .>>. literal ":=" .>>. exp
                                                       |>> mk_assign_node ) s

//_____________________________________________________________________________
//

and opt_seq = function
    | (d, [ ])             -> d
    | (d, [(semicol, c2)]) -> Sequence (d, c2)
    | _                    -> error "impossible"

and mk_while_node ((((w, ex), d), com), e)         = While (ex, com)

and mk_if_node ((((((i, ex), t), c1), el), c2), e) = Conditional (ex, c1, c2)

and mk_assign_node ((v, _), e)                     = Assign (v, e)
