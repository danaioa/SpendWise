import type { ReactNode } from "react";
import { NavLink, useNavigate } from "react-router-dom";
import {
  FiHome,
  FiCreditCard,
  FiPieChart,
  FiRepeat,
  FiCamera,
  FiLogOut,
  FiDollarSign,
} from "react-icons/fi";

type AppLayoutProps = {
  children: ReactNode;
};

function AppLayout({ children }: AppLayoutProps) {
  const navigate = useNavigate();

  const handleLogout = () => {
    localStorage.removeItem("token");
    localStorage.removeItem("email");
    localStorage.removeItem("fullName");
    navigate("/");
  };

  return (
    <div className="app-layout">
      <aside className="sidebar">
        <div className="sidebar-brand">
          <div className="sidebar-logo">S</div>
          <div>
            <h2>SpendWise</h2>
            <p>Finance app</p>
          </div>
        </div>

        <nav className="sidebar-nav">
          <NavLink to="/dashboard">
            <FiHome /> Dashboard
          </NavLink>

          <NavLink to="/transactions">
            <FiCreditCard /> Transactions
          </NavLink>

          <NavLink to="/budgets">
            <FiPieChart /> Budgets
          </NavLink>

          <NavLink to="/recurring">
            <FiRepeat /> Recurring
          </NavLink>

          <NavLink to="/scan">
            <FiCamera /> Scan Product
          </NavLink>

          <NavLink to="/savings">
        <FiDollarSign /> Savings
      </NavLink>

          
        </nav>

        <button className="logout-btn" onClick={handleLogout}>
          <FiLogOut /> Logout
        </button>
      </aside>

      <main className="main-content">{children}</main>
    </div>
  );
}

export default AppLayout;