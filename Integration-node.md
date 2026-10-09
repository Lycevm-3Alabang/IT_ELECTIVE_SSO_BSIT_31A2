# Node.js and Express Integration Guide

## Overview

A Node.js/Express application can authenticate users through the SSO Gateway and maintain a local session after validating the authentication response.

This guide provides a starter structure. The callback and token-validation logic must match the protocol and token configuration implemented by the Gateway.

## Requirements

* Node.js and npm.
* Express.
* Express session middleware.
* A trusted authentication or JWT validation library if required by the Gateway.

## Install Dependencies

```bash
npm init -y
npm install express express-session dotenv
```

Install an appropriate JWT validation library if the SSO protocol returns JWTs directly and your application is responsible for validating them.

## Environment Configuration

Create `.env`:

```env
PORT=3000
SSO_BASE_URL=https://localhost:7001
SESSION_SECRET=replace-with-a-long-random-secret
```

Use a strong secret stored securely in production. Do not commit `.env` to version control.

## Express Application

Create `app.js`:

```javascript
require("dotenv").config();

const express = require("express");
const session = require("express-session");

const app = express();

app.use(express.urlencoded({ extended: false }));
app.use(express.json());

app.use(
  session({
    name: "client.sid",
    secret: process.env.SESSION_SECRET,
    resave: false,
    saveUninitialized: false,
    cookie: {
      httpOnly: true,
      secure: process.env.NODE_ENV === "production",
      sameSite: "lax",
      maxAge: 30 * 60 * 1000,
    },
  })
);

function requireAuth(req, res, next) {
  if (!req.session.user) {
    return res.redirect("/login");
  }

  next();
}

app.get("/", (req, res) => {
  res.send("SSO client is running.");
});

app.get("/login", (req, res) => {
  const ssoBaseUrl = process.env.SSO_BASE_URL;

  if (!ssoBaseUrl) {
    return res.status(500).send("SSO base URL is not configured.");
  }

  // Replace this path and add the required registered callback
  // URL and security parameters for the actual Gateway protocol.
  const loginUrl =
    ssoBaseUrl.replace(/\/$/, "") +
    "/REPLACE-WITH-LOGIN-ENDPOINT";

  res.redirect(loginUrl);
});

app.get("/callback", async (req, res) => {
  // TODO:
  // 1. Validate state and other required callback parameters.
  // 2. Exchange an authorization code if the protocol requires it.
  // 3. Validate the authentication response or JWT.
  // 4. Verify signature, issuer, audience, and expiration as applicable.
  // 5. Store only verified user information in the session.

  return res
    .status(501)
    .send("SSO callback validation has not been implemented.");
});

app.get("/dashboard", requireAuth, (req, res) => {
  const user = req.session.user;

  res.json({
    message: "Authenticated dashboard",
    user: {
      name: user.name ?? null,
      email: user.email ?? null,
      groups: user.groups ?? [],
      level: user.level ?? null,
    },
  });
});

app.post("/logout", (req, res, next) => {
  req.session.destroy((err) => {
    if (err) {
      return next(err);
    }

    res.clearCookie("client.sid");
    res.redirect("/");
  });
});

const port = Number(process.env.PORT || 3000);

app.listen(port, () => {
  console.log(`Client application running on port ${port}`);
});
```

The callback intentionally returns HTTP `501` until the real authentication protocol is implemented. Do not treat this starter application as a completed SSO integration.

## Middleware Responsibilities

The `requireAuth` middleware checks whether the client session contains a user. It protects the dashboard from unauthenticated access.

This check is only reliable when the session is created after successful authentication validation.

## JWT Validation

If the Gateway returns a JWT, validate it using a trusted library and the correct signing configuration.

At minimum, validate the signature, issuer, audience, expiration, and any required claims before establishing the session.

Do not trust a decoded token payload by itself.

## Testing

1. Start the application using `node app.js`.
2. Open `http://localhost:3000`.
3. Visit `/login` and verify the Gateway redirect.
4. Implement callback validation using the actual Gateway protocol.
5. Verify `/dashboard` requires an authenticated session.
6. Verify logout destroys the local session.
7. Test invalid and expired authentication responses.
