import { useState } from 'react'
import { NavLink, useLocation } from 'react-router-dom'
import {
    BookOpen, HandHeart, Handshake, LayoutDashboard,
    Repeat, LogOut, Users, ClipboardList, ChevronDown
} from 'lucide-react'
import { useAuth } from '../../contexts/useAuth'
import './AppLayout.css'

const links = [
    { to: '/', label: 'Início', icon: LayoutDashboard, end: true },
    { to: '/livros', label: 'Catálogo de livros', icon: BookOpen },
    { to: '/emprestimos', label: 'Empréstimos', icon: Repeat },
    { to: '/doacoes', label: 'Doações', icon: HandHeart },
    { to: '/parcerias', label: 'Parcerias', icon: Handshake, staffOnly: true },
]

function AppLayout({ children }) {
    const { usuario, sair, autenticado } = useAuth()
    const location = useLocation()
    const [gestaoAberta, setGestaoAberta] = useState(
        ['/usuarios', '/auditoria'].includes(location.pathname)
    )

    const administrador = usuario?.tipoUsuario === 'Administrador'
    const educador = usuario?.tipoUsuario === 'Educador'

    return (
        <div className="app-shell">
            <aside className="sidebar">
                <div className="sidebar-brand">
                    <div className="brand-icon"><BookOpen size={25} /></div>
                    <div>
                        <h2>Leia & Compartilhe</h2>
                        <span>Literatura e inclusão</span>
                    </div>
                </div>

                <div className="sidebar-user">
                    {autenticado ? (
                        <>
                            <div className="user-avatar">
                                {usuario?.nome?.charAt(0)?.toUpperCase() || 'U'}
                            </div>

                            <NavLink to="/perfil" className="user-info user-profile-link">
                                <strong>{usuario?.nome}</strong>
                                <span>{usuario?.tipoUsuario}</span>
                            </NavLink>

                            <button
                                type="button"
                                className="logout-button"
                                onClick={sair}
                                title="Sair da conta"
                                aria-label="Sair da conta"
                            >
                                <LogOut size={19} />
                            </button>
                        </>
                    ) : (
                        <NavLink to="/login" className="user-info user-profile-link">
                            <strong>Visitante</strong>
                            <span>Entrar ou criar conta</span>
                        </NavLink>
                    )}
                </div>

                <nav className="sidebar-nav">
                    <span className="nav-heading">MENU PRINCIPAL</span>

                    {links
                        .filter(link => {
                            if (!autenticado) return link.to === '/livros'
                            if (link.staffOnly && usuario?.tipoUsuario === 'Aluno') return false
                            return true
                        })
                        .map(link => {
                            const Icone = link.icon
                            return (
                                <NavLink
                                    key={link.to}
                                    to={link.to}
                                    end={link.end}
                                    className={({ isActive }) => isActive ? 'menu-link active' : 'menu-link'}
                                >
                                    <Icone size={19} />
                                    <span>{link.label}</span>
                                </NavLink>
                            )
                        })}

                    {(administrador || educador) && (
                        <div className="menu-group">
                            <button
                                type="button"
                                className={`menu-link menu-group-button ${gestaoAberta ? 'active' : ''}`}
                                onClick={() => setGestaoAberta(!gestaoAberta)}
                            >
                                <Users size={19} />
                                <span>Gestão</span>
                                <ChevronDown
                                    size={16}
                                    className={gestaoAberta ? 'menu-chevron aberto' : 'menu-chevron'}
                                />
                            </button>

                            {gestaoAberta && (
                                <div className="submenu">
                                    <NavLink
                                        to="/usuarios"
                                        className={({ isActive }) => isActive ? 'submenu-link active' : 'submenu-link'}
                                    >
                                        <Users size={16} />
                                        <span>Usuários</span>
                                    </NavLink>

                                    {administrador && (
                                        <NavLink
                                            to="/auditoria"
                                            className={({ isActive }) => isActive ? 'submenu-link active' : 'submenu-link'}
                                        >
                                            <ClipboardList size={16} />
                                            <span>Auditoria</span>
                                        </NavLink>
                                    )}
                                </div>
                            )}
                        </div>
                    )}

                </nav>

            </aside>

            <div className="app-main">
                <header className="topbar">
                    <div>
                        <span className="topbar-subtitle">Plataforma de inclusão literária</span>
                        <h1>
                            {autenticado
                                ? `Bem-vindo, ${usuario?.nome}!`
                                : 'Bem-vindo ao Leia & Compartilhe!'}
                        </h1>
                    </div>
                    <span className="profile-label">
                        {autenticado ? usuario?.tipoUsuario : 'Visitante'}
                    </span>
                </header>

                <div className="page-content">{children}</div>
            </div>
        </div>
    )
}

export default AppLayout