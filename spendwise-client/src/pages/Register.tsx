import { useState } from "react";
import axiosClient from "../api/axiosClient";

type RegisterProps = {
  onGoToLogin: () => void;
};

type RegisterErrors = {
  fullName?: string;
  email?: string;
  password?: string;
  monthlyIncome?: string;
  general?: string;
  success?: string;
};

function Register({ onGoToLogin }: RegisterProps) {
  const [fullName, setFullName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [monthlyIncome, setMonthlyIncome] = useState("");
  const [isLoading, setIsLoading] = useState(false);

  const [errors, setErrors] = useState<RegisterErrors>({});

  const validateForm = () => {
    const newErrors: RegisterErrors = {};
    const incomeValue = Number(monthlyIncome);

    if (!fullName.trim()) {
      newErrors.fullName = "Full name is required.";
    } else if (fullName.trim().length < 3) {
      newErrors.fullName = "Full name must have at least 3 characters.";
    }

    if (!email.trim()) {
      newErrors.email = "Email is required.";
    } else if (!/\S+@\S+\.\S+/.test(email)) {
      newErrors.email = "Please enter a valid email address.";
    }

    if (!password.trim()) {
      newErrors.password = "Password is required.";
    } else if (password.length < 6) {
      newErrors.password = "Password must have at least 6 characters.";
    }

    if (!monthlyIncome.trim()) {
      newErrors.monthlyIncome = "Monthly income is required.";
    } else if (Number.isNaN(incomeValue)) {
      newErrors.monthlyIncome = "Monthly income must be a number.";
    } else if (incomeValue <= 0) {
      newErrors.monthlyIncome = "Monthly income must be greater than 0.";
    }

    setErrors(newErrors);

    return Object.keys(newErrors).length === 0;
  };

  const clearFieldError = (field: keyof RegisterErrors) => {
    setErrors((prev) => ({
      ...prev,
      [field]: undefined,
      general: undefined,
      success: undefined,
    }));
  };

  const handleRegister = async () => {
    if (!validateForm()) {
      return;
    }

    try {
      setIsLoading(true);
      setErrors({});

      const response = await axiosClient.post("/Auth/register", {
        fullName,
        email,
        password,
        monthlyIncome: Number(monthlyIncome),
      });

      localStorage.setItem("token", response.data.token);

      setErrors({
        success: "Account created successfully. You can now log in.",
      });

      setTimeout(() => {
        onGoToLogin();
      }, 800);
    } catch (error: any) {
      console.error(error.response?.data || error);

      const data = error.response?.data;

      if (Array.isArray(data)) {
        setErrors({
          general: data.map((e) => e.description).join(" "),
        });
      } else if (typeof data === "string") {
        setErrors({
          general: data,
        });
      } else {
        setErrors({
          general: "Could not create account. Please try again.",
        });
      }
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="page">
      <div className="card">
        <h1>SpendWise</h1>
        <h2>Create account</h2>
    

        {errors.general && <p className="form-error">{errors.general}</p>}
        {errors.success && <p className="form-success">{errors.success}</p>}

        <div className="form-field">
          <input
            type="text"
            placeholder="Full name"
            value={fullName}
            maxLength={100}
            onChange={(e) => {
              setFullName(e.target.value);
              clearFieldError("fullName");
            }}
          />
          {errors.fullName && <span className="field-error">{errors.fullName}</span>}
        </div>

        <div className="form-field">
          <input
            type="email"
            placeholder="Email"
            value={email}
            maxLength={100}
            onChange={(e) => {
              setEmail(e.target.value);
              clearFieldError("email");
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
              clearFieldError("password");
            }}
          />
          {errors.password && <span className="field-error">{errors.password}</span>}
        </div>

        <div className="form-field">
          <input
            type="text"
            inputMode="decimal"
            placeholder="Monthly income"
            value={monthlyIncome}
            onChange={(e) => {
              setMonthlyIncome(e.target.value);
              clearFieldError("monthlyIncome");
            }}
          />
          {errors.monthlyIncome && (
            <span className="field-error">{errors.monthlyIncome}</span>
          )}
        </div>

        <button type="button" onClick={handleRegister} disabled={isLoading}>
          {isLoading ? "Creating account..." : "Create account"}
        </button>

        <p className="switch-text">
          Already have an account?{" "}
          <button type="button" className="link-button" onClick={onGoToLogin}>
            Login
          </button>
        </p>
      </div>
    </div>
  );
}

export default Register;