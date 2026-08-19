import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { addToLibrary, getLibrary, removeFromLibrary, updateLibraryStatus } from '../api/library';

const LibraryContext = createContext(null);

export function LibraryProvider({ children }) {
  const [savedGames, setSavedGames] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  const refresh = useCallback(async () => {
    setLoading(true);
    setError(null);
    try {
      const games = await getLibrary();
      setSavedGames(games);
    } catch {
      setError('Could not load your library. Is the API running?');
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => {
    refresh();
  }, [refresh]);

  const add = useCallback(async (game, status = 'Wishlist') => {
    const saved = await addToLibrary({
      externalGameId: game.id,
      name: game.name,
      coverUrl: game.coverUrl,
      status,
    });
    setSavedGames((current) => [saved, ...current]);
  }, []);

  const updateStatus = useCallback(async (id, status) => {
    const updated = await updateLibraryStatus(id, status);
    setSavedGames((current) => current.map((g) => (g.id === id ? updated : g)));
  }, []);

  const remove = useCallback(async (id) => {
    await removeFromLibrary(id);
    setSavedGames((current) => current.filter((g) => g.id !== id));
  }, []);

  const savedByExternalId = useMemo(() => {
    const map = new Map();
    for (const game of savedGames) map.set(game.externalGameId, game);
    return map;
  }, [savedGames]);

  const value = useMemo(
    () => ({ savedGames, loading, error, refresh, add, updateStatus, remove, savedByExternalId }),
    [savedGames, loading, error, refresh, add, updateStatus, remove, savedByExternalId],
  );

  return <LibraryContext.Provider value={value}>{children}</LibraryContext.Provider>;
}

export function useLibrary() {
  const context = useContext(LibraryContext);
  if (!context) throw new Error('useLibrary must be used within a LibraryProvider');
  return context;
}
