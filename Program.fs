// Learn more about F# at http://fsharp.org

open System 
open System.IO

//  junk function that handles resource releases the dotnet object way ... 
type LineChooser(fileName1, fileName2) = 
    let file1 = File.OpenText(fileName1) 
    let file2 = File.OpenText(fileName2)     
    let rnd = System.Random() 

    let mutable disposed = false 

    let cleanup() = 
        if not disposed then 
            disposed <- true; 
            file1.Dispose(); 
            file2.Dispose();             

    interface System.IDisposable with 
        member x.Dispose() = cleanup() 

    member obj.CloseAll() = cleanup()

    member obj.GetLine() = 
        if not file1.EndOfStream && 
            (file2.EndOfStream || rnd.Next() % 2 = 0) then file1.ReadLine() 
        elif not file2.EndOfStream then file2.ReadLine() 
        else raise (new EndOfStreamException()) 

//  extends the List module with pairwise 
module List = 
    let rec pairwise l = 
        match l with 
        | [] | [_] -> [] 
        | h1 :: ((h2 :: _) as t) -> (h1, h2) :: pairwise t 

//  val it : (int * int) list = [(1, 2); (2, 3); (3, 4)]

[<EntryPoint>]
let main argv =

//////////////////////////////////////////////////////////
// example data or function calls 

    printfn "pairwise example '%A'" (List.pairwise [1;2;3;4])  

    File.WriteAllLines("test1.txt", [|"Daisy, Daisy"; "Give me your hand oh do"|])  
    File.WriteAllLines("test2.txt", [|"I'm a little teapot"; "Short and Stout"|])  
    let chooser = new LineChooser ("test1.txt", "test2.txt") 
    chooser.GetLine() |> printfn "first GetLine example '%A'"
// val it : string = "I'm a little teapot"
    chooser.GetLine() |> printfn "second GetLine example '%A'"
// val it : string = "Short and Stout"
    (chooser :> IDisposable).Dispose() 
// val it : unit = ()
//    chooser.GetLine();; 
// System.ObjectDisposedException: Cannot read from a closed TextReader.

    printfn "All finished from ExpertF#Ch06" 
    0 // return an integer exit code
