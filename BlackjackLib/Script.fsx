// Learn more about F# at http://fsharp.net. See the 'F# Tutorial' project
// for more guidance on F# programming.

//#load "Library1.fs"
//open BlackjackLib
open System

// Note that you're never going to reference an item in the list incorrectly.
// Many things about F# make it so you're not constantly checking for null.  In fact, you never do.
let rec someListFunc (list: string List) =
    match list with
    | hd::tl -> Console.WriteLine( "Current item: {0}", hd )
                if tl.IsEmpty then Console.WriteLine( "Last item in list" )
                someListFunc tl
    // Uncomment below: compiler knows how to deal with lists.
    //| last -> Console.WriteLine( "Last item in list: {0}", last )
    | [] -> Console.WriteLine( "We've run out of items" )

let list = [ "one"; "two"; "three" ]

someListFunc list
