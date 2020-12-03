module CharPics

open Base.String
open Base.List

// Prototype as proof of concept

type picture =
    Pic of int * int * string list

/// This can be used to form very simple atomic pictures.
/// The argument should be a list of the picture lines.
let mkPic piclist =
    let d = length piclist
    let shape = map size piclist
    let w = maxposlist shape

    let addSpaces line =
        let a = size line
        if a < w then
            concat line (spaces (w - a))
        else
            line

    // add spaces to align to longest line
    let checkedLines = map addSpaces piclist
    Pic(d, w, checkedLines)

/// Returns the number of lines
let depth (Pic(d, _, _)) = d
/// Returns the length of the longest line
let width (Pic(_, w, _)) = w
/// Lines of the picture.
/// Returns the picture lines themselves (used in defining some other operations)
let linesof (Pic(_, _, sl)) = sl

let nullpic = Pic(0, 0, [])

/// padside n p forms a picture of AT LEAST width n using
/// spaces to pad from the right when necessary.
let padside n (Pic(d, w, sl) as pic) =
    if n <= w then pic
    else Pic(d, n, map (fun s -> concat s (spaces (n - w))) sl)

/// padbottom n p forms a picture of AT LEAST depth n using
/// spaces to pad when necessary.
let padbottom n (Pic(d, w, sl) as pic) =
    if n <= d then pic
    else Pic(n, w, sl @ copy (n - d) (spaces w))

/// Similar to row (see previous chapter), but a triple
/// of strings must be supplied to be duplicated on the left,
/// between pictures and on the right respectively ((f)ront(s)epartor(b)ottom).
/// Used to form a table rows.
let rowwith fsb piclist =
    let d' = maxposlist (map depth piclist)
    let blocks = map (linesof << padbottom d') piclist

    let mkline n = stringwith fsb (map (select n) blocks)
    let sl' = map mkline (1 |> upto <| d')
    let w' = if nil sl' then 0
             else size (hd sl') // length - 1

    Pic(d', w', sl')

let row = rowwith ("", "", "")

/// Similar to column, but a triple of strings must be supplied
/// (characters) to be duplicated along the top, between pictures
/// and along the bottom, respectively.
let colwith (f, s, b) piclist =
    let w' = maxposlist (map width piclist)

    let flines = map (implode << (copy w')) (explode f)
    let slines = map (implode << (copy w')) (explode s)
    let blines = map (implode << (copy w')) (explode b)

    let sl' = linkwith (flines, slines, blines) (map (linesof << padside w') piclist)
    let d' = length sl'

    Pic(d', w', sl')

let column = colwith ("", "", "")

/// Move to the right. indent n p add spaces on the left.
let indent n (Pic(d, w, sl) as pic) =
    if n < 1 then pic
    else Pic(d, w + n, map (concat (spaces n)) sl)

/// Move to the bottom. lower n p adds spaces at the top of p
let lower n (Pic(d, w, sl) as pic) =
    if n < 1 then pic
    else Pic(d + n, w, copy n (spaces w) @ sl)

/// This forms a table when supplied with a list of the rows of the
/// table. Each row should be a list of pictures.
let table pics =
    match pics with
    | []          -> nullpic
    | piclistlist -> // makes sure each list has same length
                     let mkrect piclistlist =
                         let sizerows = map length piclistlist
                         let maxrow = maxposlist sizerows

                         let addnulls len piclist = if len < maxrow then
                                                        piclist @ (copy (maxrow - len) nullpic)
                                                    else
                                                        piclist

                         zip addnulls sizerows piclistlist

                     let newpics = mkrect piclistlist

                     let picwidths = map (map width) newpics

                     let colwidths = map maxposlist (transpose picwidths)

                     let picrowlists = map (zip padside colwidths) newpics

                     let  tablerows = map (rowwith ("|", "|", "|")) picrowlists

                     let dashes n = implode (copy n "-")

                     let sep = stringwith ("+", "+", "+") (map dashes colwidths)
                     let sl' = linkwith ([sep], [sep], [sep]) (map linesof tablerows)
                     let d' = length sl'
                     let w' = size (hd sl')

                     Pic(d', w', sl')

let frame picture = table [ [picture] ]

let header s pic = colwith ("", "~", "") [mkPic [s]; pic]

/// This can be used to look at a picture. (It is actually
/// the identity operation with a side effect printing the
/// picture. Consequently it should only be used at the top level.
let showpic picture = show (stringwith ("", "\n", "\n") (linesof picture))
                      picture

/// paste n m p1 p2 places p2 ontop of p1 at the point after
/// n characters down and m characters along. It is robust in that
/// it works for negative n and m and when p1 is too small.
let rec paste n m pic1 pic2 = (* n,m may be negative, pic2 goes over *)
                              (* pic1 at n rows down and m chars in  *)
    if n < 0 then
        paste 0 m (lower (-n) pic1) pic2
    else
        if m < 0 then
            paste n 0 (indent (-m) pic1) pic2
        else
            // this picture will be on bottom
            let pic1' = padbottom (n + depth pic2) (padside (m + width pic2) pic1)

            // Counts n elements before start f (overlay) execution
            let rec spliceat n f x y = if n < 1 then
                                           splice f x y
                                       else
                                           hd x::spliceat (n - 1) f (tl x) y

            // Covers the first character comming from the first line by
            // charecter 'b' from second line.
            let overlay _ b = b

            let stringop line line' = implode (spliceat m overlay
                                                            (explode line)
                                                            (explode line'))

            let sl' = spliceat n stringop (linesof pic1') (linesof pic2)
            let w' = if nil sl' then 0 else size (hd sl')
            let d' = length sl'

            Pic(d', w', sl')

/// cutfrom p n m d w produces a picture of depth d and width w
/// cut from p starting at the point after n characters down and
/// m characters along. (None of the integers are required to be positive.
let rec cutfrom pic n m a b = (* n,m,a,b may be negative, a picture of size a deep and b wide *)
                              (* is cut from pic starting at n rows down and m chars in       *)
    if n < 0 then
        cutfrom (lower (-n) pic) 0 m a b
    else
        if m < 0 then
            cutfrom (indent (-m) pic) n 0 a b
        else
              if a < 0 then
                  cutfrom pic (n + a) m (-a) b
              else
                  if b < 0 then
                      cutfrom pic n (m + b) a (-b)
                  else
                      let pic' = padbottom (n + a) (padside (m + b) pic)
                      let edit str = implode (sublist (m + 1) b (explode str))
                      let newsl = map edit (sublist (n + 1) a (linesof pic'))

                      Pic(a, b, newsl)

/// Crop from
let tilepic d w tile =
    let dt = depth tile
    let wt = width tile
    let ndeep = (d + dt - 1) / dt
    let nacross = (w + wt - 1) / wt

    let col = column (copy ndeep tile)
    let excess = row (copy nacross col)
    cutfrom excess 0 0 d w

let mkrect listlist defaults =
    let shape = map length listlist
    let maxrow = maxposlist shape

    let extend len list =
        if len < maxrow then
            list @ (copy (maxrow - len) defaults)
        else list

    zip extend shape listlist

// Examples:

// let p0 = frame (mkPic ["this is"; "a"; "sample picture"])
// let p1 = paste 3 5 p0 p0
// let p2 = row [p1; p1; frame p1]
// let alpha = mkPic ["ABCD"; "EFGH"; "IJKL"]
// let p3 = tilepic 8 23 alpha
// let p5 = table [[paste 4 5 p3 (frame p3); p3];
//                 [alpha; alpha; alpha]]
// showpic p5
