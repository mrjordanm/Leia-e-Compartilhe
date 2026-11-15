import { useEffect, useState } from 'react'
import { useNavigate } from 'react-router-dom'
import {
    BookOpen,
    UserRound,
    Tag,
    CheckCircle2,
    CircleX,
    HandHeart,
} from 'lucide-react'
import { listarLivros } from './livrosService'
import { solicitarEmprestimo } from '../emprestimos/emprestimosService'
import { useAuth } from '../../contexts/useAuth'
import './Livros.css'

function Livros() {
    const { usuario } = useAuth()
    const navigate = useNavigate()
    const autenticado = !!usuario
    const [livros, setLivros] = useState([])
    const [carregando, setCarregando] = useState(true)
    const [erro, setErro] = useState('')
    const [mensagem, setMensagem] = useState('')
    const [processandoId, setProcessandoId] = useState(null)
    const [solicitacoesEnviadas, setSolicitacoesEnviadas] = useState([])

    const ehAluno = usuario?.tipoUsuario === 'Aluno'

    useEffect(() => {
        async function carregarLivros() {
            try {
                const dados = await listarLivros()
                setLivros(dados)
            } catch (error) {
                setErro(error.message)
            } finally {
                setCarregando(false)
            }
        }

        carregarLivros()
    }, [])

    async function solicitar(livroId) {
        if (!autenticado) {
            navigate('/login')
            return
        }
        setProcessandoId(livroId)
        setErro('')
        setMensagem('')

        try {
            await solicitarEmprestimo(livroId)

            setSolicitacoesEnviadas((atuais) => [...atuais, livroId])
            setMensagem('Solicitação de empréstimo enviada com sucesso!')
        } catch (error) {
            setErro(error.message)
        } finally {
            setProcessandoId(null)
        }
    }

    if (carregando) {
        return (
            <div className="livros-carregando">
                <BookOpen size={28} />
                <p>Carregando catálogo de livros...</p>
            </div>
        )
    }

    return (
        <main className="catalogo-livros">
            <section className="catalogo-cabecalho">
                <div className="catalogo-icone">
                    <BookOpen size={30} />
                </div>

                <div>
                    <span className="catalogo-subtitulo">
                        BIBLIOTECA DIGITAL
                    </span>
                    <h2>Catálogo de livros</h2>
                    <p>
                        Explore os livros disponíveis e descubra sua próxima leitura.
                    </p>
                </div>

                {!autenticado && (
                    <button
                        type="button"
                        className="botao-doar-visitante"
                        onClick={() => navigate('/login')}
                    >
                        <HandHeart size={18} />
                        Doar um livro
                    </button>
                )}
            </section>

            {erro && (
                <div className="livros-mensagem erro" role="alert">
                    <CircleX size={19} />
                    <span>{erro}</span>
                </div>
            )}

            {mensagem && (
                <div className="livros-mensagem sucesso" role="status">
                    <CheckCircle2 size={19} />
                    <span>{mensagem}</span>
                </div>
            )}

            <section className="catalogo-conteudo">
                <div className="catalogo-titulo">
                    <div>
                        <h3>Acervo disponível</h3>
                        <p>Confira os títulos cadastrados na plataforma.</p>
                    </div>

                    <span className="catalogo-contagem">
                        {livros.length} {livros.length === 1 ? 'livro' : 'livros'}
                    </span>
                </div>

                {livros.length === 0 ? (
                    <div className="catalogo-vazio">
                        <BookOpen size={38} />
                        <h3>Nenhum livro cadastrado</h3>
                        <p>Novos títulos aparecerão aqui quando forem adicionados ao acervo.</p>
                    </div>
                ) : (
                    <div className="livros-grid">
                        {livros.map((livro) => {
                            const solicitacaoEnviada = solicitacoesEnviadas.includes(livro.id)
                            const disponivel = livro.disponivel

                            return (
                                <article className="livro-card" key={livro.id}>
                                    <div className="livro-card-topo">
                                        <div className="livro-capa-icone">
                                            <BookOpen size={34} />
                                        </div>

                                        <span className={`livro-status ${disponivel ? 'disponivel' : 'indisponivel'}`}>
                                            {disponivel ? (
                                                <>
                                                    <CheckCircle2 size={14} />
                                                    Disponível
                                                </>
                                            ) : (
                                                <>
                                                    <CircleX size={14} />
                                                    Indisponível
                                                </>
                                            )}
                                        </span>
                                    </div>

                                    <div className="livro-card-conteudo">
                                        <h3>{livro.titulo}</h3>

                                        <p className="livro-autor">
                                            <UserRound size={15} />
                                            {livro.autor || 'Autor não informado'}
                                        </p>

                                        <div className="livro-informacoes">
                                            <span>
                                                <Tag size={14} />
                                                {livro.genero || 'Gênero não informado'}
                                            </span>

                                            <span>
                                                Condição: {livro.condicao || 'Não informada'}
                                            </span>
                                        </div>

                                        {livro.origem && (
                                            <p className="livro-origem">
                                                Origem: {livro.origem}
                                            </p>
                                        )}

                                        {livro.observacoes && (
                                            <p className="livro-observacoes">
                                                {livro.observacoes}
                                            </p>
                                        )}
                                    </div>

                                    {(ehAluno || !autenticado) && (
                                        <div className="livro-card-rodape">
                                            {disponivel ? (
                                                <button
                                                    type="button"
                                                    className="botao-solicitar"
                                                    disabled={processandoId === livro.id || solicitacaoEnviada}
                                                    onClick={() => solicitar(livro.id)}
                                                >
                                                    <HandHeart size={17} />
                                                    {processandoId === livro.id
                                                        ? 'Enviando...'
                                                        : solicitacaoEnviada
                                                            ? 'Solicitação enviada'
                                                            : 'Solicitar empréstimo'}
                                                </button>
                                            ) : (
                                                <span className="livro-indisponivel-aviso">
                                                    Este livro não está disponível no momento.
                                                </span>
                                            )}
                                        </div>
                                    )}
                                </article>
                            )
                        })}
                    </div>
                )}
            </section>
        </main>
    )
}

export default Livros
