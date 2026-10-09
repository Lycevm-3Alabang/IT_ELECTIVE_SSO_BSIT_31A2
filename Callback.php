<?php
declare(strict_types=1);

session_start();
require_once __DIR__ . '/vendor/autoload.php';

use Firebase\JWT\JWT;
use Firebase\JWT\Key;

$config = require __DIR__ . '/config.php';

function failLogin(string $message, int $status = 401): never
{
    http_response_code($status);
    exit(htmlspecialchars($message, ENT_QUOTES, 'UTF-8'));
}

$token = $_GET['token'] ?? '';
$state = $_GET['state'] ?? '';

if (!is_string($token) || $token === '') {
    failLogin('Missing authentication token.');
}

if (
    !is_string($state) ||
    empty($_SESSION['sso_state']) ||
    !hash_equals($_SESSION['sso_state'], $state)
) {
    failLogin('Invalid authentication state.');
}

if ($config['jwt_secret'] === '') {
    failLogin('JWT signing key is not configured.', 500);
}

try {
    $claims = (array) JWT::decode(
        $token,
        new Key($config['jwt_secret'], 'HS256')
    );

    if (
        $config['jwt_issuer'] !== '' &&
        ($claims['iss'] ?? null) !== $config['jwt_issuer']
    ) {
        failLogin('Invalid token issuer.');
    }

    if (
        $config['jwt_audience'] !== '' &&
        !in_array(
            $config['jwt_audience'],
            (array) ($claims['aud'] ?? []),
            true
        )
    ) {
        failLogin('Invalid token audience.');
    }

    if (empty($claims['sub'])) {
        failLogin('Required user claim is missing.');
    }

    session_regenerate_id(true);

    $_SESSION['user'] = [
        'sub' => $claims['sub'],
        'email' => $claims['email'] ?? null,
        'groups' => $claims['groups'] ?? [],
        'levels' => $claims['levels'] ?? [],
    ];

    unset($_SESSION['sso_state']);

    header('Location: dashboard.php');
    exit;
} catch (Throwable $e) {
    failLogin('Authentication failed.');
}