<?php
declare(strict_types=1);

session_start();

$config = require __DIR__ . '/config.php';

if (!empty($_SESSION['user'])) {
    header('Location: dashboard.php');
    exit;
}

if (empty($_SESSION['sso_state'])) {
    $_SESSION['sso_state'] = bin2hex(random_bytes(32));
}

$loginUrl = rtrim($config['sso_base_url'], '/') . '/Auth/Login';

$params = http_build_query([
    'callbackUrl' => $config['callback_url'],
    'state' => $_SESSION['sso_state'],
]);

header('Location: ' . $loginUrl . '?' . $params);
exit;