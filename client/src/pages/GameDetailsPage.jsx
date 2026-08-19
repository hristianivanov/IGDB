import { useEffect, useState } from 'react';
import { Link, useParams } from 'react-router-dom';
import { ArrowLeft, Star } from 'lucide-react';
import { getGameById } from '../api/games';
import { useLibrary } from '../context/LibraryContext';
import StatusSelect from '../components/StatusSelect';

export default function GameDetailsPage() {
  const { id } = useParams();
  const [game, setGame] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const { savedByExternalId, add, updateStatus } = useLibrary();

  useEffect(() => {
    let cancelled = false;

    async function load() {
      setLoading(true);
      setError(null);
      try {
        const result = await getGameById(id);
        if (!cancelled) setGame(result);
      } catch {
        if (!cancelled) setError('Could not load this game.');
      } finally {
        if (!cancelled) setLoading(false);
      }
    }

    load();
    return () => {
      cancelled = true;
    };
  }, [id]);

  if (loading) return <p className="px-4 py-8 text-sm text-slate-400">Loading…</p>;
  if (error || !game) return <p className="px-4 py-8 text-sm text-red-400">{error ?? 'Game not found.'}</p>;

  const saved = savedByExternalId.get(game.id);

  return (
    <div className="mx-auto flex max-w-5xl flex-col gap-6 px-4 py-8">
      <Link to="/" className="flex w-fit items-center gap-1.5 text-sm text-slate-400 hover:text-white">
        <ArrowLeft size={16} />
        Back to browse
      </Link>

      <div className="flex flex-col gap-6 sm:flex-row">
        <div className="w-full max-w-[240px] shrink-0 overflow-hidden rounded-lg bg-slate-800">
          {game.coverUrl ? (
            <img src={game.coverUrl} alt={game.name} className="h-full w-full object-cover" />
          ) : (
            <div className="flex aspect-[3/4] items-center justify-center text-sm text-slate-500">No cover</div>
          )}
        </div>

        <div className="flex flex-1 flex-col gap-3">
          <h1 className="text-2xl font-semibold text-white">{game.name}</h1>

          <div className="flex flex-wrap items-center gap-3 text-sm text-slate-400">
            {game.firstReleaseDate && <span>{new Date(game.firstReleaseDate).getFullYear()}</span>}
            {typeof game.rating === 'number' && (
              <span className="flex items-center gap-1 text-amber-400">
                <Star size={14} fill="currentColor" />
                {Math.round(game.rating)} / 100
                {game.ratingCount ? ` (${game.ratingCount} ratings)` : ''}
              </span>
            )}
          </div>

          {game.genres?.length > 0 && (
            <div className="flex flex-wrap gap-1.5">
              {game.genres.map((genre) => (
                <span key={genre} className="rounded-full bg-violet-500/15 px-2.5 py-0.5 text-xs text-violet-300">
                  {genre}
                </span>
              ))}
            </div>
          )}

          <div className="mt-2">
            {saved ? (
              <StatusSelect value={saved.status} onChange={(status) => updateStatus(saved.id, status)} />
            ) : (
              <button
                type="button"
                onClick={() => add(game)}
                className="rounded-md bg-violet-500 px-4 py-1.5 text-sm font-medium text-white transition hover:bg-violet-400"
              >
                Add to library
              </button>
            )}
          </div>

          {game.summary && <p className="text-sm leading-relaxed text-slate-300">{game.summary}</p>}

          {game.platforms?.length > 0 && (
            <p className="text-sm text-slate-400">
              <span className="text-slate-500">Platforms: </span>
              {game.platforms.join(', ')}
            </p>
          )}

          {game.involvedCompanies?.length > 0 && (
            <p className="text-sm text-slate-400">
              <span className="text-slate-500">Developers/Publishers: </span>
              {game.involvedCompanies.join(', ')}
            </p>
          )}
        </div>
      </div>

      {game.screenshots?.length > 0 && (
        <div className="flex flex-col gap-2">
          <h2 className="text-sm font-medium uppercase tracking-wide text-slate-400">Screenshots</h2>
          <div className="grid grid-cols-2 gap-3 sm:grid-cols-3">
            {game.screenshots.map((url) => (
              <img key={url} src={url} alt="" loading="lazy" className="aspect-video w-full rounded-md object-cover" />
            ))}
          </div>
        </div>
      )}
    </div>
  );
}
