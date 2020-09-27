(*
Reference: https://vmsdurano.com/-net-core-3-1-signing-jwt-with-rsa/

Header
{
  "alg": "RS256",
  "typ": "JWT"
}

Payload
{
  "name": "John Doe",
  "admin": true
}
*)

#r "nuget: System.IdentityModel.Tokens.Jwt"
#r "nuget: Microsoft.IdentityModel.Tokens"

open System.IdentityModel.Tokens.Jwt
open System
open System.IO
open System.Text
open System.Security.Claims
open System.IdentityModel.Tokens.Jwt
open Microsoft.IdentityModel.Tokens
open System.Security.Cryptography

type Payload = { name: String; admin: bool }


let readKey file =
    let content = File.ReadAllText "key.pub"
    content.Replace("-----BEGIN RSA PRIVATE KEY-----", "").Replace("-----END RSA PRIVATE KEY-----", "")
           .Replace("-----BEGIN PUBLIC KEY-----", "").Replace("-----END PUBLIC KEY-----", "").Replace("\r\n", "")
           .Replace("\n", "")
    |> Convert.FromBase64String

let token = File.ReadAllText("token.txt")
let pubKey = readKey "key.pub"

let validate (token: string) pubKey =
    use rsa = RSA.Create()
    let mutable bytesRead = 0
    rsa.ImportSubjectPublicKeyInfo(new ReadOnlySpan<byte>(pubKey), &bytesRead)
    printfn $"{nameof bytesRead}: {bytesRead}"

    let validationParameters =
        new TokenValidationParameters(ValidateIssuer = false,
                                      ValidateAudience = false,
                                      ValidateLifetime = false,
                                      ValidateIssuerSigningKey = true,
                                      ValidIssuer = "",
                                      ValidAudience = "",
                                      IssuerSigningKey = new RsaSecurityKey(rsa))

    try
        let handler = new JwtSecurityTokenHandler()
        let mutable validatedToken: SecurityToken = upcast new JwtSecurityToken()
        handler.ValidateToken(token, validationParameters, &validatedToken)
        true
    with :? System.Exception as e ->
        printfn $"Exception: {e.Message}"
        false

match validate token pubKey with
| true -> "Is valid!"
| false -> "Is invalid!"
