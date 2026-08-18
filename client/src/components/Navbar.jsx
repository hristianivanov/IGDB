import { NavLink } from 'react-router-dom';
import { Gamepad2, Library } from 'lucide-react';

const linkClasses = ({ isActive }) =>
  `flex items-center gap-1.5 rounded-md px-3 py-1.5 text-sm font-medium transition ${
    isActive ? 'bg-violet-500/20 text-violet-300' : 'text-slate-300 hover:bg-white/5 hover:text-white'
  }`;

export default function Navbar() {
  return (
    <header className="sticky top-0 z-10 border-b border-white/10 bg-[#0b0f1a]/90 backdrop-blur">
      <nav className="mx-auto flex max-w-6xl items-center justify-between px-4 py-3">
        <NavLink to="/" className="flex items-center gap-2 text-lg font-semibold text-white">
          <Gamepad2 className="text-violet-400" size={22} />
          GameVault
        </NavLink>

        <div className="flex items-center gap-2">
          <NavLink to="/" end className={linkClasses}>
            Browse
          </NavLink>
          <NavLink to="/library" className={linkClasses}>
            <Library size={16} />
            Library
          </NavLink>
        </div>
      </nav>
    </header>
  );
}
