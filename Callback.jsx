import { useEffect, useState } from "react";
import { useNavigate } from "react-router-dom";

export default function Callback() {
    const navigate = useNavigate();
    const [error, setError] = useState("");

    useEffect(() => {
        async function completeLogin() {
            try {
                const response = await fetch("/api/auth/callback", {
                    credentials: "include",
                });

                if (!response.ok) {
                    throw new Error("Authentication failed");
                }

                navigate("/dashboard", { replace: true });
            } catch {
                setError("Unable to complete sign-in.");
            }
        }

        completeLogin();
    }, [navigate]);

    return (
        <main>
            <h1>Completing sign-in</h1>
            {error ? <p role="alert">{error}</p> : <p>Please wait...</p>}
        </main>
    );
}