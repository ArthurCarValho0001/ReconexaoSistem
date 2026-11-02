import { useEffect, useRef, useState, type FormEvent } from 'react';

type Session = { id: string; role: 'aluno' | 'admin' };
type Report = { id: string; author: { id: string; email: string | null }; createdAt: string; text: string };
type ReportPage = { items: Report[]; nextCursor: string | null };

const normalize = (text: string) => text.replace(/\r\n?/g, '\n');
const count = (text: string) => Array.from(normalize(text)).length;
const valid = (text: string) => {
  const normalized = normalize(text);
  return count(normalized) <= 1000 && !/^\p{White_Space}*$/u.test(normalized) && !/[\u0000\uD800-\uDFFF]/u.test(normalized);
};
const dateFormat = new Intl.DateTimeFormat('pt-BR', {
  timeZone: 'America/Sao_Paulo', dateStyle: 'short', timeStyle: 'medium',
});

function isPage(value: unknown): value is ReportPage {
  if (!value || typeof value !== 'object' || !('items' in value) || !Array.isArray(value.items) || value.items.length > 50 ||
      !('nextCursor' in value) || (value.nextCursor !== null && typeof value.nextCursor !== 'string')) return false;
  return value.items.every(item => item && typeof item.id === 'string' && typeof item.text === 'string' &&
    typeof item.createdAt === 'string' && Number.isFinite(Date.parse(item.createdAt)) && item.author &&
    typeof item.author.id === 'string' && (typeof item.author.email === 'string' || item.author.email === null));
}

export function ReportsPanel({ session, onSessionExpired }: { session: Session; onSessionExpired: () => void }) {
  return session.role === 'aluno' ? <ReportForm onSessionExpired={onSessionExpired} />
    : <AdminReports onSessionExpired={onSessionExpired} />;
}

function ReportForm({ onSessionExpired }: { onSessionExpired: () => void }) {
  const [text, setText] = useState('');
  const [busy, setBusy] = useState(false);
  const [error, setError] = useState('');
  const [message, setMessage] = useState('');
  const sending = useRef(false);
  const generation = useRef(0);
  const controller = useRef<AbortController | null>(null);

  useEffect(() => () => { generation.current++; controller.current?.abort(); }, []);

  async function submit(event: FormEvent<HTMLFormElement>) {
    event.preventDefault();
    if (sending.current || !valid(text)) return;
    sending.current = true;
    const version = generation.current;
    const request = new AbortController();
    controller.current = request;
    const timeout = setTimeout(() => request.abort(), 10000);
    const current = () => version === generation.current;
    setBusy(true); setError(''); setMessage('');
    try {
      const csrf = await fetch('/api/auth/csrf', { signal: request.signal, credentials: 'same-origin' });
      if (!csrf.ok) throw new Error('unavailable');
      const token = (await csrf.json()).token;
      if (!current()) return;
      const response = await fetch('/api/relatos', {
        method: 'POST', credentials: 'same-origin', signal: request.signal,
        headers: { 'Content-Type': 'application/json', 'X-CSRF-TOKEN': token },
        body: JSON.stringify({ text: normalize(text) }),
      });
      if (!current()) return;
      if (response.status === 401) { onSessionExpired(); return; }
      if (response.status !== 201) {
        setError(response.status === 403 ? 'Sua conta não tem permissão para enviar relatos.'
          : response.status === 400 ? 'Não foi possível validar o relato. Confira o texto e tente novamente.'
          : 'Não foi possível enviar. Seu texto foi preservado. Tente novamente.');
        return;
      }
      const confirmation: unknown = await response.json();
      if (!current()) return;
      if (!confirmation || typeof confirmation !== 'object' || !('status' in confirmation) || confirmation.status !== 'received')
        throw new Error('invalid confirmation');
      setText(''); setMessage('Relato enviado. Obrigado por compartilhar.');
    } catch {
      if (current()) setError('Não foi possível confirmar o envio. Seu texto foi preservado. Tente novamente.');
    } finally {
      clearTimeout(timeout);
      if (current()) { sending.current = false; setBusy(false); }
    }
  }

  return <div className="reports-panel" aria-labelledby="report-title">
    <h3 id="report-title">Compartilhe um relato</h3>
    <p id="report-help">Opcional. Apenas administradores podem consultar seu relato. Após o envio, ele não fica disponível no seu histórico e não gera pontos.</p>
    {message && <p role="status">{message}</p>}
    {error && <p role="alert" className="auth-error">{error}</p>}
    <form onSubmit={event => { void submit(event); }} aria-busy={busy}>
      <label htmlFor="report-text">Seu relato</label>
      <textarea id="report-text" rows={6} value={text} disabled={busy} autoComplete="off"
        aria-describedby="report-help report-count report-unicode" aria-invalid={text.length > 0 && !valid(text)}
        onChange={event => { setText(event.target.value); setError(''); setMessage(''); }} />
      <p id="report-count" aria-live="polite">{count(text)} de 1.000 caracteres</p>
      <p id="report-unicode" className="report-hint">Emoji simples conta como um caractere; acentos combinantes e sequências de emojis contam seus componentes. Quebras de linha contam como um. Texto vazio ou só com espaços não é enviado.</p>
      <button type="submit" disabled={busy || !valid(text)}>{busy ? 'Enviando relato…' : 'Enviar relato'}</button>
    </form>
  </div>;
}

