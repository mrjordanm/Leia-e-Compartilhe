
import { Navigate, Outlet } from 'react-router-dom'
import { useAuth } from '../contexts/useAuth'

function ProtectedRoute({ allowedRoles }) {
    const { autenticado, usuario } = useAuth()

    if (!autenticado) {
        return <Navigate to="/login" replace />
    }

    if (
        allowedRoles &&
        !allowedRoles.includes(usuario?.tipoUsuario)
    ) {
        return <Navigate to="/acesso-negado" replace />
    }

    return <Outlet />
}

export default ProtectedRoute
