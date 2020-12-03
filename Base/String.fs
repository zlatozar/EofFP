module Base.String

open Base.List

let concat s1 s2 = sprintf "%s%s" s1 s2

/// Converts a list of strings into a string
let implode = accumulate concat ""

/// Converts a string into a list of strings
let explode (s: string) =
    [for c in s -> string c]

/// Print given string
let show s = printf "%s" s

let stringwith (front, sep, back) sl =
    let rec f rest = match rest with
                     | []   -> [back]
                     | [a]  -> [a; back]
                     | a::x -> a :: sep :: (f x)
    implode (front::f sl)

let spaces n = implode (copy n " ")

let newlines n = implode (copy n "\n")

let stringofint n = sprintf "%i" n

// Prefer prefix notation

let sless (s: string) s' =
    s' < s

let slesseq (s: string) s' =
    s' <= s

let size s = String.length s

let uppercase (s: string) =
    let check c =
        int c >= int 'A' && int c <= int 'Z'
    check (char s)

let lowercase s =
    let check c =
        int c >= int 'a' && int c <= int 'z'
    check (char s)

let letter s = (lowercase s) || (uppercase s)

let intofdigit (d: string) = int d - int "0"
