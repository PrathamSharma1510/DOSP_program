module Server

open System
open System.Net
open System.Net.Sockets
open System.Text

let mutable clientCount = 0

let calculateResult (command: string) (numbers: string list) =
    try
        let numbers = List.map float numbers
        match command.ToLower() with
        | "add" -> Some (List.sum numbers)
        | "subtract" -> Some (List.reduce (-) numbers)
        | "multiply" -> Some (List.reduce (*) numbers)
        | "terminate" -> Some -5.0
        | "bye" -> Some -5.0
        | _ -> None
    with
        | _ -> None

let startServer (port: int) =
    try
        let listener = new TcpListener(IPAddress.Any, port)
        listener.Start()
        printfn "Server is running and listening on port %d." port

        while true do
            let client = listener.AcceptTcpClient()
            clientCount <- clientCount + 1
            let stream = client.GetStream()
            let buffer = Array.zeroCreate 256
            let bytesRead = stream.Read(buffer, 0, buffer.Length)
            let message = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim()
            printfn "Received: %s" message

            let parts = message.Split(' ') |> Array.toList
            let command = List.head parts
            let numbers = List.tail parts

            match calculateResult command numbers with
            | Some result -> 
                printfn "Responding to client %d with result: %A" clientCount result
                let response = Encoding.ASCII.GetBytes(result.ToString())
                stream.Write(response, 0, response.Length)
            | None -> 
                printfn "Responding to client %d with result: one or more of the inputs contain(s) non-number(s)" clientCount
                let response = Encoding.ASCII.GetBytes("one or more of the inputs contain(s) non-number(s)")
                stream.Write(response, 0, response.Length)

            client.Close()
    with
        | ex -> printfn "An error occurred: %s" (ex.Message)
