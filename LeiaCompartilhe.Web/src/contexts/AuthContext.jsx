
import { createContext, useContext, useState } from 'react'
import { login as realizarLogin } from '../features/auth/authService'

export const AuthContext = createContext(null)

function AuthProvider({ children }) {
    const [token, setToken] = useState(
        () => sessionStorage.getItem('token')
    )

    const [usuario, setUsuario] = useState(() => {
        const usuarioSalvo = sessionStorage.getItem('usuario')

        return usuarioSalvo ? JSON.parse(usuarioSalvo) : null
    })

    async function entrar(email, senha) {
        const dados = await realizarLogin(email, senha)

        sessionStorage.setItem('token', dados.token)
        sessionStorage.setItem('usuario', JSON.stringify(dados.usuario))

        setToken(dados.token)
        setUsuario(dados.usuario)

        return dados.usuario
    }

    function sair() {
        sessionStorage.removeItem('token')
        sessionStorage.removeItem('usuario')

        setToken(null)
        setUsuario(null)
    }

    const autenticado = Boolean(token && usuario)

    return (
        <AuthContext.Provider
            value={{
                token,
                usuario,
                autenticado,
                entrar,
                sair,
            }}
        >
            {children}
        </AuthContext.Provider>
    )
}

export function useAuth() {
    return useContext(AuthContext)
}

export { AuthProvider }