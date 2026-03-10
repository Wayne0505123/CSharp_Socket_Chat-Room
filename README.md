# TCP Chat Room System

![C#](https://img.shields.io/badge/language-C%23-blue)
![.NET](https://img.shields.io/badge/framework-.NET%20WPF-purple)
![Socket](https://img.shields.io/badge/network-TCP%20Socket-green)
![JSON](https://img.shields.io/badge/data-JSON-orange)

A **real-time desktop chat room system** built using **C#**, **WPF**, and **TCP socket programming**.  
The project implements a **client-server architecture** where multiple clients can connect to a server and exchange messages in real time.

This project demonstrates practical skills in:

- Network programming
- Desktop application development
- Multi-threading
- Client-server system design
- JSON-based data communication

---

# Project Overview

This project implements a **real-time chat system** using a **TCP-based client-server architecture**.

The **server application** manages incoming connections and broadcasts messages to all connected clients.  
The **client applications** provide a graphical chat interface that allows users to connect, send messages, and receive updates instantly.

Key learning objectives of this project include:

- Understanding **TCP socket communication**
- Implementing **multi-client networking systems**
- Handling **concurrent client connections**
- Integrating **network communication with GUI applications**

---

# System Architecture
```
        +----------------------+
        | Chat Server          |
        |----------------------|
        | TCP Listener         |
        | Client Manager       |
        | Message Broadcasting |
        +----------+-----------+
                   |
----------------------------------------
|                                      |
+-----------------+ +------------------+
| Chat Client 1   | | Chat Client 2    |
|-----------------| |------------------|
| WPF UI          | | WPF UI           |
| TCP Connection  | | TCP Connection   |
| Send / Receive  | | Send / Receive   |
+-----------------+ +------------------+
```


The system consists of three applications:

| Component | Description |
|--------|--------|
| ChatServer | Handles client connections and broadcasts messages |
| ChatClient | WPF client application used by users |
| ChatClient2 | Additional client instance used to simulate multiple users |

---

# Features

## Server Features

- GUI-based server management
- Configure server **IP address** and **port**
- Start / Stop server listening
- Display **real-time chat logs**
- Show **connected client list**
- Broadcast messages to all clients
- Automatically detect client disconnections

---

## Client Features

- Connect to server using **IP + Port**
- Set **custom username**
- Send and receive messages in real time
- Display connection status
- Disconnect from server manually
- Auto-notification when server disconnects

---

# GUI Interface

## Client Interface

Features:

- Server IP input
- Server port input
- Username input
- Connect / Disconnect buttons
- Chat message display window
- Message input box
- Send button

Example workflow:

Enter Server IP -> Connect -> Start Chatting


---

## Server Interface

Features:

- Start / Stop listening
- Server status indicator
- Connected clients list
- Message log window
- Manual broadcast message feature

---

# Technologies Used

| Technology | Purpose |
|--------|--------|
| C# | Core programming language |
| .NET / WPF | Desktop UI framework |
| TCP Socket | Network communication |
| JSON | Message serialization |
| Newtonsoft.Json | JSON parsing |
| Threading | Concurrent message receiving |
| Dispatcher | Safe UI updates from background threads |

---

# Message Communication Format

Messages are serialized using **JSON** before being transmitted over TCP.

Example message:

```json
{
  "Username": "user01",
  "Message": "Hello everyone!"
}
```

Advantages:

- Simple structure

- Easy to serialize / deserialize

- Language independent

---

# Installation & Setup
## Requirements  

- Visual Studio 2019 / 2022

- .NET Framework / .NET Desktop Development

- NuGet package

---

## How to run

### 1. Open the project
Open the solution in Visual Studio.

### 2. Restore NuGet packages
**Install required package:**

`Newtonsoft.Json`

### 3. Run the Server
Run:

`ChatServer`

Then:

`Enter IP -> Enter Port -> Click Start Listening`

### 4. Run Clients
Run:
```
ChatClient
ChatClient2
```
Then:
```
Enter Server IP
Enter Port
Enter Username
Click Connect
```

### 5. Start chatting
Messages sent by any client will be broadcast to all connected users.

