import {
    BrowserRouter,
    Routes,
    Route,
    Navigate,
} from "react-router-dom";

import { AuthProvider } from "./AuthContext";
import ProtectedRoute from "./ProtectedRoute";
import Login from "./Login";
import Callback from "./Callback";
import Dashboard from "./Dashboard";

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

                    <Route
                        path="*"
                        element={<Navigate to="/dashboard" replace />}
                    />
                </Routes>
            </BrowserRouter>
        </AuthProvider>
    );
}