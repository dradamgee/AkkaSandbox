open System
open AkkaCS
open Akka.FSharp
open Akka.Routing
open Akka.Actor
open System.Threading

// Learn more about F# at http://fsharp.org
// See the 'F# Tutorial' project for more help.


let configuration = Configuration.load()

let mas = System.create "my-akka-system" (configuration)    

type ActorMessages = 
    | Counter of (int * int)
    | Message of string
    | PerformLogon


let rec do_counting (i1, i2) =
    Thread.Sleep(500)    
    printfn "%i" i1
    if i1 < i2 then do_counting ((i1 + 1), i2)

//let server_address = "akka.tcp://my-akka-system@localhost:8080/user/logon_acceptor"
let logon_acceptor = "akka://my-akka-system/user/logon_acceptor"

let rec do_work msg =
    match msg with
    | ActorMessages.Counter (i1, i2) -> 
        do_counting (i1, i2)
    | ActorMessages.Message s -> 
        printfn "%s" s
    | ActorMessages.PerformLogon -> 
        let logon_acceptor = mas.ActorSelection(logon_acceptor)
        printfn "i had better logon"
        logon_acceptor.Tell("Oh wont you please take me home")

let router = SpawnOption.Router(FromConfig.Instance)

let client = 
    spawne mas "client_worker" <@actorOf do_work@> [ router ]

let logon (mailbox: Actor<string>) msg =
    printfn "Received message from client: %s" msg 
    //let aref = mailbox.Context.Sender
    let aref = client
    aref.Tell("Welcome to the Jungle")
    aref.Tell (ActorMessages.Counter(1, 5), null)
    aref.Tell (ActorMessages.Counter(1, 5), null)
    aref.Tell (ActorMessages.Counter(1, 5), null)
    aref.Tell (ActorMessages.Counter(1, 5), null)
    aref.Tell (ActorMessages.Counter(1, 5), null)
    

let logonAcceptor =
    spawn mas "logon_acceptor" (actorOf2 logon)

[<EntryPoint>]
let main argv =     
    
    printfn "%A" argv

    client.Tell(ActorMessages.PerformLogon)

    do Console.ReadLine() |> ignore
    
    0 // return an integer exit code
