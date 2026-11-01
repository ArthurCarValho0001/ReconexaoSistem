import { useEffect, useRef, useState, type FormEvent } from 'react';

type Session = { id: string; email: string; role: 'aluno' | 'admin' };
type View = 'login' | 'forgot' | 'reset';

async function request(path: string, body?: object) {
  const controller = new AbortController();
  const timeout = setTimeout(() => controller.abort(), 10000);
  try {
    const headers: Record<string, string> = {};
    if (body !== undefined) {
      const csrf = await fetch('/api/auth/csrf', { signal: controller.signal, credentials: 'same-origin' });
      if (!csrf.ok) throw new Error('Serviço indisponível. Tente novamente.');
      headers['X-CSRF-TOKEN'] = (await csrf.json()).token;
      headers['Content-Type'] = 'application/json';
    }
    return await fetch(path, {
      method: body === undefined ? 'GET' : 'POST', headers,
      body: body === undefined ? undefined : JSON.stringify(body),
      credentials: 'same-origin', signal: controller.signal,
    });
  } finally { clearTimeout(timeout); }
}

function failure(status: number, view: View) {
  if (status === 429) return 'Muitas tentativas. Aguarde um minuto e tente novamente.';
  if (status >= 500) return 'Serviço temporariamente indisponível. Tente novamente.';
  if (view === 'reset') return 'Código inválido, expirado ou já utilizado. Confira também os requisitos da senha.';
  if (status === 400) return 'Não foi possível validar o pedido. Atualize a página e tente novamente.';
  return 'E-mail ou senha inválidos, ou acesso temporariamente bloqueado.';
}

export function AuthPanel() {
  const [session, setSession] = useState<Session | null>(null);
  const [view, setView] = useState<View>('login');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [token, setToken] = useState('');
  const [loading, setLoading] = useState(true);
  const [busy, setBusy] = useState(false);
  const [message, setMessage] = useState('');
  const [error, setError] = useState('');
  const generation = useRef(0);
  const mutating = useRef(false);
  const authenticated = useRef(false);

  useEffect(() => {
    let active = true;
    let running = false;
    async function refresh() {
      if (running || mutating.current) return;
      running = true;
      const version = generation.current;
      try {
        const response = await request('/api/auth/session');
        if (!active || version !== generation.current) return;
        if (response.ok) {
          const current: Session = await response.json();
          if (!active || version !== generation.current) return;
          authenticated.current = true;
          setSession(current);
        } else if (response.status === 401) {
          setSession(null);
          if (authenticated.current) setMessage('Sua sessão terminou. Entre novamente.');
          authenticated.current = false;
        } else setError('Não foi possível consultar sua sessão. Tente novamente.');
      } catch { if (active && version === generation.current) setError('Não foi possível consultar sua sessão. Verifique a conexão.'); }
      finally { running = false; if (active) setLoading(false); }
    }
    void refresh();
    const timer = setInterval(() => { if (authenticated.current) void refresh(); }, 30000);
    const onFocus = () => { void refresh(); };
    window.addEventListener('focus', onFocus);
    return () => { active = false; clearInterval(timer); window.removeEventListener('focus', onFocus); };
  }, []);

  function changeView(next: View) {
    setView(next); setError(''); setMessage(''); setPassword(''); setToken('');
  }

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (busy) return;
    generation.current++; mutating.current = true;
    setBusy(true); setError(''); setMessage('');
    try {
      const route = view === 'login' ? 'login' : view === 'forgot' ? 'forgot-password' : 'reset-password';
      const body = view === 'forgot' ? { email } : view === 'reset' ? { email, token, password } : { email, password };
      const response = await request(`/api/auth/${route}`, body);
      if (!response.ok) { setError(failure(response.status, view)); return; }
      setPassword('');
      if (view === 'login') {
        authenticated.current = true;
        setSession(await response.json()); setMessage('Você entrou com segurança.');
      } else if (view === 'forgot') {
        setMessage('Pedido recebido. Se houver uma conta para esse e-mail, você receberá as instruções de recuperação.');
      } else {
        authenticated.current = false;
        setToken(''); setSession(null); setView('login');
        setMessage('Senha redefinida. Entre novamente com sua nova senha.');
      }
    } catch { setError('Não foi possível concluir. Verifique a conexão e tente novamente.'); }
    finally { mutating.current = false; setBusy(false); }
  }

  async function logout() {
    generation.current++; mutating.current = true;
    setBusy(true); setError(''); setMessage('');
    try {
      const response = await request('/api/auth/logout', {});
      if (response.ok || response.status === 401) {
        authenticated.current = false;
        setSession(null); setPassword(''); setToken(''); setMessage('Você saiu da sua conta.');
      } else setError(failure(response.status, 'login'));
    } catch { setError('Não foi possível sair. Verifique a conexão e tente novamente.'); }
    finally { mutating.current = false; setBusy(false); }
  }

  return <div className="auth-panel" aria-labelledby="account-title" aria-busy={loading || busy}>
    <h2 id="account-title">{session ? 'Sua conta' : view === 'login' ? 'Acesse sua conta' : view === 'forgot' ? 'Recuperar senha' : 'Redefinir senha'}</h2>
    {message && <p role="status">{message}</p>}
    {error && <p className="auth-error" role="alert">{error}</p>}
    {loading ? <p role="status">Consultando sessão…</p> : session ? <>
      <p>{session.email}</p>
      <p>Perfil: {session.role === 'admin' ? 'Administrador' : 'Aluno'}</p>
      <button type="button" disabled={busy} onClick={() => { void logout(); }}>Sair</button>
    </> : <>
      <form onSubmit={event => { void submit(event); }}>
        <label htmlFor="auth-email">E-mail</label>
        <input id="auth-email" type="email" autoComplete="username" required maxLength={254}
          value={email} onChange={event => setEmail(event.target.value)} disabled={busy} />
        {view === 'reset' && <>
          <label htmlFor="auth-token">Código recebido por e-mail</label>
          <input id="auth-token" type="password" autoComplete="off" required maxLength={4096}
            value={token} onChange={event => setToken(event.target.value)} disabled={busy} />
        </>}
        {view !== 'forgot' && <>
          <label htmlFor="auth-password">{view === 'reset' ? 'Nova senha' : 'Senha'}</label>
          <input id="auth-password" type="password" required maxLength={256} minLength={view === 'reset' ? 12 : undefined}
            autoComplete={view === 'reset' ? 'new-password' : 'current-password'}
            aria-describedby={view === 'reset' ? 'password-help' : undefined}
            value={password} onChange={event => setPassword(event.target.value)} disabled={busy} />
          {view === 'reset' && <p id="password-help">Use pelo menos 12 caracteres, com maiúscula, minúscula, número e símbolo.</p>}
        </>}
        <button disabled={busy} type="submit">{busy ? 'Aguarde…' : view === 'login' ? 'Entrar' : view === 'forgot' ? 'Enviar instruções' : 'Redefinir senha'}</button>
      </form>
      <nav aria-label="Acesso à conta">
        {view !== 'login' && <button type="button" disabled={busy} onClick={() => changeView('login')}>Voltar para entrar</button>}
        {view !== 'forgot' && <button type="button" disabled={busy} onClick={() => changeView('forgot')}>Esqueci minha senha</button>}
        {view !== 'reset' && <button type="button" disabled={busy} onClick={() => changeView('reset')}>Já tenho um código</button>}
      </nav>
    </>}
  </div>;
}
