import apiClient from './client';

export async function searchGames(query, { limit = 20, offset = 0 } = {}) {
  const { data } = await apiClient.get('/games/search', {
    params: { query, limit, offset },
  });
  return data;
}

export async function getPopularGames(limit = 20) {
  const { data } = await apiClient.get('/games/popular', { params: { limit } });
  return data;
}

export async function getGameById(id) {
  const { data } = await apiClient.get(`/games/${id}`);
  return data;
}
