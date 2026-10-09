<?php
declare(strict_types=1);

session_start();

if (empty($_SESSION['user'])) {
    header('Location: index.php');
    exit;
}

$user = $_SESSION['user'];
?>
<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="UTF-8">
    <title>SSO Dashboard</title>
</head>
<body>
    <h1>Dashboard</h1>

    <p>User ID:
        <?= htmlspecialchars((string) $user['sub']) ?>
    </p>

    <p>Email:
        <?= htmlspecialchars((string) ($user['email'] ?? 'N/A')) ?>
    </p>

    <h2>Groups</h2>
    <ul>
        <?php foreach ((array) $user['groups'] as $group): ?>
            <li><?= htmlspecialchars((string) $group) ?></li>
        <?php endforeach; ?>
    </ul>

    <h2>Levels</h2>
    <ul>
        <?php foreach ((array) $user['levels'] as $level): ?>
            <li><?= htmlspecialchars((string) $level) ?></li>
        <?php endforeach; ?>
    </ul>

    <form method="post" action="logout.php">
        <button type="submit">Logout</button>
    </form>
</body>
</html>