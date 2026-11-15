import { API_URL } from '../../services/api'

async function listarRegistrosAuditoria() {
    const token = sessionStorage.getItem('token')

    const resposta = await fetch(`${API_URL}/api/Auditoria`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    })

    const dados = await resposta.json()

    if (!resposta.ok) {
        throw new Error(dados?.mensagem || 'Erro ao carregar auditoria.')
    }

    return dados
}

export { listarRegistrosAuditoria }