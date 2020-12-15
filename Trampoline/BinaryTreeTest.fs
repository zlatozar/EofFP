module Test

    // The basic BinaryTree structure
    type BinaryTree<'node> =
        {
            Value : 'node
            Left  : BinaryTree<'node> option
            Right : BinaryTree<'node> option
        }
    with
        static member Apply(n, ?l, ?r) = { Value = n; Left = l; Right = r }
        member this.AddLeft(n)  = { this with Left  = (Some << BinaryTree<_>.Apply) n }
        member this.AddRight(n) = { this with Right = (Some << BinaryTree<_>.Apply) n }

    // In-Order Traversal
    let foldInOrder root seed consume =

        // The (stack-unsafe but elegant) recursive version
        let rec foldRecursive node accum =
            match node with
            | None -> accum
            | Some n ->
                foldRecursive n.Left accum
                |> (fun left -> consume left n.Value)
                |> (fun curr -> foldRecursive n.Right curr)

        // The (stack-safe and equally elegant) recursive version
        let rec foldTrampoline node accum =
            match node with
            | None -> Return accum
            | Some n ->
                Suspend (fun () -> foldTrampoline n.Left accum)
                >>= (fun left -> Return (consume left n.Value))
                >>= (fun curr -> Suspend (fun () -> foldTrampoline n.Right curr))

        foldTrampoline root seed
        |> execute