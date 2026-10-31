import { useEffect, useState } from 'react';
import { createRoot } from 'react-dom/client';
import './style.css';

type Connection = 'loading' | 'ready' | 'error';

function App() {
  const [connection, setConnection] = useState<Connection>('loading');
  const [attempt, setAttempt] = useState(0);

  useEffect(() => {
    const controller = new AbortController();
    let active = true;
    const timeout = setTimeout(() => controller.abort(), 8000);
    setConnection('loading');
    async function connect() {
      try {
        const response = await fetch('/api/foundation', { signal: controller.signal });
        if (!response.ok) throw new Error('unavailable');
        const data: unknown = await response.json();
        if (typeof data !== 'object' || data === null || !('status' in data) || data.status !== 'ready')
          throw new Error('invalid response');
        if (active) setConnection('ready');
      } catch {
        if (active) setConnection('error');
      } finally {
        clearTimeout(timeout);
      }
    }
    void connect();
    return () => { active = false; clearTimeout(timeout); controller.abort(); };
  }, [attempt]);

  return <main>
    <header><span className="brand" aria-hidden="true">R</span><span>RECONEXÃO</span></header>
    <section aria-labelledby="welcome">
      <p className="eyebrow">UM NOVO COMEÇO</p>
      <h1 id="welcome">Todo caminho começa<br />com uma conexão.</h1>
      <p className="intro">Estamos preparando este espaço para sua jornada.</p>
      <div className="connection" aria-live="polite" aria-busy={connection === 'loading'}>
        <span className={`dot ${connection}`} aria-hidden="true" />
        <p>{connection === 'loading' ? 'Conectando…' : connection === 'ready'
          ? 'Conexão estabelecida.' : 'Não foi possível conectar. Tente novamente em instantes.'}</p>
      </div>
      <button type="button" disabled={connection === 'loading'} onClick={() => setAttempt(value => value + 1)}>
        {connection === 'loading' ? 'Aguarde…' : 'Verificar conexão'}
      </button>
    </section>
    <footer>Reconexão · Espaço em preparação</footer>
  </main>;
}

createRoot(document.getElementById('root')!).render(<App />);
