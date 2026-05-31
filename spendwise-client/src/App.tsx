import type { ReactNode } from "react";
import { Navigate, Route, Routes } from "react-router-dom";
import Login from "./pages/Login";
import Register from "./pages/Register";
import Dashboard from "./pages/Dashboard";
import AppLayout from "./components/AppLayout";
import TransactionsPage from "./pages/TransactionsPage";
import BudgetsPage from "./pages/BudgetsPage";
import RecurringPage from "./pages/RecurringPage";
import BarcodeScanner from "./pages/BarcodeScanner";
import ProfilePage from "./pages/ProfilePage";
import LandingPage from "./pages/LandingPage";
import SavingsPage from "./pages/SavingsPage";
import "./App.css";

function ProtectedRoute({ children }: { children: ReactNode }) {
  const token = localStorage.getItem("token");

  if (!token) {
    return <Navigate to="/login" replace />;
  }

  return children;
}

function App() {
  return (
    <Routes>
      <Route path="/" element={<LandingPage />} />

      <Route
        path="/login"
        element={
          <Login
            onGoToRegister={() => (window.location.href = "/register")}
            onLoginSuccess={() => (window.location.href = "/dashboard")}
          />
        }
      />

      <Route
        path="/register"
        element={<Register onGoToLogin={() => (window.location.href = "/login")} />}
      />

      <Route
        path="/dashboard"
        element={
          <ProtectedRoute>
            <AppLayout>
              <Dashboard />
            </AppLayout>
          </ProtectedRoute>
        }
      />

      <Route
        path="/transactions"
        element={
          <ProtectedRoute>
            <AppLayout>
              <TransactionsPage />
            </AppLayout>
          </ProtectedRoute>
        }
      />

      <Route
  path="/savings"
  element={
    <ProtectedRoute>
      <AppLayout>
        <SavingsPage />
      </AppLayout>
    </ProtectedRoute>
  }
/>

      <Route
        path="/budgets"
        element={
          <ProtectedRoute>
            <AppLayout>
              <BudgetsPage />
            </AppLayout>
          </ProtectedRoute>
        }
      />

      <Route
        path="/recurring"
        element={
          <ProtectedRoute>
            <AppLayout>
              <RecurringPage />
            </AppLayout>
          </ProtectedRoute>
        }
      />

      <Route
        path="/scan"
        element={
          <ProtectedRoute>
            <AppLayout>
              <BarcodeScanner />
            </AppLayout>
          </ProtectedRoute>
        }
      />

      <Route
        path="/profile"
        element={
          <ProtectedRoute>
            <AppLayout>
              <ProfilePage />
            </AppLayout>
          </ProtectedRoute>
        }
      />

      <Route path="*" element={<Navigate to="/" replace />} />
    </Routes>
  );
}

export default App;