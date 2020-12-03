## Game Rules

-  A dead cell comes to life if it has 3 adjacent cells (or "neighbours") that are alive, through reproduction.
-  A live cell with more than 3 live neighbours dies due to overcompetition.
-  A live cell with fewer than 2 live neighbours dies due to loneliness.


## Life, the Universe and SAT Solvers

We call Life state **A** the "parent" of state **B** if **A** turns into **B** by
following the rules of Life. The reason that it's difficult to find the parent of a state
is that the rules of Life are non-reversible. There's no direct way to go from a Life
state to its parent, and in fact, it's possible for a state to have multiple parents or
even no parents.

What we can do is construct a boolean equation that captures the conditions that any
parent state of our target state must satisfy, then solve it to find a parent, if a parent
exists.

(Note: a boolean equation is an equation where the variables take on true/false values,
and where the operators, instead of the pluses and minuses that we usually see, are
replaced by boolean operators such as `AND` and `OR`. For example, the equation `sour AND
(NOT sweet)` is solved by setting `sour:=true` and `sweet:=false`.