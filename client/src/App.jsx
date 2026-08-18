import { Route, Routes } from 'react-router-dom';
import Navbar from './components/Navbar';
import BrowsePage from './pages/BrowsePage';
import GameDetailsPage from './pages/GameDetailsPage';
import LibraryPage from './pages/LibraryPage';

export default function App() {
  return (
    <div className="min-h-screen">
      <Navbar />
      <Routes>
        <Route path="/" element={<BrowsePage />} />
        <Route path="/games/:id" element={<GameDetailsPage />} />
        <Route path="/library" element={<LibraryPage />} />
      </Routes>
    </div>
  );
}
