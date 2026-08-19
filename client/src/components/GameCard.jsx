import { Link } from 'react-router-dom';
import { Bookmark, BookmarkCheck, Star } from 'lucide-react';
import { useLibrary } from '../context/LibraryContext';

function releaseYear(isoDate) {
  return isoDate ? new Date(isoDate).getFullYear() : '—';
}

export default function GameCard({ game }) {
  const { savedByExternalId, add } = useLibrary();
  const saved = savedByExternalId.get(game.id);

  const handleSave = async (event) => {
    event.preventDefault();
    if (saved) return;
    await add(game);
  };

  return (
    <Link
      to={`/games/${game.id}`}
      className="group relative flex flex-col overflow-hidden rounded-lg border border-white/10 bg-white/5 transition hover:border-violet-400/60 hover:bg-white/10"
    >
      <div className="aspect-[3/4] w-full overflow-hidden bg-slate-800">
        {game.coverUrl ? (
          <img
            src={game.coverUrl}
            alt={game.name}
            loading="lazy"
            className="h-full w-full object-cover transition duration-300 group-hover:scale-105"
          />
        ) : (
          <div className="flex h-full items-center justify-center text-sm text-slate-500">No cover</div>
        )}
      </div>

      <button
        type="button"
        onClick={handleSave}
        title={saved ? 'In your library' : 'Add to library'}
        className="absolute right-2 top-2 rounded-full bg-black/60 p-1.5 text-white backdrop-blur transition hover:bg-black/80"
      >
        {saved ? <BookmarkCheck size={16} className="text-violet-400" /> : <Bookmark size={16} />}
      </button>

      <div className="flex flex-1 flex-col gap-1 p-3">
        <h3 className="line-clamp-2 text-sm font-medium text-slate-100">{game.name}</h3>
        <div className="mt-auto flex items-center justify-between text-xs text-slate-400">
          <span>{releaseYear(game.firstReleaseDate)}</span>
          {typeof game.rating === 'number' && (
            <span className="flex items-center gap-1 text-amber-400">
              <Star size={12} fill="currentColor" />
              {Math.round(game.rating)}
            </span>
          )}
        </div>
      </div>
    </Link>
  );
}
