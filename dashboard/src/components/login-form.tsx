import { cn } from "@/lib/utils"
import { Button } from "@/components/ui/button"
import {
  Card,
  CardContent,
  CardDescription,
  CardHeader,
  CardTitle,
} from "@/components/ui/card"
import { Input } from "@/components/ui/input"
import { Label } from "@/components/ui/label"

interface LoginFormProps {
  className?: string
  username: { value: string; onChange: (e: React.ChangeEvent<HTMLInputElement>) => void }
  password: { value: string; onChange: (e: React.ChangeEvent<HTMLInputElement>) => void }
  loading: boolean
  onSubmit: (e: React.FormEvent) => void
}

export function LoginForm({ className, username, password, loading, onSubmit }: LoginFormProps) {
  return (
    <div
      className={cn(
        "flex flex-col gap-6",
        className
      )}
    >
      <Card>
        <CardHeader className="text-center">
          <CardTitle className="text-xl">Welcome back to hotels administration</CardTitle>
          <CardDescription>
            Login with your employee username and password below.
          </CardDescription>
        </CardHeader>
        <CardContent>
          <form onSubmit={onSubmit} className="grid gap-6">
            <div className="grid gap-6">
              <div className="grid gap-3">
                <Label htmlFor="email">Username</Label>
                <Input id="username" type="text" placeholder="Username" {...username} required />
              </div>
              <div className="grid gap-3">
                <div className="flex items-center">
                  <Label htmlFor="password">Password</Label>
                </div>
                <Input id="password" type="password" {...password} required />
              </div>
              <Button type="submit" className="w-full" disabled={loading}>
                {loading ? "Logging in..." : "Login"}
              </Button>
            </div>
          </form>
        </CardContent>
      </Card>

      <div className="text-muted-foreground *:[a]:hover:text-primary text-center text-xs text-balance *:[a]:underline *:[a]:underline-offset-4">
        By clicking continue, you agree to our <a href="#">Terms of Service</a> and <a href="#">Privacy Policy</a>.
      </div>
    </div>
  )
}
