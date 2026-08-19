import apiClient from './client';

export async function getLibrary() {
  const { data } = await apiClient.get('/library');
  return data;
}

export async function addToLibrary({ externalGameId, name, coverUrl, status }) {
  const { data } = await apiClient.post('/library', {
    externalGameId,
    name,
    coverUrl,
    status,
  });
  return data;
}

export async function updateLibraryStatus(id, status) {
  const { data } = await apiClient.put(`/library/${id}`, { status });
  return data;
}

export async function removeFromLibrary(id) {
  await apiClient.delete(`/library/${id}`);
}
