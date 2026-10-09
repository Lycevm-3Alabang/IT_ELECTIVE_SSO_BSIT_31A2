# React Integration Guide

## Overview

A React application can use the SSO Gateway to authenticate users. React manages the user interface, while a trusted authentication flow must handle callback validation and session establishment.

A React application should not treat decoded JWT claims as proof of authentication.

## Recommended Structure

```text
src/
├── auth/
│   └── AuthContext.jsx
├── components/
│   └── ProtectedRoute.jsx
├── pages/
│   ├── Login.jsx
│   ├── Callback.jsx
│   └── Dashboard.jsx
└── App.jsx
```

## Authentication Context

Create `src/auth/AuthContext.jsx`:

```jsx
import {
  createContext,
  useContext,
  useMemo,
  useState,
} from "react";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(false);

  const value = useMemo(
    () => ({
      user,
      loading,

      // Call only after a trusted callback handler has
      // validated the authentication result.
      setAuthenticatedUser: (verifiedUser) => {
        setUser(verifiedUser);
      },

      clearUser: () => {
        setUser(null);
      },
    }),
    [user, loading]
  );

  return (
    <AuthContext.Provider value={value}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);

  if (!context) {
    throw new Error("useAuth must be used within AuthProvider");
  }

  return context;
}
```

This context manages UI state only. It is not a substitute for server-side session or token validation.

## ProtectedRoute Component

Create `src/components/ProtectedRoute.jsx`:

```jsx
import { Navigate, Outlet } from "react-router-dom";
import { useAuth } from "../auth/AuthContext";

export default function ProtectedRoute() {
  const { user } = useAuth();

  if (!user) {
    return <Navigate to="/login" replace />;
  }

  return <Outlet />;
}
```

For applications using a backend session, the app should check the session with the backend before treating the user as authenticated.

## Login Page

Create `src/pages/Login.jsx`:

```jsx
export default function Login() {
  const ssoBaseUrl = import.meta.env.VITE_SSO_BASE_URL;

  function handleLogin() {
    if (!ssoBaseUrl) {
      throw new Error("SSO base URL is not configured");
    }

    // Replace this path with the actual Gateway login endpoint.
    window.location.assign(
      `${ssoBaseUrl.replace(/\/$/, "")}/REPLACE-WITH-LOGIN-ENDPOINT`
    );
  }

  return (
    <main>
      <h1>Sign in</h1>
      <button onClick={handleLogin}>Continue with SSO</button>
    </main>
  );
}
```

Create `.env` in the React project:

```env
VITE_SSO_BASE_URL=https://localhost:7001
```

Only public configuration belongs in a frontend environment variable. Never put client secrets or signing keys in React environment variables.

## Callback Page

Create `src/pages/Callback.jsx`:

```jsx
export default function Callback() {
  return (
    <main>
      <h1>Completing sign-in</h1>
      <p>
        Implement callback processing using the protocol supported
        by the SSO Gateway.
      </p>
    </main>
  );
}
```

The callback handler must validate state and process the actual authorization response. If JWTs are used, validate them through a trusted backend or an appropriate secure protocol implementation before creating an authenticated session.

## Dashboard

Create `src/pages/Dashboard.jsx`:

```jsx
import { useAuth } from "../auth/AuthContext";

export default function Dashboard() {
  const { user, clearUser } = useAuth();

  if (!user) {
    return <p>No authenticated user is available.</p>;
  }

  return (
    <main>
      <h1>Dashboard</h1>
      <p>Name: {user.name ?? "Not available"}</p>
      <p>Email: {user.email ?? "Not available"}</p>
      <p>Level: {user.level ?? "Not available"}</p>

      <h2>Groups</h2>
      <ul>
        {(user.groups ?? []).map((group) => (
          <li key={group}>{group}</li>
        ))}
      </ul>

      <button onClick={clearUser}>Clear local user state</button>
    </main>
  );
}
```

Clearing React state alone does not terminate the backend session or the SSO session. Implement logout through the appropriate backend and Gateway endpoints.

## Route Configuration

Create or update `src/App.jsx`:

```jsx
import {
  BrowserRouter,
  Navigate,
  Route,
  Routes,
} from "react-router-dom";

import { AuthProvider } from "./auth/AuthContext";
import ProtectedRoute from "./components/ProtectedRoute";
import Login from "./pages/Login";
import Callback from "./pages/Callback";
import Dashboard from "./pages/Dashboard";

export default function App() {
  return (
    <AuthProvider>
      <BrowserRouter>
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route path="/callback" element={<Callback />} />

          <Route element={<ProtectedRoute />}>
            <Route path="/dashboard" element={<Dashboard />} />
          </Route>

          <Route path="*" element={<Navigate to="/dashboard" replace />} />
        </Routes>
      </BrowserRouter>
    </AuthProvider>
  );
}
```

Install React Router if it is not already installed:

```bash
npm install react-router-dom
```

## Testing

* Confirm the login button redirects to the configured Gateway.
* Confirm the callback validates the authentication response.
* Verify protected routes cannot be accessed without authentication.
* Test session expiration and logout.
* Ensure user roles and groups are checked by a trusted backend when they control sensitive operations.
