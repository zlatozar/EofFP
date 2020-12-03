module Base.Bool

/// Invert given predicute result
let non p = not << p

/// Execute second predicate with given parameter 'x'
/// only if first predicate fails
let ou p q x = p x || q x

/// Return true only both predicates return true with
/// given parameter 'x'
let et p q x = p x && q x
