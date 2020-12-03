module Base.Int

let succ x = x + 1

// Sometimes it is useful to use prefix notation
// for algebraic functions

let plus (a: int) b =
    a + b

let times (a: int) b =
    a * b

let less (a: int) b =
    b < a

let lesseq (a: int) b =
    b <= a

let greater (a: int) b =
    b > a

let greatereq (a:int) b =
    b >= a

let max (a: int) b =
    if a < b then b else a

let min (a: int) b =
    if a < b then a else b

let private codeofO = (int << char) "0"
let private codeof9 = codeofO + 9

let digit (d: string) =
    let check c =
        int c >= codeofO && int c <= codeof9
    check (char d)
