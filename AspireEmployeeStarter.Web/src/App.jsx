import { useCallback, useEffect, useMemo, useState } from 'react';
import { UsersRound, UserPlus, Search, RefreshCw, Pencil, Trash2, X, Check, AlertCircle, Database, LayoutDashboard, BriefcaseBusiness, CalendarDays } from 'lucide-react';

const empty = { firstName: '', lastName: '', email: '', department: '', dateOfJoining: new Date().toISOString().slice(0, 10), isActive: true };

async function request(path, options = {}) {
  const response = await fetch(path, { ...options, headers: { 'Content-Type': 'application/json', ...options.headers } });
  if (response.status === 204) return null;
  const body = await response.json().catch(() => null);
  if (!response.ok) throw new Error(body?.error || body?.detail || body?.title || `Request failed (${response.status})`);
  return body;
}

export default function App() {
  const [employees, setEmployees] = useState([]);
  const [form, setForm] = useState(empty);
  const [editingId, setEditingId] = useState(null);
  const [search, setSearch] = useState('');
  const [showForm, setShowForm] = useState(false);
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [error, setError] = useState('');
  const [notice, setNotice] = useState('');
  const [database, setDatabase] = useState('Checking…');

  const load = useCallback(async () => {
    setLoading(true); setError('');
    try {
      const data = await request('/api/employees');
      setEmployees(Array.isArray(data) ? data : []);
      const status = await request('/api/database/status');
      setDatabase(`${status.database} · Connected`);
    } catch (err) { setError(err.message); setDatabase('Connection unavailable'); }
    finally { setLoading(false); }
  }, []);
  useEffect(() => { load(); }, [load]);

  const filtered = useMemo(() => employees.filter(e =>
    `${e.firstName} ${e.lastName} ${e.email} ${e.department}`.toLowerCase().includes(search.toLowerCase())
  ), [employees, search]);
  const activeCount = employees.filter(e => e.isActive).length;
  const departments = new Set(employees.map(e => e.department)).size;

  function openCreate() { setEditingId(null); setForm({ ...empty }); setShowForm(true); setError(''); }
  function openEdit(e) {
    setEditingId(e.id);
    setForm({ firstName: e.firstName, lastName: e.lastName, email: e.email, department: e.department, dateOfJoining: e.dateOfJoining, isActive: e.isActive });
    setShowForm(true); setError('');
  }
  async function submit(event) {
    event.preventDefault(); setSaving(true); setError(''); setNotice('');
    try {
      await request(editingId ? `/api/employees/${editingId}` : '/api/employees', {
        method: editingId ? 'PUT' : 'POST', body: JSON.stringify(form)
      });
      setNotice(editingId ? 'Employee updated successfully.' : 'Employee added successfully.');
      setShowForm(false); setEditingId(null); await load();
    } catch (err) { setError(err.message); }
    finally { setSaving(false); }
  }
  async function remove(e) {
    if (!window.confirm(`Delete ${e.firstName} ${e.lastName}? This cannot be undone.`)) return;
    setError(''); setNotice('');
    try { await request(`/api/employees/${e.id}`, { method: 'DELETE' }); setNotice('Employee deleted.'); await load(); }
    catch (err) { setError(err.message); }
  }

  return <div className="app-shell">
    <aside className="sidebar">
      <div className="brand"><div className="brand-icon"><UsersRound size={22}/></div><div><strong>Workforce</strong><span>MANAGEMENT</span></div></div>
      <div className="nav-label">WORKSPACE</div>
      <div className="nav-item selected"><LayoutDashboard size={18}/> Overview</div>
      <div className="nav-item"><UsersRound size={18}/> Employees</div>
      <div className="nav-label nav-label-lower">SYSTEM</div>
      <div className="nav-item"><Database size={18}/> {database.includes('Connected') ? 'Database online' : 'Database status'}</div>
      <div className="sidebar-bottom"><div className="avatar">AS</div><div><strong>Aspire Starter</strong><span>Local development</span></div></div>
    </aside>

    <main className="main">
      <header className="topbar"><div className="breadcrumb">Workspace <span>/</span> <b>Employees</b></div><div className="topbar-right"><span className="live-dot"/> Aspire connected <span className="divider-vertical"/> <span className="environment">DEVELOPMENT</span></div></header>
      <div className="content">
        <div className="heading-row"><div><div className="eyebrow">PEOPLE & OPERATIONS</div><h1>Employee management</h1><p className="subtitle">Manage your team members, departments, and employee records in one place.</p></div><button className="primary-btn" onClick={openCreate}><UserPlus size={17}/> Add employee</button></div>
        <div className="stats">
          <div className="stat"><div className="stat-title">Total employees <span className="stat-icon blue"><UsersRound size={19}/></span></div><div className="stat-value">{employees.length}</div><div className="stat-foot">Across all departments</div></div>
          <div className="stat"><div className="stat-title">Active employees <span className="stat-icon green"><Check size={19}/></span></div><div className="stat-value">{activeCount}</div><div className="stat-foot">Currently active</div></div>
          <div className="stat"><div className="stat-title">Departments <span className="stat-icon violet"><BriefcaseBusiness size={19}/></span></div><div className="stat-value">{departments}</div><div className="stat-foot">Unique teams</div></div>
        </div>
        {error && <div className="alert error"><AlertCircle size={17}/><span>{error}</span><button onClick={()=>setError('')} aria-label="Dismiss error"><X size={16}/></button></div>}
        {notice && <div className="alert success"><Check size={17}/><span>{notice}</span><button onClick={()=>setNotice('')} aria-label="Dismiss notification"><X size={16}/></button></div>}
        <section className="panel"><div className="panel-top"><div><h2>All employees</h2><p>View and manage your employee directory</p></div><div className="tools"><div className="search"><Search size={17}/><input aria-label="Search employees" value={search} onChange={e=>setSearch(e.target.value)} placeholder="Search employees..."/></div><button className="icon-btn" title="Refresh" onClick={load} disabled={loading}><RefreshCw size={17}/></button></div></div>
          <div className="table-wrap"><table><thead><tr><th>EMPLOYEE</th><th>DEPARTMENT</th><th>DATE JOINED</th><th>STATUS</th><th className="right">ACTIONS</th></tr></thead><tbody>
          {loading ? <tr><td colSpan="5" className="empty">Loading employees…</td></tr> : filtered.length === 0 ? <tr><td colSpan="5" className="empty">{search ? 'No employees match your search.' : 'No employees yet. Click “Add employee” to create your first record.'}</td></tr> : filtered.map(e => <tr key={e.id}><td><div className="employee-cell"><div className="initials">{e.firstName[0]}{e.lastName[0]}</div><div><strong>{e.firstName} {e.lastName}</strong><small>{e.email}</small></div></div></td><td>{e.department}</td><td>{e.dateOfJoining}</td><td><span className={`status ${e.isActive ? 'active' : 'inactive'}`}><span className="status-dot"/>{e.isActive ? 'Active' : 'Inactive'}</span></td><td><div className="actions"><button aria-label={`Edit ${e.firstName}`} title="Edit" onClick={()=>openEdit(e)}><Pencil size={16}/></button><button className="delete" aria-label={`Delete ${e.firstName}`} title="Delete" onClick={()=>remove(e)}><Trash2 size={16}/></button></div></td></tr>)}
          </tbody></table></div><div className="table-footer">Showing {filtered.length} of {employees.length} employees <span>SQL Server · TestDataBase</span></div></section>
      </div>
    </main>

    {showForm && <div className="modal-overlay" onMouseDown={e=>{if(e.target===e.currentTarget && !saving) setShowForm(false)}}><div className="modal" role="dialog" aria-modal="true" aria-labelledby="form-title"><div className="modal-header"><div><h2 id="form-title">{editingId ? 'Edit employee' : 'Add new employee'}</h2><p>{editingId ? 'Update employee information' : 'Enter the details to create an employee record'}</p></div><button className="icon-btn" onClick={()=>setShowForm(false)} aria-label="Close" disabled={saving}><X size={19}/></button></div>
      <form onSubmit={submit}><div className="form-body"><div className="form-grid"><label>First name <input required maxLength="100" value={form.firstName} onChange={e=>setForm({...form,firstName:e.target.value})} placeholder="e.g. Priya"/></label><label>Last name <input required maxLength="100" value={form.lastName} onChange={e=>setForm({...form,lastName:e.target.value})} placeholder="e.g. Kumar"/></label></div><label>Email address <input required type="email" maxLength="256" value={form.email} onChange={e=>setForm({...form,email:e.target.value})} placeholder="priya@example.com"/></label><label>Department <input required maxLength="100" value={form.department} onChange={e=>setForm({...form,department:e.target.value})} placeholder="e.g. Engineering"/></label><label>Date of joining <input required type="date" value={form.dateOfJoining} onChange={e=>setForm({...form,dateOfJoining:e.target.value})}/></label>{editingId && <label className="checkbox-row"><input type="checkbox" checked={form.isActive} onChange={e=>setForm({...form,isActive:e.target.checked})}/> Active employee</label>}</div><div className="modal-actions"><button type="button" className="secondary-btn" disabled={saving} onClick={()=>setShowForm(false)}>Cancel</button><button type="submit" className="primary-btn" disabled={saving}>{saving ? 'Saving…' : editingId ? 'Save changes' : 'Create employee'}</button></div></form>
    </div></div>}
  </div>;
}
