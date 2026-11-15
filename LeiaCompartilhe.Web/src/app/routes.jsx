import { Navigate, Route, Routes, Outlet } from 'react-router-dom'
import Login from '../features/auth/Login'
import ProtectedRoute from './ProtectedRoute'
import { useAuth } from '../contexts/useAuth'
import Livros from '../features/livros/Livros'
import Emprestimos from '../features/emprestimos/Emprestimos'
import Doacoes from '../features/doacoes/Doacoes'
import Parcerias from '../features/parcerias/Parcerias'
import AppLayout from '../components/layout/AppLayout'
import Dashboard from '../features/dashboard/Dashboard'
import GestaoUsuarios from '../features/usuarios/GestaoUsuarios'
import Perfil from '../features/usuarios/Perfil'
import Auditoria from '../features/auditoria/Auditoria'

function LoginRoute() {
    const { autenticado } = useAuth()
    return autenticado ? <Navigate to="/" replace /> : <Login />
}

function RedirecionarPorPerfil() {
    const { autenticado, usuario } = useAuth()

    if (!autenticado) return <Navigate to="/login" replace />

    switch (usuario?.tipoUsuario) {
        case 'Administrador':
            return <Navigate to="/administrador" replace />
        case 'Educador':
            return <Navigate to="/educador" replace />
        case 'Aluno':
            return <Navigate to="/aluno" replace />
        default:
            return <Navigate to="/acesso-negado" replace />
    }
}

function LayoutRoute() {
    return (
        <AppLayout>
            <Outlet />
        </AppLayout>
    )
}

function DashboardPerfil({ titulo, descricao }) {
    return (
        <Dashboard
            titulo={titulo}
            descricao={descricao}
        />
    )
}

function AcessoNegado() {
    return (
        <main>
            <h1>Acesso negado</h1>
            <p>Você não tem permissão para acessar esta página.</p>
            <a href="/login">Voltar ao login</a>
        </main>
    )
}

function AppRoutes() {
    return (
        <Routes>
            <Route path="/login" element={<LoginRoute />} />
            <Route path="/acesso-negado" element={<AcessoNegado />} />
            <Route element={<LayoutRoute />}>
                <Route path="/livros" element={<Livros />} />
            </Route>

            <Route element={<ProtectedRoute />}>
                <Route element={<LayoutRoute />}>
                    <Route
                        path="/emprestimos"
                        element={<Emprestimos />}
                    />

                    <Route
                        path="/doacoes"
                        element={<Doacoes />}
                    />

                    <Route element={<ProtectedRoute allowedRoles={['Administrador', 'Educador']} />}>
                        <Route
                            path="/parcerias"
                            element={<Parcerias />}
                        />
                    </Route>

                    <Route path="/perfil" element={<Perfil />} />

                    <Route element={<ProtectedRoute allowedRoles={['Administrador', 'Educador']} />}>
                        <Route
                            path="/usuarios"
                            element={<GestaoUsuarios />}
                        />
                    </Route>

                    <Route element={<ProtectedRoute allowedRoles={['Administrador']} />}>
                        <Route
                            path="/auditoria"
                            element={<Auditoria />}
                        />
                    </Route>

                    <Route element={<ProtectedRoute allowedRoles={['Administrador']} />}>
                        <Route
                            path="/administrador"
                            element={
                                <DashboardPerfil
                                    titulo="Painel do Administrador"
                                    descricao="Área de gestão geral da plataforma."
                                />
                            }
                        />
                    </Route>

                    <Route element={<ProtectedRoute allowedRoles={['Educador']} />}>
                        <Route
                            path="/educador"
                            element={
                                <DashboardPerfil
                                    titulo="Painel do Educador"
                                    descricao="Área de gerenciamento de livros, empréstimos e doações."
                                />
                            }
                        />
                    </Route>

                    <Route element={<ProtectedRoute allowedRoles={['Aluno']} />}>
                        <Route
                            path="/aluno"
                            element={
                                <DashboardPerfil
                                    titulo="Painel do Aluno"
                                    descricao="Área de consulta ao catálogo, empréstimos e doações."
                                />
                            }
                        />
                    </Route>
                </Route>
            </Route>

            <Route path="/" element={<RedirecionarPorPerfil />} />
            <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
    )
}

export default AppRoutes