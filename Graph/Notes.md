## Abstract Data Type (ADT)

An abstract datatype is defined not by naming its values, but by naming its
operations.

Firstly, the programmer wants to know what primitive operations are given.
The list of operations, together with their types, is called the signature of the
datatype (use `.fsi` files).

Secondly, the programmer needs to know what these operations mean. The
names and types give some clues, as does an informal description, but more
precise information is needed. Users of the datatype need to know what they
can rely on, and the implementor of the datatype needs to know what has to
be provided.

### Algebraic specifications

One way of supplying the information is to give what is called an algebraic or
axiomatic specification.

### Representation

In order to implement an abstract type, a programmer has to provide a
representation of its values, define the operations of the type in terms of this
representation, and show that the implemented operations satisfy the
prescribed specification. Apart from these obligations, the implementor is free
to choose between different representations on the grounds of efficiency,
simplicity, or taste. In the following section we will describe mechanisms
provided by Haskell for hiding the implementation of an abstract type so that
reference to the chosen representation is not permitted elsewhere in the script.

Other things being equal, the best implementation of an abstract datatype is
one that makes the operations as efficient as possible. In practice, though,
there is usually a trade-off: one operation can be made more efficient only at
the expense of another operation. The crafting of an implementation requires
judgement as to where best to put one's effort in obtaining efficiency. There
may also be requirements attached to the specification of the datatype, in that
one operation or another may be required to possess a certain efficiency.
