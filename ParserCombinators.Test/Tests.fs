module Tests

open FsUnit.Xunit
open Xunit

open Base.String
open Types

open Lexer

[<Fact>]
let ``Test lexer for keywords`` () =
    (lexanal << explode) "IF THEN ELSE" |> should equal [Symbol "IF"; Symbol "THEN"; Symbol "ELSE"]

open Combinators

[<Fact>]
let ``Test simple parser`` () =
    let exp = number |>> Constant
    let mk_assign_node ((v, _), e) = Assign (v, e)

    let assign = variable .>>. literal ":=" .>>. exp |>> mk_assign_node
    RUN assign [Ident "a"; Symbol ":="; Number 42] |> should equal (Success (Assign (Var "a", Constant 42),[]))

[<Fact>]
let ``Test parser on expression with brakets`` () =
    let unparenth ((left, num), rigth) = num
    let br = literal "(" .>>. number .>>. literal ")" |>> unparenth
    RUN br [Symbol "("; Number 42; Symbol ")"] |> should equal (Success (42,[]))

[<Fact>]
let ``Test MANY combinator`` () =
    let tokens = (lexanal << explode) "1 2 e 3"
    RUN (MANY number) tokens |> should equal (Success ([1; 2],[Ident "e"; Number 3]))

open Grammar

[<Fact>]
let ``Test WHILE statement`` () =
    let tokens = (lexanal << explode) "WHILE 4 > 3 DO a := 4 END"

    let assign = variable .>>. literal ":=" .>>. exp
    let pwhile = literal "WHILE" .>>. exp    .>>.
                 literal "DO"    .>>. assign .>>.
                 literal "END"

    RUN pwhile tokens |> should equal (Success
                                            ((((("WHILE", Greater (Constant 4, Constant 3)), "DO"),
                                                ((Var "a", ":="), Constant 4)), "END"),[]))

[<Fact>]
let ``Test IF ELSE statement`` () =
    let tokens = (lexanal << explode) "IF 5 > 4 THEN a := 4 ELSE a := 5 ENDIF"

    let assign = variable .>>. literal ":=" .>>. exp
    let pif = literal "IF"   .>>. exp     .>>.
              literal "THEN" .>>. assign .>>.
              literal "ELSE" .>>. assign .>>.
              literal "ENDIF"
    RUN pif tokens |> should equal (Success
                                        ((((((("IF", Greater (Constant 5, Constant 4)), "THEN"),
                                                ((Var "a", ":="), Constant 4)), "ELSE"),
                                                    ((Var "a", ":="), Constant 5)), "ENDIF"),[]))

[<Fact>]
let ``Test full parser`` () =
    let tokens = (lexanal << explode) "WHILE X > Y
                                       DO
                                         X := X - 1;
                                         Z := Z * Z;
                                         IF X > Z THEN A := X ELSE A := Z ENDIF
                                       END"
    RUN command tokens |> should equal (Success
                                            (While
                                                (Greater (Contents (Var "X"), Contents (Var "Y")),
                                                    Sequence
                                                        (Assign (Var "X", Minus (Contents (Var "X"), Constant 1)),
                                                            Sequence
                                                                (Assign (Var "Z", Times (Contents (Var "Z"), Contents (Var "Z"))),
                                                                    Conditional
                                                                        (Greater (Contents (Var "X"), Contents (Var "Z")),
                                                                        Assign (Var "A", Contents (Var "X")),
                                                                        Assign (Var "A", Contents (Var "Z")))))),[]))

open Parser

[<Fact>]
let ``Test reports`` () =
    let program = "WHILE X > Y
                   DO
                     X := X - 1;
                     Z := Z * Z;
                     IF X > Z THEN A := X ELSE A := Z ENDIF
                   END"
    parseFromString command program

open Interpret

[<Fact>]
let ``Test interpretation`` () =
    let program = "X := 1; IF X > Y THEN X := 42 ELSE X := 1 ENDIF"
    let pprogram = parseFromString command program
    let newStore = interpret pprogram store0
    fetch ((Var "X"), newStore) |> should equal (Intval 42)
