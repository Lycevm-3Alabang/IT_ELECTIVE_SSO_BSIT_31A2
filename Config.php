<?php
declare(strict_types=1);

return [
    'sso_base_url' => 'https://localhost:7425',
    'callback_url' => 'https://localhost:7300/callback.html',
    'jwt_secret' => getenv('SSO_JWT_SECRET') ?: '',
    'jwt_issuer' => getenv('SSO_JWT_ISSUER') ?: '',
    'jwt_audience' => getenv('SSO_JWT_AUDIENCE') ?: '',
];