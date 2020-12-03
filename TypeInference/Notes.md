## Hindley-Milner

The stages are:

1. Assign symbolic type names (like `t1`, `t2`, ...) to all sub-expressions.

2. Using the language's typing rules, write a list of type equations (or constraints)
   in terms of these type names.

   The real trick of type inference is running these typing rules _in reverse_.
   The rule tells us how to assign types to the whole expression given its
   constituent types, but we can also use it as an equation that works both ways
   and lets us infer constituent types from the whole expression's type.

3. Solve the list of type equations using **unification**.
