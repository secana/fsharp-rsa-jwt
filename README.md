# F# JWT RSA Example

F# example for using RSA signing and validation for Json Web Tokens (JWT)

The example script takes a payload and creates a JWT, signed with a RSA private key. It then validates the created token with the corresponding public key.

Blog post with explanations: [F# JWT with RSA](https://secanablog.wordpress.com/2020/09/28/f-jwt-with-rsa/)

## Run

Clone the repository and run the `jwt.fsx` file with `dotnet fsi jwt.fsx`.

The code uses features from F# 5 and .NET 5, make sure you have the correct SDK installed. As .NET 5 is in preview at the moment of writing this, use the preview flag to run the script. `dotnet fsi --langversion:preview .\jwt.fsx`. This won't be needed anymore, as soon as .NET 5 is officially released.

## Example

```powershell
> dotnet fsi --langversion:preview .\jwt.fsx
Payload: { Name = "John Doe"
  Admin = true }
Signed JWT: eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9.eyJOYW1lIjoiSm9obiBEb2UiLCJBZG1pbiI6IlRydWUifQ.ma49T3npXQJVMa-afyfFgIPW5PEYhrrYvX2mUA6rmzXHXq_Wy-ij9MLc0b6UxZX8STcRrSC93meIMa4a8LI7UBe0Pxn8IQBrhXztcElMfktMQoQWb7Osx9XwmqD1CaQWwz3FX963B4fQFdxx7GpxdLPj-CSOJZ4OZbk8fWpurVX1QXMLokaJ8C-gLB026jFVJjIV1APSMOnAzx9lcZfU5m3jwVP8HMIc0yJkm4d7IJO1lQjYnUWQkY_DmwR8-vysqo3N5yY57xQUFRoyHwFofDb25fA6SkKcNHrOX0_bc7KzxzWacoPWgtUolThKasWpXgqHipR-uJ4hz6zahInCmw
Validate: Valid token
```

## Attention

Do not use the private key in this repo for anything else than testing. Always create a new private/public key pair and never share your private key with anyone!
