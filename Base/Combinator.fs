module Base.Combinator

open Base.Common

/// Identity. Could be treated same as [] for lists.
let I x = x

/// Ignore second
let K x _ = x

/// Lifted application
let S f g x = f x (g x)

/// Flip parameters. (aka 'flip')
let C f x y = f y x

/// Double parameters
let W f x = f x x

/// For a function, f, define Y(f) as the "fixed point" of f:
/// A value, z, such that f(z) = z.
///
/// Substituting Y(f) for z, gives f(Y(f)) = Y(f), which
/// we flip to Y(f) = f(Y(f)).
///
/// This fixed point, z, is itself a function that takes an
/// argument, x. We have to make x explicit in the definition
/// in order to avoid infinite recursion when fix is called.
///
/// (In a typed language like F#, fix has to be defined recursively.
/// However, in an untyped system, such as the pure lambda calculus,
/// fix can be defined without recursion. That version of fix is
/// called the Y-combinator.)
let rec Y f =
    let z x = (f (Y f)) x
    z

let curry f a b = f(a, b)

let uncurry f (a, b) = f a b

let pair a b = (a, b)

// Tip: Glue together thing that can't be compose.
//      In other words 'f' and 'g' are from same domain but have different ranges.
let couple f g x = (f x, g x)

/// FOR times DO <function> params => repeat <function> times params
let repeat f =
    let rec rptf n x = if n = 0 then x
                       else rptf (n - 1) (f x)

    let check n = if n < 0 then error "'repeat' < 0"
                  else n
    rptf << check
