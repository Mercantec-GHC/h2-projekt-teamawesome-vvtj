import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { LoginForm } from "../../components/login-form";

export function Login() {
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [loading, setLoading] = useState(false);
  const [, setToken] = useState("");
  
  const [error, setError] = useState<string | null>(null);
  const navigate = useNavigate();

  const handleLogin = async (e: React.FormEvent) => {
    e.preventDefault();
    setLoading(true);
    try {
      const res = await fetch(`${import.meta.env.VITE_API_URL}/api/Auth/login`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ email, password }),
      });
       if (!res.ok) {
        const errData = await res.json().catch(() => null);
        setError(errData?.message || "Error logging in");
        return;
      }
      const data = await res.json();

      if (res.ok) {
        localStorage.setItem("token", data.token);
        setToken(data.token);
        navigate("/");
      }
    } catch (err) {
       setError("Can not connect to server, please try again later.");
      console.error(err);
    } finally {
      setLoading(false);
    }
  };

  return (
     <div className="bg-muted flex min-h-svh flex-col items-center justify-center gap-6 p-6 md:p-10">
      <div className="flex w-full max-w-sm flex-col gap-6">
        <a href="https://prod-novahotels-blazor-mercantec-tech.azurewebsites.net/" className="flex items-center gap-2 self-center font-medium">
          <div className="flex size-6 items-center justify-center">
          </div>
          Nova Hotels A/S
        </a>
         <LoginForm
      className="w-96"
      onSubmit={handleLogin}
      email={{
        value: email,
        onChange: (e: React.ChangeEvent<HTMLInputElement>) => setEmail(e.target.value),
      }}
      password={{
        value: password,
        onChange: (e: React.ChangeEvent<HTMLInputElement>) => setPassword(e.target.value),
      }}
      loading={loading}
    />
    {error && <div className="text-red-600 text-sm text-center">{error}</div>}
      </div>
    </div>
   
  );
}
