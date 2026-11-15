
import { API_URL } from '../../services/api'

async function requisicaoUsuarios(url, opcoes = {}) {
    const token = sessionStorage.getItem('token')

    const resposta = await fetch(`${API_URL}${url}`, {
        ...opcoes,
        headers: {
            ...(opcoes.body ? { 'Content-Type': 'application/json' } : {}),
            Authorization: `Bearer ${token}`,
            ...opcoes.headers,
        },
    })

    const texto = await resposta.text()
    let dados = null

    try {
        dados = texto ? JSON.parse(texto) : null
    } catch {
        dados = texto
    }

    if (!resposta.ok) {
        throw new Error(
            dados?.mensagem || `Erro na operação (${resposta.status}).`
        )
    }

    return dados
}

function cadastrarEducador(educador) {
    return requisicaoUsuarios('/api/Usuarios/cadastrar-educador', {
        method: 'POST',
        body: JSON.stringify(educador),
    })
}

function listarUsuarios() {
    return requisicaoUsuarios('/api/Usuarios')
}

function alterarStatusUsuario(id, ativo) {
    return requisicaoUsuarios(`/api/Usuarios/${id}/status`, {
        method: 'PATCH',
        body: JSON.stringify(ativo),
    })
}

export { cadastrarEducador, listarUsuarios, alterarStatusUsuario }