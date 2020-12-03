module Base.Common

let error s = failwith (sprintf "\n     ERROR REPORT:%s\n" s)

let equal a b =
    a = b
