# Error Codes and Responses

## 1. Authentication Errors

| Error ID | Description                      | Recommended Action                              |
| -------- | -------------------------------- | ----------------------------------------------- |
| AUTH-001 | Invalid email or password        | Verify the user's credentials.                  |
| AUTH-002 | Account is suspended or inactive | Check the account status.                       |
| AUTH-003 | Too many failed login attempts   | Follow the configured lockout policy.           |
| AUTH-004 | Missing email address            | Enter an email address.                         |
| AUTH-005 | Invalid email format             | Enter a valid email address.                    |
| AUTH-006 | Missing password                 | Enter a password.                               |
| AUTH-007 | Unapproved client application    | Check the client registration and callback URL. |

These are documentation identifiers. Confirm each message against the actual Gateway code.

## 2. Configuration Errors

| Error ID   | Description                          | Recommended Action                                                                         |
| ---------- | ------------------------------------ | ------------------------------------------------------------------------------------------ |
| CONFIG-001 | JWT signing key is missing           | Configure the signing key securely.                                                        |
| CONFIG-002 | Invalid JWT issuer                   | Verify the expected issuer.                                                                |
| CONFIG-003 | Invalid JWT audience                 | Verify the expected audience.                                                              |
| CONFIG-004 | Incorrect SSO base URL               | Confirm that `https://localhost:7425` is reachable.                                        |
| CONFIG-005 | Invalid or unregistered callback URL | Confirm that `https://localhost:7300/callback.html` is registered for the intended client. |
| CONFIG-006 | Callback handler is unavailable      | Ensure that the callback page or endpoint exists and processes the response.               |

## 3. JWT Errors

| Error ID | Description                      | Recommended Action                            |
| -------- | -------------------------------- | --------------------------------------------- |
| JWT-001  | Token has expired                | Authenticate again.                           |
| JWT-002  | Token signature is invalid       | Verify the trusted signing configuration.     |
| JWT-003  | Token issuer is invalid          | Check issuer configuration.                   |
| JWT-004  | Token audience is invalid        | Check the intended audience.                  |
| JWT-005  | Required user claims are missing | Inspect token generation and claim mapping.   |
| JWT-006  | Token is missing or malformed    | Check callback parameters and token handling. |

These are recommended integration error categories, not confirmed standardized responses from the Gateway.

## 4. UI and Navigation Issues

| Error ID | Description                                                | Status         |
| -------- | ---------------------------------------------------------- | -------------- |
| UI-001   | Application title was incorrect or missing in the sidebar  | Reported fixed |
| UI-002   | Application logo was missing                               | Reported fixed |
| UI-003   | Sidebar buttons had incorrect navigation paths             | Reported fixed |
| UI-004   | Double-clicking an interface element still causes an error | Unresolved     |

### UI-001: Application Title

**Issue:** The sidebar did not display the intended application title.

**Expected result:** The sidebar displays `IT-ELECTIVE - SSO`.

**Status:** Reported fixed. Verify the title in the running application.

### UI-002: Missing Logo

**Issue:** The application logo was missing from the sidebar.

**Expected result:** The correct logo appears in the sidebar.

**Status:** Reported fixed. Verify the image path and rendering.

### UI-003: Incorrect Sidebar Navigation

**Issue:** Sidebar buttons had incorrect navigation paths.

**Expected result:** Each sidebar button opens its intended page.

**Status:** Reported fixed. Test all buttons and verify their routes.

### UI-004: Error When Double-Clicking

**Issue:** An error still occurs when the user double-clicks an interface element.

**Steps to reproduce:**

1. Start the application.
2. Open the affected page.
3. Double-click the button or interface element.
4. Record the error message and inspect the application logs.

**Expected result:** The application handles repeated clicks without an unexpected error.

**Actual result:** An error is reported, but its exact message and cause have not yet been confirmed.

**Recommended investigation:**

* Check whether the click handler runs twice.
* Check for duplicate requests or form submissions.
* Check whether navigation happens twice.
* Inspect browser console errors and server logs.
* Prevent repeated submissions while an operation is running, where appropriate.

**Resolution:** Pending investigation and verification.

## 5. HTTP Status Codes

| HTTP Status | Meaning                           |
| ----------- | --------------------------------- |
| 200         | Request completed successfully    |
| 302         | Redirect                          |
| 400         | Invalid request                   |
| 401         | Authentication missing or invalid |
| 403         | Access denied                     |
| 404         | Resource not found                |
| 500         | Internal server error             |

Confirm the actual status codes returned by your application's routes.

## 6. Reporting New Errors

When another error occurs, record:

* Error ID
* Date and time
* Affected page or endpoint
* Steps to reproduce
* Exact error message
* HTTP status, if applicable
* Resolution and verification result

Do not record passwords, signing keys, live JWTs, or session cookies.

## 7. Configuration Reference

SSO Gateway URL: `https://localhost:7425`

Configured callback URL: `https://localhost:7300/callback.html`

These values must match the Gateway configuration and the client application's actual callback implementation.