function AdminReports({ onSessionExpired }: { onSessionExpired: () => void }) {
  const [items, setItems] = useState<Report[]>([]);
  const [cursor, setCursor] = useState<string | null>(null);
  const [previous, setPrevious] = useState<(string | null)[]>([]);
  const [nextCursor, setNextCursor] = useState<string | null>(null);
  const [reload, setReload] = useState(0);
  const [busy, setBusy] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    let active = true;
    const request = new AbortController();
    const timeout = setTimeout(() => request.abort(), 10000);
    setBusy(true); setError(''); setItems([]); setNextCursor(null);
    async function load() {
      try {
        const query = new URLSearchParams({ limit: '20' });
        if (cursor) query.set('cursor', cursor);
        const response = await fetch(`/api/admin/relatos?${query}`, { signal: request.signal, credentials: 'same-origin' });
        if (!active) return;
        if (response.status === 401) { onSessionExpired(); return; }
        if (response.status === 403) { setError('Sua conta não tem permissão para consultar relatos.'); return; }
        if (!response.ok) throw new Error('unavailable');
        const page: unknown = await response.json();
        if (!active) return;
        if (!isPage(page)) throw new Error('invalid response');
        setItems(page.items); setNextCursor(page.nextCursor);
      } catch { if (active) setError('Não foi possível consultar os relatos. Tente novamente.'); }
      finally { clearTimeout(timeout); if (active) setBusy(false); }
    }
    void load();
    return () => { active = false; clearTimeout(timeout); request.abort(); };
  }, [cursor, reload, onSessionExpired]);

  function refresh() { setCursor(null); setPrevious([]); setReload(value => value + 1); }
  function next() { if (nextCursor) { setPrevious(values => [...values, cursor]); setCursor(nextCursor); } }
  function back() {
    if (previous.length) { setCursor(previous[previous.length - 1]); setPrevious(values => values.slice(0, -1)); }
  }

  return <div className="reports-panel admin-reports" aria-labelledby="admin-reports-title" aria-busy={busy}>
    <h3 id="admin-reports-title">Relatos recebidos</h3>
    <p>Mais recentes primeiro. Datas e horários de Brasília.</p>
    <button type="button" onClick={refresh} disabled={busy}>Atualizar relatos</button>
    {busy ? <p role="status">Consultando relatos…</p> : error ? <p role="alert" className="auth-error">{error}</p>
      : items.length === 0 ? <p>Nenhum relato nesta página.</p> : <ol className="report-list">
        {items.map(item => <li key={item.id} className="report-entry" data-report-id={item.id}>
          <p>Autor: {item.author.email ?? 'Conta sem e-mail'}</p>
          <time dateTime={item.createdAt}>{dateFormat.format(new Date(item.createdAt))}</time>
          <p className="report-text">{item.text}</p>
        </li>)}
      </ol>}
    <nav aria-label="Páginas de relatos">
      <button type="button" disabled={busy || previous.length === 0} onClick={back}>Página anterior</button>
      <span>Página {previous.length + 1}</span>
      <button type="button" disabled={busy || !nextCursor} onClick={next}>Próxima página</button>
    </nav>
  </div>;
}
