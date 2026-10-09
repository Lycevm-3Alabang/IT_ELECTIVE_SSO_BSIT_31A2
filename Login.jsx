export default function Login() {
    return (
        <main>
            <h1>Sign in</h1>
            <button onClick={() => {
                window.location.assign("/api/login");
            }}>
                Sign in with SSO
            </button>
        </main>
    );
}