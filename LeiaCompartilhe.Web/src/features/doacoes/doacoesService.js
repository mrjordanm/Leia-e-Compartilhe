
import { API_URL } from '../../services/api'

async function requisitarDoacoes(endpoint, metodo = 'GET', corpo = null) {
    const token = sessionStorage.getItem('token')

    const resposta = await fetch(`${API_URL}/api/Doacoes${endpoint}`, {
        method: metodo,
        headers: {
            Authorization: `Bearer ${token}`,
            ...(corpo && { 'Content-Type': 'application/json' }),
        },
        ...(corpo && { body: JSON.stringify(corpo) }),
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
            dados?.mensagem ||
            (typeof dados === 'string' ? dados : null) ||
            `Erro na operação de doação (${resposta.status}).`
        )
    }

    return dados
}

async function listarTodasDoacoes() {
    return requisitarDoacoes('')
}

async function listarMinhasDoacoes() {
    return requisitarDoacoes('/minhas')
}

async function cadastrarDoacao(doacao) {
    return requisitarDoacoes('', 'POST', doacao)
}

async function decidirDoacao(id, aprovada, parecer) {
    return requisitarDoacoes(`/${id}/decidir`, 'PUT', {
        aprovada,
        parecer,
    })
}

export {
    listarTodasDoacoes,
    listarMinhasDoacoes,
    cadastrarDoacao,
    decidirDoacao,
}
