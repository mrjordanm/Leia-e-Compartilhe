
import { API_URL } from '../../services/api'

async function login(email, senha) {
    const resposta = await fetch(`${API_URL}/api/Usuarios/login`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify({ email, senha }),
    })

    const dados = await resposta.json()

    if (!resposta.ok)
        throw new Error(dados.mensagem || 'Não foi possível realizar o login.')

    return dados
}

async function registrarAluno(dadosAluno) {
    const resposta = await fetch(`${API_URL}/api/Usuarios/registrar`, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json',
        },
        body: JSON.stringify(dadosAluno),
    })

    const dados = await resposta.json()

    if (!resposta.ok)
        throw new Error(dados.mensagem || 'Não foi possível realizar o cadastro.')

    return dados
}

export { login, registrarAluno }