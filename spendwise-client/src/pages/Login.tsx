import { useState } from "react";
import axiosClient from "../api/axiosClient";

type LoginProps = {
  onGoToRegister: () => void;
  onLoginSuccess: () => void;
};

type LoginErrors = {
  email?: string;
  password?: string;
  general?: string;
};

function Login({ onGoToRegister, onLoginSuccess }: LoginProps) {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");

  const [errors, setErrors] = useState<LoginErrors>({});
  const [isLoading, setIsLoading] = useState(false);

  const validateForm = () => {
    const newErrors: LoginErrors = {};

    if (!email.trim()) {
      newErrors.email = "Email is required.";
    } else if (!/\S+@\S+\.\S+/.test(email)) {
      newErrors.email = "Please enter a valid email address.";
    }

    if (!password.trim()) {
      newErrors.password = "Password is required.";
    }

    setErrors(newErrors);

    return Object.keys(newErrors).length === 0;
  };

  const handleLogin = async () => {
    if (!validateForm()) {
      return;
    }

    try {
      setIsLoading(true);
      setErrors({});

      const response = await axiosClient.post("/Auth/login", {
        email,
        password,
      });

      localStorage.setItem("token", response.data.token);
      localStorage.setItem("email", response.data.email);
      localStorage.setItem("fullName", response.data.fullName);

      onLoginSuccess();
    } catch (error) {
      console.error(error);

      setErrors({
        general: "Invalid email or password.",
      });
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="page">
      <div className="card">
        <h1>SpendWise</h1>
        <h2>Login</h2>

        {errors.general && <p className="form-error">{errors.general}</p>}

        <div className="form-field">
          <input
            type="email"
            placeholder="Email"
            value={email}
            maxLength={100}
            onChange={(e) => {
              setEmail(e.target.value);
              setErrors((prev) => ({ ...prev, email: undefined, general: undefined }));
            }}
          />
          {errors.email && <span className="field-error">{errors.email}</span>}
        </div>

        <div className="form-field">
          <input
            type="password"
            placeholder="Password"
            value={password}
            maxLength={100}
            onChange={(e) => {
              setPassword(e.target.value);
              setErrors((prev) => ({ ...prev, password: undefined, general: undefined }));
            }}
          />
          {errors.password && <span className="field-error">{errors.password}</span>}
        </div>

        <button type="button" onClick={handleLogin} disabled={isLoading}>
          {isLoading ? "Signing in..." : "Login"}
        </button>

        <p className="switch-text">
          Don&apos;t have an account?{" "}
          <button type="button" className="link-button" onClick={onGoToRegister}>
            Create account
          </button>
        </p>
      </div>
    </div>
  );
}

export default Login;