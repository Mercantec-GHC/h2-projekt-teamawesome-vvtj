import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { LoginForm } from "../../components/login-form";
import { jwtDecode } from "jwt-decode";
import type { NotificationSubscriptionDto } from "../../types/NotificationDTO";

function urlBase64ToUint8Array(base64String: string) {
  const padding = "=".repeat((4 - (base64String.length % 4)) % 4);
  const base64 = (base64String + padding).replace(/-/g, "+").replace(/_/g, "/");
  const rawData = atob(base64);
  return Uint8Array.from([...rawData].map((char) => char.charCodeAt(0)));
}

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
    setError(null);

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
      localStorage.setItem("token", data.token);
      setToken(data.token);

      const decoded: { role?: string } = jwtDecode(data.token);

      if (decoded.role === "Admin") {
        try {
          if ("Notification" in window && "serviceWorker" in navigator) {
            let permission = Notification.permission;
            if (permission === "default") {
              permission = await Notification.requestPermission();
            }

            if (permission === "granted") {
              const reg = await navigator.serviceWorker.ready;
              const pushSub = await reg.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: urlBase64ToUint8Array(
                  import.meta.env.VITE_VAPID_PUBLIC_KEY
                ),
              });

              const subscription: NotificationSubscriptionDto = {
                Url: pushSub.endpoint,
                P256dh: btoa(
                  String.fromCharCode(
                    ...new Uint8Array(pushSub.getKey("p256dh")!)
                  )
                ),
                Auth: btoa(
                  String.fromCharCode(
                    ...new Uint8Array(pushSub.getKey("auth")!)
                  )
                ),
              };

              await fetch(`${import.meta.env.VITE_API_URL}/api/Notifications/subscribe`, {
                method: "POST",
                headers: {
                  "Content-Type": "application/json",
                  Authorization: `Bearer ${data.token}`,
                },
                body: JSON.stringify(subscription),
              });
            }
          }
        } catch (err) {
          console.error("Push subscription failed:", err);
        }
      }

      navigate("/"); 
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
        <a
          href="https://prod-novahotels-blazor-mercantec-tech.azurewebsites.net/"
          className="flex items-center gap-2 self-center font-medium"
        >
          <div className="flex size-6 items-center justify-center"></div>
          Nova Hotels A/S
        </a>
        <LoginForm
          className="w-96"
          onSubmit={handleLogin}
          email={{
            value: email,
            onChange: (e: React.ChangeEvent<HTMLInputElement>) =>
              setEmail(e.target.value),
          }}
          password={{
            value: password,
            onChange: (e: React.ChangeEvent<HTMLInputElement>) =>
              setPassword(e.target.value),
          }}
          loading={loading}
        />
        {error && <div className="text-red-600 text-sm text-center">{error}</div>}
      </div>
    </div>
  );
}
