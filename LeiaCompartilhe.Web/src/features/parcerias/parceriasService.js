import { API_URL } from '../../services/api'

async function requisitarParcerias(endpoint) {
    const token = sessionStorage.getItem('token')

    const resposta = await fetch(`${API_URL}/api/Parceria${endpoint}`, {
        method: 'GET',
        headers: {
            Authorization: `Bearer ${token}`,
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
            dados?.mensagem ||
            (typeof dados === 'string' ? dados : null) ||
            `Erro ao consultar parcerias (${resposta.status}).`
        )
    }

    return dados
}

async function listarParcerias() {
    return requisitarParcerias('')
}

async function buscarParceriaPorId(id) {
    return requisitarParcerias(`/${id}`)
}

async function cadastrarParceria(parceria) {
    const token = sessionStorage.getItem('token')

    const resposta = await fetch(`${API_URL}/api/Parceria`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
        },
        body: JSON.stringify(parceria),
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
            `Erro ao cadastrar parceria (${resposta.status}).`
        )
    }

    return dados
}

export {
    listarParcerias,
    buscarParceriaPorId,
    cadastrarParceria,
}