## Definitions

### Abstraction

Software solutions should be decomposable into their component parts, and
recomposable into new solutions, without changing the internal component
implementation details.

Abstraction in software takes many forms:

- Algorithms
- Data structures
- Modules
- Classes
- Frameworks

When you create abstractions, you should be deliberate about it, and you should
be aware of the good abstractions that have already been made available to you
(such as the always useful `map`, `filter`, and `reduce`). Learn to recognize
characteristics of good abstractions:

- Simple
- Concise
- Reusable
- Independent
- Decomposable
- Recomposable

### Closure

A closure is a function bundled with its lexical scope. Closures are created at
runtime during function creation.

### Category Theory

A lot of functional programming terms come from category theory, and the essence
of category theory is **composition**. Like jumping off a diving board or riding a
roller coaster. Here’s the foundation of category theory in a few bullet points:

- A category is a collection of objects and arrows between objects (where “object” can mean literally anything).

- Arrows are known as morphisms. Morphisms can be thought of and represented in code as functions.

- For any group of connected objects, `a -> b -> c`, there must be a composition which goes directly from `a -> c`.

- All arrows can be represented as compositions (even if it’s just a composition with the object’s identity arrow).
  All objects in a category have identity arrows. _Composition is associative_.
  Basically that means that when you’re composing multiple functions (morphisms if you’re feeling fancy),
  you don’t need parenthesis.

### Composition and Curried function

Composition is the ability to compose simple functions to form more complex functions.

**A curried function is a function that takes multiple arguments one at a time.**
Given a function with 3 parameters, the curried version will take one
argument and return a function that takes the next argument, which returns a
function that takes the third argument. The last function returns the result of
applying the function to all of its arguments. Curried functions are particularly
useful in the context of function composition. The returned function is just
a **specialized**(some params are set) version of the more general function.

A partial application is a function which has been applied to **some** (as many
or as few arguments a time as desired), but not yet all of its arguments. In
other words, it’s a function which has some arguments fixed inside its closure
scope. A function with some of its parameters fixed is said to be _partially
applied._ All **curried** functions return partial applications, but not all
partial applications are the result of curried functions.

### Point free style

It is very common for functional programmers to write functions as a composition
of other functions, never mentioning the actual arguments they will be applied
to. For example, compare: `let sum = List.fold (+) 0` with `let sum' xs = List.fold (+) 0 xs`.

These functions perform the same operation, however, the former is more compact,
and is considered cleaner. This is closely related to function pipelines (and to
unix shell scripting): it is clearer to write `let fn = f << g << h` than to write
`let fn x = f (g (h x))`.

### Functor

A functor data type is something you can _map over_. It’s a container which has an
interface which can be used to apply a function to the values inside it. When
you see a functor, you should think “mappable”. Functor types are typically
represented as an object with a `map` method that maps from inputs to outputs
while **preserving** structure. In practice, “preserving structure” means that the
return value is the same type of functor (though values inside the container may
be a different type).

A functor supplies a box with zero or more things inside, and a mapping
interface. An array is a good example of a functor, but many other kinds of
objects can be mapped over as well, including promises, streams, trees, objects,
etc. For collections (arrays, streams, etc.), `map` typically iterates over the
collection and applies the given function to each value in the collection, but
not all functors iterate. Functors are really about applying a function in a
specific context.

An _endofunctor_ is a functor that maps from a category back to the same category.
A monad is just a monoid in the category of endofunctors.

**Functor Lows**

- Identity

If you pass the identity function (`x -> x`) into `f.map`, where `f` is any functor,
the result should be equivalent to (have the same meaning as) `f`.

- Composition

`E` stands for effect.

Functors must obey the composition law: `E.map(x -> f(g(x)))` is equivalent to `E.map(g).map(f)`.

### Monads

_“Once you understand monads, you immediately become incapable of explaining them to anyone else”_
Douglas Crockford

Flatten means unwrap the value from the context. `E(a) -> a`

A monad is a way of composing functions that require context in addition to the
return value, such as computation, branching, or I/O. Monads type _lift_, _flatten_
and _map_ so that the types line up for lifting functions `a -> M(b)`, making them
composable. It’s a mapping from some type `a` to some type `b` along with some
computational context, hidden in the implementation details of `lift`, `flatten`,
and `map`:

- Functions map: `a -> b`
Map means, "apply a function to an `a` and return a `b`". Given some input, return some output.
_Type lift_ means to lift a type into a context.

- Functors map with context: `Functor(a) -> Functor(b)`
Context is the computational detail of the monad’s composition. The point of
functors and monads is to abstract that context away(unwrap) so we don’t have to
worry about it while we’re composing things. Mapping inside the context means
that you apply a function from `a -> b` to the value inside the context, and
return a new value `b` wrapped(wrap again) inside the same kind of context.

- Monads _flatten_ and _map_ with context: `Monad(Monad(a)) -> Monad(b)`
