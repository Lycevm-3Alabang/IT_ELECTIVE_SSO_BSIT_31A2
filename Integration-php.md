# PHP Integration Guide

## Overview

A PHP application can integrate with the SSO Gateway by redirecting users to the Gateway and processing the authentication response through a registered callback.

The exact login endpoint, callback format, and token exchange must follow the Gateway's implemented protocol.

## Requirements

* PHP with session support.
* HTTPS for production.
* A JWT validation library if the Gateway returns JWTs.
* The correct SSO base URL and registered callback URL.

## Configuration

Create `config.php`:

```php
<?php

declare(strict_types=1);

return [
    'sso_base_url' => 'https://localhost:7001',
    'callback_url' => 'https://localhost:8000/callback.php',
];
```

Replace these example URLs with your actual configuration.

## Login

Create `index.php`:

```php
<?php

declare(strict_types=1);

session_start();

if (!empty($_SESSION['user'])) {
    header('Location: dashboard.php');
    exit;
}

$config = require __DIR__ . '/config.php';

// Replace this with the actual Gateway login endpoint.
// Add the registered callback URL and required security
// parameters according to the Gateway protocol.
$loginUrl = rtrim($config['sso_base_url'], '/')
    . '/REPLACE-WITH-LOGIN-ENDPOINT';

header('Location: ' . $loginUrl);
exit;
```

The login endpoint is a placeholder until it is matched to the actual Gateway implementation.

## Callback Handler

Create `callback.php`:

```php
<?php

declare(strict_types=1);

session_start();

// TODO:
// 1. Read the callback parameters required by the Gateway.
// 2. Validate state to prevent CSRF attacks.
// 3. Exchange an authorization code if the protocol requires it.
// 4. Validate the returned token or authentication response.
// 5. Verify signature, issuer, audience, and expiration
//    when JWT validation applies.
// 6. Map verified claims to the local user session.

http_response_code(501);
echo 'SSO callback validation has not been implemented.';
```

Do not accept user information or a JWT from the callback without validating it.

## Protected Dashboard

Create `dashboard.php`:

```php
<?php

declare(strict_types=1);

session_start();

if (empty($_SESSION['user'])) {
    header('Location: index.php');
    exit;
}

$user = $_SESSION['user'];

$email = $user['email'] ?? 'Not available';
$name = $user['name'] ?? 'User';
$groups = $user['groups'] ?? [];
$level = $user['level'] ?? 'Not available';

?>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>Dashboard</title>
</head>
<body>
    <h1>Dashboard</h1>

    <p>Name: <?= htmlspecialchars($name, ENT_QUOTES, 'UTF-8') ?></p>
    <p>Email: <?= htmlspecialchars($email, ENT_QUOTES, 'UTF-8') ?></p>
    <p>Level: <?= htmlspecialchars($level, ENT_QUOTES, 'UTF-8') ?></p>

    <h2>Groups</h2>
    <ul>
        <?php foreach ($groups as $group): ?>
            <li>
                <?= htmlspecialchars((string) $group, ENT_QUOTES, 'UTF-8') ?>
            </li>
        <?php endforeach; ?>
    </ul>

    <form action="logout.php" method="post">
        <button type="submit">Logout</button>
    </form>
</body>
</html>
```

The `user` session must be populated only after the callback handler validates the authentication result.

## Logout

Create `logout.php`:

```php
<?php

declare(strict_types=1);

session_start();

$_SESSION = [];

if (ini_get('session.use_cookies')) {
    $params = session_get_cookie_params();

    setcookie(
        session_name(),
        '',
        time() - 42000,
        $params['path'],
        $params['domain'],
        $params['secure'],
        $params['httponly']
    );
}

session_destroy();

header('Location: index.php');
exit;
```

For production, add CSRF protection to the logout form and configure secure session cookies.

## Testing

1. Start the PHP development server.
2. Open `index.php`.
3. Confirm the browser redirects to the correct Gateway endpoint.
4. Complete callback validation before creating a session.
5. Verify the dashboard is inaccessible without a valid session.
6. Confirm logout clears the local session.
