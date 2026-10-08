# Mock Client App

A small ASP.NET Core app that behaves like an external app using the SSO Gateway.
It exists so we can test the full SSO flow end to end.

## How it works

1. The home page shows a *Login with SSO* button.
2. The button sends the browser to <Gateway>/Auth/Login?returnUrl=<client callback URL>.
3. After a successful login the Gateway redirects to <callback URL>?token=<jwt>.
4. callback.html saves the token in sessionStorage and returns to /.
5. The home page calls GET /api/userinfo with the token as a Bearer header.
   The API validates the JWT (signature, issuer, audience, expiry) and returns the user's email, groups and levels.

## One-time setup

1. *Same signing key.* Copy Jwt:Key from Gateway/appsettings.json into MockClient/appsettings.json.
   Jwt:Issuer and Jwt:Audience must also match. If they differ, every token is rejected.
2. *Check the URLs* in MockClient/appsettings.json:
   - Sso:BaseUrl is the Gateway's https address (default https://localhost:7245).
   - Sso:ClientCallbackUrl is this app's callback (default https://localhost:7300/callback.html).
3. *Register this app in the Gateway* (Admin > Apps > Register New App):
   - Name: MockClient
   - Return URL: https://localhost:7300/callback.html
   The Return URL must match Sso:ClientCallbackUrl character for character, or the Gateway shows "Unapproved External App".
4. *Create a group and assign it* (optional but recommended): create a group for MockClient (e.g. Admin, level 0)
   and assign it to a test user. Otherwise groups and levels come back empty.

## Run both apps

# terminal 1 - SSO Gateway (https://localhost:7245)
dotnet run --project Gateway --launch-profile https

# terminal 2 - Mock client (https://localhost:7300)
dotnet run --project MockClient --launch-profile https

## Try the flow

1. Open https://localhost:7300. You should see "You are not signed in" and the *Login with SSO* button.
2. Click it. You land on the Gateway login page saying "Sign in to continue to MockClient".
3. Sign in with the test user. You are sent back and the page shows the email, app, groups and levels.
4. *Log out* clears the saved token (it does not sign you out of the Gateway).

Things worth trying: a wrong password, an inactive user ("Account Suspended"),
and a Return URL that is not registered ("Unapproved External App").
To see the expired-session handling, open dev tools > Application > Session Storage, change a character in sso_token
and reload. You get "Your session is no longer valid" and the login button again.
The expired-specific message is covered by the automated tests.

## Tests

## JWT claims

The Gateway issues an HS256-signed JWT. Example payload:

{
  "iss": "IT_ELECTIVE_SSO",
  "aud": "IT_ELECTIVE_SSO_CLIENTS",
  "nbf": 1790000000,
  "iat": 1790000000,
  "exp": 1790032400,
  "sub": "6666a0af-8c98-456e-8aa6-270ca59be34f",
  "email": "andrei@gmail.com",
  "tenant_app": "MockClient",
  "groups": "MockClient-Admin,MockClient-Manager",
  "levels": { "MockClient-Admin": 0, "MockClient-Manager": 1 }
}

| Claim | Type | Meaning |
|---|---|---|
| iss | string | Issuer. Always IT_ELECTIVE_SSO. Client apps must check it. |
| aud | string | Audience. Always IT_ELECTIVE_SSO_CLIENTS. Client apps must check it. |
| sub | string | The user's ID (GUID). |
| email | string | The user's email. |
| tenant_app | string | Name of the registered app the user logged in to. |
| groups | string | Comma-separated names of the user's groups *for this app only*. Empty string if none. |
| levels | JSON object | Map of group name to power level for this app. 0 is the highest access; higher numbers mean less power. |
| iat / nbf | number | Issued-at / not-before time (Unix seconds). |
| exp | number | Expiry time (Unix seconds). Tokens last 9 hours. |

### How a client app should validate a token

1. Verify the signature using the shared Jwt:Key.
2. Check iss and aud.
3. Check exp has not passed. If it has, send the user back through *Login with SSO*.
4. Read email, groups and levels from the claims. Parse levels as JSON.
dotnet test Tests.Integration

## Known limitations (by design for this project)

- The token travels in the URL query string, as specified in the master plan. It can end up in browser history or server logs.
  callback.html uses location.replace so the URL is not kept in history.
- The Gateway does not keep its own login session, so every *Login with SSO* asks for credentials again.
- Tokens are signed with a shared secret (HS256). Any app holding the key could create valid tokens.
  A production system would use asymmetric keys (RS256) so client apps only need the public key.