## Character Pictures

 We have two tasks to perform in implementing pictures. One is to decide on a suitable
basic collection of operations which allow us to construct pictures relatively easily. The
other is to decide upon a convenient representation for pictures using the types of values
we have introduced so far (integers, booleans, strings, tuples, lists, functions) which
will allow us to implement the required operations. **Tokanize picture lines in a list**
**is the key point.**

 Usually, the former task should precede the latter since we can only make a good choice
of representation when we know what is to be implemented.  However, we cannot easily judge
what we want implemented until we have had a chance to experiment and find which
operations are more useful. Such experimenting requires an implementation
experiments. This chicken and egg situation can be resolved by building **prototypes**
with the expectation that we may well change our minds about the collection of operations
and the representations as we proceed and experiment.

