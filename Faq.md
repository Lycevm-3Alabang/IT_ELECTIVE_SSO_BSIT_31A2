# Frequently Asked Questions

## 1. What is Single Sign-On (SSO)?

SSO allows a user to authenticate through a central service and use supported client applications without implementing a separate authentication process in each application.

## 2. What is the Gateway?

The Gateway is the central ASP.NET Core application that provides the SSO functionality implemented in this project.

## 3. What is the MockClient?

The MockClient is a client application used to test and demonstrate the SSO integration flow.

## 4. Where should I configure the SSO base URL?

For an ASP.NET Core client, configure the URL in `appsettings.json`, under `Sso:BaseUrl`.

For other applications, use the configuration mechanism appropriate to that technology.

## 5. Why is the login redirect failing?

Check that:

* The Gateway is running.
* The SSO base URL is correct.
* The login endpoint exists.
* The client uses the correct HTTP or HTTPS scheme.
* The configured callback URL is registered if required.
* The Gateway and client are reachable from the browser.

## 6. Why is the callback failing?

Verify the callback route and HTTP method. Check the callback parameters and follow the actual authentication protocol. If the protocol uses authorization codes, state parameters, or JWTs, validate them as required.

Do not bypass validation to make a callback succeed.

## 7. Why is the JWT rejected?

If JWTs are used, verify that:

* The token has not expired.
* The signature is valid.
* The issuer matches the expected issuer.
* The audience matches the intended recipient.
* The token uses the expected signing algorithm.
* Required claims are present.

Check the Gateway's token-generation configuration before changing client validation settings.

## 8. Why are email, groups, or level missing?

Inspect the claims issued by the Gateway and compare them with the names expected by the client. Claim names and mappings may differ between applications.

Do not assume that every token contains every optional claim.

## 9. Why can a user open the login page but not the dashboard?

The dashboard may require a valid local session. Confirm that the callback completes successfully, the authentication result is validated, and the client establishes the expected session.

## 10. Does logging out of a client log the user out everywhere?

Not necessarily. Clearing a client's local session may leave the user's SSO session or other client sessions active.

Check whether the Gateway supports a remote logout mechanism and implement it according to the actual protocol.

## 11. Can I decode a JWT and immediately trust its claims?

No. Decoding only reveals the payload. Validate the token before using its claims for authentication or authorization.

## 12. Should I put client secrets in React environment variables?

No. Frontend environment variables can be included in the built application. Keep secrets on a trusted backend or in secure server-side configuration.

## 13. Where should I document errors encountered during testing?

Record them in `error-codes.md`. Include the actual error message, where it occurred, the steps to reproduce it, the HTTP status if applicable, and the verified solution.

## 14. What should I do if an integration example does not work?

Compare the example against the actual Gateway routes, token format, claim names, callback requirements, and configuration. The examples in this guide are starting points and must be adapted to the implemented protocol.

## 15. What should be tested before deployment?

* Successful login and callback processing.
* Invalid and expired tokens, if JWTs are used.
* Correct issuer and audience validation.
* Access to protected routes.
* Logout behavior.
* Callback URL validation.
* Secure cookie and session settings.
* Appropriate handling of authentication failures.
