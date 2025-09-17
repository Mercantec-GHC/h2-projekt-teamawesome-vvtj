import { useState } from 'react'
import { useNavigate } from 'react-router-dom' //
import { Button } from '@/components/ui/button'
import { Input } from '@/components/ui/input'

export function Login() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [loading, setLoading] = useState(false)
  const [token, setToken] = useState('') 
  const navigate = useNavigate()

  const handleLogin = async (e: React.FormEvent) => {
  e.preventDefault()
  setLoading(true)
  try {
    const res =await fetch(`${import.meta.env.VITE_API_URL}/api/Auth/login`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify({ email, password }),
    })
    const data = await res.json()
    setToken(data.token)
    localStorage.setItem('token', data.token)

    if (res.ok) {
      navigate('/')
    }
  } catch (err) {
    console.error(err)
  } finally {
    setLoading(false)
  }
}
  return (
    <div className="flex min-h-screen items-center justify-center">
      <form onSubmit={handleLogin} className="flex flex-col gap-4 w-80">
        <h2 className="text-2xl font-bold text-center">Login</h2>
        <Input
          type="email"
          placeholder="Email"
          value={email}
          onChange={e => setEmail(e.target.value)}
          required
        />
        <Input
          type="password"
          placeholder="Password"
          value={password}
          onChange={e => setPassword(e.target.value)}
          required
        />
        <Button type="submit" disabled={loading}>
          {loading ? 'Logging in...' : 'Login'}
        </Button>
      </form>

      {token && <p className="mt-4 break-all">Token: {token}</p>}
    </div>
  )
}
