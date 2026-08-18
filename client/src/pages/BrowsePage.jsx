import { useEffect, useState } from 'react';
import { Search } from 'lucide-react';
import { getPopularGames, searchGames } from '../api/games';
import { useDebounce } from '../hooks/useDebounce';
import GameCard from '../components/GameCard';

export default function BrowsePage() {
  const [query, setQuery] = useState('');
  const debouncedQuery = useDebounce(query);
  const [games, setGames] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    let cancelled = false;

    async function load() {
      setLoading(true);
      setError(null);
      try {
        const results = debouncedQuery.trim()
          ? await searchGames(debouncedQuery.trim())
          : await getPopularGames();
        if (!cancelled) setGames(results);
      } catch {
        if (!cancelled) setError('Could not reach the API. Is it running?');
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    load();
    return () => {
      cancelled = true;
    };
  }, [debouncedQuery]);

  return (
    <div className="mx-auto flex max-w-6xl flex-col gap-6 px-4 py-8">
      <div className="relative">
        <Search className="pointer-events-none absolute left-3 top-1/2 -translate-y-1/2 text-slate-500" size={18} />
        <input
          value={query}
          onChange={(event) => setQuery(event.target.value)}
          placeholder="Search for a game…"
          className="w-full rounded-lg border border-white/10 bg-white/5 py-2.5 pl-10 pr-4 text-slate-100 placeholder:text-slate-500 focus:border-violet-400 focus:outline-none"
        />
      </div>

      <h2 className="text-sm font-medium uppercase tracking-wide text-slate-400">
        {debouncedQuery.trim() ? `Results for "${debouncedQuery.trim()}"` : 'Popular games'}
      </h2>

      {error && <p className="text-sm text-red-400">{error}</p>}
      {!error && loading && <p className="text-sm text-slate-400">Loading…</p>}
      {!error && !loading && games.length === 0 && (
        <p className="text-sm text-slate-400">No games found.</p>
      )}

      <div className="grid grid-cols-2 gap-4 sm:grid-cols-3 md:grid-cols-4 lg:grid-cols-5">
        {games.map((game) => (
          <GameCard key={game.id} game={game} />
        ))}
      </div>
    </div>
  );
}
