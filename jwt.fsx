(*
Reference: https://vmsdurano.com/-net-core-3-1-signing-jwt-with-rsa/

Header
{
  "alg": "RS256",
  "typ": "JWT"
}

Payload
{
  "Name": "John Doe",
  "Admin": true
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

type Payload = { Name: String; Admin: bool }

let readKey file =
    let content = File.ReadAllText file
    content.Replace("-----BEGIN RSA PRIVATE KEY-----", "").Replace("-----END RSA PRIVATE KEY-----", "")
           .Replace("-----BEGIN PUBLIC KEY-----", "").Replace("-----END PUBLIC KEY-----", "").Replace("\r\n", "")
           .Replace("\n", "")
    |> Convert.FromBase64String

let pubKey = readKey "key.pub"
let privKey = readKey "key.priv"

let sign payload privKey =
    use rsa = RSA.Create()
    let mutable bytesRead = 0
    rsa.ImportRSAPrivateKey(new ReadOnlySpan<byte>(privKey), &bytesRead)

    let crypProvFactory =
        new CryptoProviderFactory(CacheSignatureProviders = false)

    let signingCredentials =
        new SigningCredentials(new RsaSecurityKey(rsa),
                               SecurityAlgorithms.RsaSha256,
                               CryptoProviderFactory = crypProvFactory)

    let claims =
        [| new Claim(nameof (payload.Name), payload.Name)
           new Claim(nameof (payload.Admin), payload.Admin.ToString()) |]

    let jwt =
        new JwtSecurityToken(claims = claims, signingCredentials = signingCredentials)

    let jwtHandler = new JwtSecurityTokenHandler()
    jwtHandler.WriteToken(jwt)


let validate (token: string) pubKey =
    CryptoProviderFactory.Default.CacheSignatureProviders <- false
    use rsa = RSA.Create()
    let mutable bytesRead = 0
    rsa.ImportSubjectPublicKeyInfo(new ReadOnlySpan<byte>(pubKey), &bytesRead)

    let validationParameters =
        new TokenValidationParameters(ValidateIssuer = false,
                                      ValidateAudience = false,
                                      ValidateLifetime = false,
                                      ValidateIssuerSigningKey = true,
                                      IssuerSigningKey = new RsaSecurityKey(rsa))

    let handler = new JwtSecurityTokenHandler()
    let mutable validatedToken: SecurityToken = upcast new JwtSecurityToken()

    try
        handler.ValidateToken(token, validationParameters, &validatedToken)
        |> ignore
        true
    with ex ->
        printfn $"Exception: {ex.Message}"
        false

let payload = { Name = "John Doe"; Admin = true }
printfn $"Payload: {payload}"

let token = sign payload privKey
printfn $"Signed JWT: {token}"

let isValid = "Valid token"
let isInvalid = "Invalid token"
printfn $"Validate: {if validate token pubKey then isValid else isInvalid}"
