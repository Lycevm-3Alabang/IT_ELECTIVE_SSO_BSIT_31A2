import {
    createContext,
    useContext,
    useEffect,
    useState,
} from "react";

const AuthContext = createContext(null);

export function AuthProvider({ children }) {
    const [user, setUser] = useState(null);
    const [loading, setLoading] = useState(true);

    useEffect(() => {
        fetch("/api/me", { credentials: "include" })
            .then((response) =>
                response.ok ? response.json() : null
            )
            .then((data) => setUser(data?.user ?? null))
            .catch(() => setUser(null))
            .finally(() => setLoading(false));
    }, []);

    async function logout() {
        const response = await fetch("/api/logout", {
            method: "POST",
            credentials: "include",
        });

        if (response.ok) {
            setUser(null);
        }
    }

    return (
        <AuthContext.Provider value={{ user, loading, logout }}>
            {children}
        </AuthContext.Provider>
    );
}

export function useAuth() {
    const context = useContext(AuthContext);

    if (!context) {
        throw new Error("useAuth must be inside AuthProvider");
    }

    return context;
}