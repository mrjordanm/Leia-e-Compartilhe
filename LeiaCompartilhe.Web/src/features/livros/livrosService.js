import { API_URL } from '../../services/api'

async function listarLivros() {
    const token = sessionStorage.getItem('token')

    const headers = {}

    if (token) {
        headers.Authorization = `Bearer ${token}`
    }

    const resposta = await fetch(`${API_URL}/api/Livros`, {
        method: 'GET',
        headers,
    })

    const dados = await resposta.json()

    if (!resposta.ok) {
        throw new Error(
            dados.mensagem || 'Não foi possível carregar os livros.'
        )
    }

    return dados
}

export { listarLivros }