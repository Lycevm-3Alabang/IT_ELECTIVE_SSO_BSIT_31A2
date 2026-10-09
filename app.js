require("dotenv").config();

const express = require("express");
const session = require("express-session");
const jwt = require("jsonwebtoken");
const crypto = require("crypto");

const app = express();

const {
    PORT = 3000,
    SSO_BASE_URL,
    CALLBACK_URL,
    JWT_SECRET,
    JWT_ISSUER,
    JWT_AUDIENCE,
    SESSION_SECRET,
} = process.env;

if (!SESSION_SECRET) {
    throw new Error("SESSION_SECRET is required.");
}

app.use(express.json());
app.use(express.urlencoded({ extended: false }));

app.use(session({
    name: "sso-client.sid",
    secret: SESSION_SECRET,
    resave: false,
    saveUninitialized: false,
    cookie: {
        httpOnly: true,
        secure: process.env.NODE_ENV === "production",
        sameSite: "lax",
        maxAge: 30 * 60 * 1000,
    },
}));

function requireAuth(req, res, next) {
    if (!req.session.user) {
        return res.redirect("/login");
    }

    next();
}

app.get("/", (req, res) => {
    res.send('<h1>SSO Client</h1><a href="/login">Login</a>');
});

app.get("/login", (req, res) => {
    if (!SSO_BASE_URL || !CALLBACK_URL) {
        return res.status(500).send("SSO configuration is incomplete.");
    }

    const state = crypto.randomBytes(32).toString("hex");
    req.session.ssoState = state;

    const loginUrl = new URL("/Auth/Login", SSO_BASE_URL);

    // Confirm these parameter names against your Gateway code.
    loginUrl.searchParams.set("callbackUrl", CALLBACK_URL);
    loginUrl.searchParams.set("state", state);

    res.redirect(loginUrl.toString());
});

app.get("/callback", (req, res) => {
    const { token, state } = req.query;

    if (
        typeof state !== "string" ||
        typeof req.session.ssoState !== "string" ||
        !crypto.timingSafeEqual(
            Buffer.from(state),
            Buffer.from(req.session.ssoState)
        )
    ) {
        return res.status(401).send("Invalid authentication state.");
    }

    if (!token || typeof token !== "string" || !JWT_SECRET) {
        return res.status(401).send("Missing token or validation configuration.");
    }

    try {
        const options = { algorithms: ["HS256"] };

        if (JWT_ISSUER) options.issuer = JWT_ISSUER;
        if (JWT_AUDIENCE) options.audience = JWT_AUDIENCE;

        const claims = jwt.verify(token, JWT_SECRET, options);

        if (!claims.sub) {
            return res.status(401).send("Required user claim is missing.");
        }

        req.session.regenerate((err) => {
            if (err) {
                return res.status(500).send("Unable to establish session.");
            }

            req.session.user = {
                sub: claims.sub,
                email: claims.email ?? null,
                groups: claims.groups ?? [],
                levels: claims.levels ?? [],
            };

            req.session.save((saveError) => {
                if (saveError) {
                    return res.status(500).send("Unable to save session.");
                }

                res.redirect("/dashboard");
            });
        });
    } catch {
        return res.status(401).send("Authentication token is invalid.");
    }
});

app.get("/dashboard", requireAuth, (req, res) => {
    res.json({
        message: "Authenticated successfully",
        user: req.session.user,
    });
});

app.get("/api/me", (req, res) => {
    if (!req.session.user) {
        return res.status(401).json({
            error: "Not authenticated",
        });
    }

    res.json({ user: req.session.user });
});

app.post("/api/logout", (req, res, next) => {
    req.session.destroy((err) => {
        if (err) return next(err);

        res.clearCookie("sso-client.sid");
        res.status(204).end();
    });
});

app.listen(Number(PORT), () => {
    console.log(`SSO client listening on port ${PORT}`);
});