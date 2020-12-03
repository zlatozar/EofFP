## F# notes

- Prefer **LazyList** over **Sequence**, because of pattern matching and automatic caching

## When to use lazy programming?

- Lazy evaluation is performant (and correct) only when correctly mixed with
  referential transparency. If the state of your variables is constantly
  changing, you can forget about memoization, and suddenly lazy evaluation
  becomes slower than doing it eagerly.

## What is monad?

Explaining "what is a monad" is a bit like saying "what is a number?" We use
numbers all the time. But imagine you met someone who didn't know anything about
numbers. How the heck would you explain what numbers are? And how would you even
begin to describe why that might be useful?

What is a monad? The short answer: **It's a specific way of chaining operations together.**

Monads are simply a way to wrapping things and provide methods to do
operations on the wrapped stuff without unwrapping it during chaining.

In essence, you're writing execution steps and linking them together with the
"bind function". (In Haskell, it's named `>>=`.) You can write the calls to the
bind operator yourself, or you can use syntax sugar which makes the compiler
insert those function calls for you. But either way, _each step is separated by a_
_call to this bind function._

So the bind function is like a _semicolon_; it separates the steps in a process.
**The bind function's job is to take the output from the previous step, and feed**
**it into the next step.**

That doesn't sound too hard, right? But there is more than one kind of monad. Why? How?

Well, the bind function can just take the result from one step, and feed it to
the next step. But if that's "all" the monad does... that actually isn't very
useful. And that's important to understand: **Every useful monad does something**
**else in addition to just being a monad**. Every useful monad has a "special power", which makes it unique.

(A monad that does nothing special is called the "identity monad".)

Basically, each monad has its own implementation of the bind function. And you
can write a bind function such that it does hoopy things between execution
steps. For example:

   - If each step returns a success/failure indicator, you can have bind execute
     the next step only if the previous one succeeded. In this way, a failing
     step aborts the whole sequence "automatically", without any conditional
     testing from you. (**The Failure Monad**.)

   - Extending this idea, you can implement "exceptions". (**The Error Monad** or
     **Exception Monad**.) Because you're defining them yourself rather than it
     being a language feature, you can define how they work. (E.g., maybe you
     want to ignore the first two exceptions and only abort when a third
     exception is thrown.)

   - You can make each step return multiple results, and have the bind function
     _loop_ over them, feeding each one into the next step for you. In this way,
     you don't have to keep writing loops all over the place when dealing with
     multiple results. The bind function "automatically" does all that for you.
     (**The List Monad**.)

   - As well as passing a "result" from one step to another, you can have the
     bind function pass extra data around as well. This data now doesn't show up
     in your source code, but you can still access it from anywhere, without
     having to manually pass it to every function. (**The Reader Monad**.)

   - You can make it so that the "extra data" can be replaced. This allows you
     to simulate destructive updates, without actually doing destructive
     updates. (**The State Monad** and its cousin the **Writer Monad**.)

   - Because you're only simulating destructive updates, you can trivially do
     things that would be impossible with real destructive updates. For example,
     you can undo the last update, or revert to an older version.

   - You can make a monad where calculations can be paused, so you can pause
     your program, go in and tinker with internal state data, and then resume
     it.

   - You can implement "continuations" as a monad. This allows you to break people's minds!

All of this and more is possible with monads. Of course, all of this is also
perfectly possible without monads too. It's just drastically easier using
monads.

What is a monad? The short answer: It's a specific way of chaining operations together.

Resources:
http://adit.io/posts/2013-04-17-functors,_applicatives,_and_monads_in_pictures.html

http://blog.sigfpe.com/2006/08/you-could-have-invented-monads-and.html

## Misc.

- Evaluation is the process of getting the root meaning of a piece of code.

- The reason so few compilers support lazy evaluation is that it makes writing
  an imperative, OOP-style compiler tricky.

- A lazy compiler can be much more efficient if it makes smart choices. For lazy
  evaluation to be efficient, it needs to use memoization. In other words, it
  needs to keep a dictionary where the key is the name of the variable, and the
  value is the result of the evaluation. When it encounters an already evaluated
  variable, it can simply look up the value in the dictionary.

- With lazy compilers, `if`s are not primitives (built into the compiler) but
  instead a part of the standard library of the language.
