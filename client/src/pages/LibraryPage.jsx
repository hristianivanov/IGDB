import { Link } from 'react-router-dom';
import { Trash2 } from 'lucide-react';
import { useLibrary } from '../context/LibraryContext';
import StatusSelect from '../components/StatusSelect';

export default function LibraryPage() {
  const { savedGames, loading, error, updateStatus, remove } = useLibrary();

  return (
    <div className="mx-auto flex max-w-4xl flex-col gap-4 px-4 py-8">
      <h1 className="text-xl font-semibold text-white">Your library</h1>

      {error && <p className="text-sm text-red-400">{error}</p>}
      {!error && loading && <p className="text-sm text-slate-400">Loading…</p>}
      {!error && !loading && savedGames.length === 0 && (
        <p className="text-sm text-slate-400">
          Nothing saved yet.{' '}
          <Link to="/" className="text-violet-400 hover:underline">
            Browse games
          </Link>{' '}
          to add some.
        </p>
      )}

      <ul className="flex flex-col divide-y divide-white/10">
        {savedGames.map((game) => (
          <li key={game.id} className="flex items-center gap-4 py-3">
            <Link to={`/games/${game.igdbGameId}`} className="shrink-0">
              <div className="h-20 w-16 overflow-hidden rounded bg-slate-800">
                {game.coverUrl && (
                  <img src={game.coverUrl} alt={game.name} className="h-full w-full object-cover" />
                )}
              </div>
            </Link>

            <Link to={`/games/${game.igdbGameId}`} className="flex-1 text-sm font-medium text-slate-100 hover:text-violet-300">
              {game.name}
            </Link>

            <StatusSelect value={game.status} onChange={(status) => updateStatus(game.id, status)} />

            <button
              type="button"
              onClick={() => remove(game.id)}
              title="Remove from library"
              className="rounded-md p-1.5 text-slate-500 transition hover:bg-red-500/10 hover:text-red-400"
            >
              <Trash2 size={16} />
            </button>
          </li>
        ))}
      </ul>
    </div>
  );
}
