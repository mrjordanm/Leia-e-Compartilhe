import { useState } from 'react'
import { useAuth } from '../../contexts/useAuth'
import { registrarAluno } from './authService'
import './Login.css'
import { Link } from 'react-router-dom'

function Login() {
    const { entrar } = useAuth()
    const [modoCadastro, setModoCadastro] = useState(false)
    const [email, setEmail] = useState('')
    const [senha, setSenha] = useState('')
    const [formulario, setFormulario] = useState({
        nome: '', email: '', senha: '', ru: '', instituicao: ''
    })
    const [erro, setErro] = useState('')
    const [mensagem, setMensagem] = useState('')
    const [tentativasFalhas, setTentativasFalhas] = useState(0)
    const [carregando, setCarregando] = useState(false)

    function atualizarCampo(evento) {
        const { name, value } = evento.target
        setFormulario(anterior => ({ ...anterior, [name]: value }))
    }

    async function handleSubmit(evento) {
        evento.preventDefault()
        setErro('')
        setMensagem('')
        setCarregando(true)

        try {
            if (modoCadastro) {
                await registrarAluno(formulario)
                setEmail(formulario.email)
                setSenha('')
                setFormulario({ nome: '', email: '', senha: '', ru: '', instituicao: '' })
                setModoCadastro(false)
                setMensagem('Cadastro realizado! Entre com seu e-mail e senha.')
            } else {
                const usuario = await entrar(email, senha)
                setTentativasFalhas(0)
            }
        } catch (error) {
            if (modoCadastro) {
                setErro(error.message)
            } else {
                const novasTentativas = tentativasFalhas + 1
                setTentativasFalhas(novasTentativas)

                if (novasTentativas > 3) {
                    setErro('')
                    setMensagem(
                        'Não conseguiu acessar sua conta? Se você esqueceu sua senha, entre em contato com um educador ou administrador para receber ajuda na recuperação do acesso.'
                    )
                } else {
                    setErro(error.message)
                }
            }
        } finally {
            setCarregando(false)
        }
    }

    return (
        <main className="login-page">
            <section className="login-container">
                <header className="login-header">
                    <h1>Leia & Compartilhe</h1>
                    <p>Inclusão literária e sustentabilidade</p>
                </header>

                <div className="login-content">
                    <h2>{modoCadastro ? 'Criar conta' : 'Bem-vindo!'}</h2>
                    <p>{modoCadastro ? 'Preencha os dados para se cadastrar.' : 'Entre com suas credenciais para acessar a plataforma.'}</p>

                    <form onSubmit={handleSubmit}>
                        {modoCadastro ? (
                            <>
                                <label htmlFor="nome">Nome completo</label>
                                <input id="nome" name="nome" value={formulario.nome} onChange={atualizarCampo} placeholder="Digite seu nome completo" required />

                                <label htmlFor="cadastro-email">E-mail</label>
                                <input id="cadastro-email" name="email" type="email" value={formulario.email} onChange={atualizarCampo} placeholder="seuemail@exemplo.com" required />

                                <label htmlFor="ru">RU</label>
                                <input id="ru" name="ru" type="text" inputMode="numeric" pattern="[0-9]+" value={formulario.ru} onChange={(evento) => { const valor = evento.target.value.replace(/\D/g, '')
                                        setFormulario(anterior => ({ ...anterior, ru: valor }))}} placeholder="Digite seu RU" required
                                />

                                <label htmlFor="instituicao">Instituição</label>
                                <input id="instituicao" name="instituicao" value={formulario.instituicao} onChange={atualizarCampo} placeholder="Nome da instituição" maxLength={150} required />

                                <label htmlFor="cadastro-senha">Senha</label>
                                <input id="cadastro-senha" name="senha" type="password" value={formulario.senha} onChange={atualizarCampo} placeholder="Crie uma senha" required />
                            </>
                        ) : (
                            <>
                                <label htmlFor="email">E-mail</label>
                                <input id="email" type="email" placeholder="seuemail@exemplo.com" value={email} onChange={evento => setEmail(evento.target.value)} required />

                                <label htmlFor="senha">Senha</label>
                                <input id="senha" type="password" placeholder="Digite sua senha" value={senha} onChange={evento => setSenha(evento.target.value)} required />
                            </>
                        )}

                        {erro && <p role="alert">{erro}</p>}
                        {mensagem && <p role="status">{mensagem}</p>}

                        <button type="submit" disabled={carregando}>
                            {carregando ? 'Aguarde...' : modoCadastro ? 'Criar conta' : 'Entrar'}
                        </button>
                    </form>

                    <p className="login-register">
                        {modoCadastro ? 'Já possui cadastro? ' : 'Ainda não possui cadastro? '}
                        <a href="#" onClick={evento => {
                            evento.preventDefault()
                            setModoCadastro(!modoCadastro)
                            setErro('')
                            setMensagem('')
                        }}>
                            {modoCadastro ? 'Entrar' : 'Cadastre-se'}
                        </a>
                    </p>

                    <Link to="/livros" className="login-back-link">
                        Ir para o catálogo
                    </Link>
                </div>
            </section>
            <footer className="login-footer">
                    Atividade Extensionista III · Jordan Matos · RU: 4671350
            </footer>
        </main>
    )
}

export default Login