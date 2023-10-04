module Client

open System
open System.Net.Sockets
open System.Text

let rec startClient (port: int) =
    try
        let client = new TcpClient("127.0.0.1", port)
        let stream = client.GetStream()

        printfn "Enter your command and numbers (e.g., add 1 2 3) or 'exit' to quit:"
        let user_input = Console.ReadLine()

        if user_input = "exit" then
            printfn "Exiting client."
        else
            printfn "Sending command: %s" user_input
            let message = Encoding.ASCII.GetBytes(user_input)
            stream.Write(message, 0, message.Length)

            let buffer = Array.zeroCreate 256
            let bytesRead = stream.Read(buffer, 0, buffer.Length)
            let response = Encoding.ASCII.GetString(buffer, 0, bytesRead).Trim()
            printfn "Server response: %s" response

            startClient port  // Recursive call to continue

        client.Close()
    with
        | ex -> printfn "An error occurred: %s" (ex.Message)
