const STATUSES = ['Wishlist', 'Playing', 'Completed'];

export default function StatusSelect({ value, onChange }) {
  return (
    <select
      value={value}
      onChange={(event) => onChange(event.target.value)}
      className="rounded-md border border-white/10 bg-slate-900 px-2 py-1 text-sm text-slate-200 focus:border-violet-400 focus:outline-none"
    >
      {STATUSES.map((status) => (
        <option key={status} value={status}>
          {status}
        </option>
      ))}
    </select>
  );
}
