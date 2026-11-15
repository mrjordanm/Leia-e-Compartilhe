
import { API_URL } from '../../services/api'

async function requisitarEmprestimos(endpoint, metodo = 'GET', corpo = null) {
    const token = sessionStorage.getItem('token')

    const resposta = await fetch(`${API_URL}/api/Emprestimos${endpoint}`, {
        method: metodo,
        headers: {
            Authorization: `Bearer ${token}`,
            ...(corpo && { 'Content-Type': 'application/json' }),
        },
        ...(corpo && { body: JSON.stringify(corpo) }),
    })

    const texto = await resposta.text()
    const dados = texto ? JSON.parse(texto) : null

    if (!resposta.ok) {
        throw new Error(
            dados?.mensagem || `Erro na operação de empréstimo (${resposta.status}).`
        )
    }

    return dados
}

async function listarTodosEmprestimos() {
    return requisitarEmprestimos('')
}

async function listarMeusEmprestimos() {
    return requisitarEmprestimos('/meus')
}

async function solicitarEmprestimo(livroId) {
    return requisitarEmprestimos('/solicitar', 'POST', { livroId })
}

async function decidirEmprestimo(id, aprovado) {
    return requisitarEmprestimos(`/${id}/decidir`, 'PUT', { aprovado })
}


async function registrarRetirada(id) {
    return requisitarEmprestimos(`/${id}/retirada`, 'PUT', {
        confirmarRetirada: true
    })
}



async function registrarDevolucao(id) {
    return requisitarEmprestimos(`/${id}/devolucao`, 'PUT', {
        confirmarDevolucao: true
    })
}


export {
    listarTodosEmprestimos,
    listarMeusEmprestimos,
    solicitarEmprestimo,
    decidirEmprestimo,
    registrarRetirada,
    registrarDevolucao,
}