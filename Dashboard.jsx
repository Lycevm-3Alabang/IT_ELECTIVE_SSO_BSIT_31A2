import { useAuth } from "./AuthContext";

export default function Dashboard() {
    const { user, logout } = useAuth();

    if (!user) return null;

    return (
        <main>
            <h1>Dashboard</h1>
            <p>User ID: {user.sub ?? "N/A"}</p>
            <p>Email: {user.email ?? "N/A"}</p>

            <h2>Groups</h2>
            <ul>
                {(user.groups ?? []).map((group, index) => (
                    <li key={`${group}-${index}`}>{group}</li>
                ))}
            </ul>

            <h2>Levels</h2>
            <ul>
                {(user.levels ?? []).map((level, index) => (
                    <li key={`${level}-${index}`}>{level}</li>
                ))}
            </ul>

            <button onClick={logout}>Logout</button>
        </main>
    );
}