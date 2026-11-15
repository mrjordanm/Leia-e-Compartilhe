
import { API_URL } from '../../services/api'

async function buscarPerfil() {
    const token = sessionStorage.getItem('token')
    const resposta = await fetch(`${API_URL}/api/Usuarios/perfil`, {
        headers: { Authorization: `Bearer ${token}` },
    })

    const dados = await resposta.json()
    if (!resposta.ok) throw new Error(dados?.mensagem || 'Erro ao carregar perfil.')
    return dados
}

export { buscarPerfil }
