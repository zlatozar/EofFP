module Graph

open Base.List

// Tip: You can receive gadget or you could receive parts and instructions
//      how to build it.

// Graph is a movement from one node to another guided by given function
type graph<'a> = Graph of ('a -> 'a list)

let depthSearch (Graph succ) p startnode =
    let rec find visited nodes =
        match nodes with
        | [ ]  -> None
        | a::x -> if mem visited a then find visited x
                  else
                      if p a then Some a
                      else find (a :: visited) (succ a @ x)
    find [ ] [startnode]

let breathSearch (Graph succ) p startnode =
    let rec find visited nodes =
        match nodes with
        | [ ]  -> None
        | a::x -> if mem visited a then find visited x
                  else
                      if p a then Some a
                      else find (a :: visited) (x @ succ a) // !!
    find [ ] [startnode]
