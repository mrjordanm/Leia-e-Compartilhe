import { useCallback, useEffect, useState } from 'react'
import {
    BookOpen,
    CalendarDays,
    CheckCircle2,
    CircleX,
    Clock3,
    RotateCcw,
    UserRound,
} from 'lucide-react'
import { useAuth } from '../../contexts/useAuth'
import {
    listarTodosEmprestimos,
    listarMeusEmprestimos,
    decidirEmprestimo,
    registrarRetirada,
    registrarDevolucao,
} from './emprestimosService'
import './Emprestimos.css'

function Emprestimos() {
    const { usuario } = useAuth()

    const [emprestimos, setEmprestimos] = useState([])
    const [carregando, setCarregando] = useState(true)
    const [erro, setErro] = useState('')
    const [mensagem, setMensagem] = useState('')
    const [processando, setProcessando] = useState(false)

    const ehAluno = usuario?.tipoUsuario === 'Aluno'

    const carregarEmprestimos = useCallback(async () => {
        setErro('')

        try {
            const dados = ehAluno
                ? await listarMeusEmprestimos()
                : await listarTodosEmprestimos()

            setEmprestimos(dados)
        } catch (error) {
            setErro(error.message)
        } finally {
            setCarregando(false)
        }
    }, [ehAluno])

    useEffect(() => {
        carregarEmprestimos()
    }, [carregarEmprestimos])

    async function executarAcao(acao, mensagemSucesso) {
        setProcessando(true)
        setErro('')
        setMensagem('')

        try {
            await acao()
            setMensagem(mensagemSucesso)
            await carregarEmprestimos()
        } catch (error) {
            setErro(error.message)
        } finally {
            setProcessando(false)
        }
    }

    function formatarData(data) {
        if (!data) return 'Não informado'
        return new Date(data).toLocaleDateString('pt-BR')
    }

    function obterStatus(status) {
        const estilos = {
            Pendente: 'pendente',
            Aprovado: 'aprovado',
            Reprovado: 'reprovado',
            Ativo: 'ativo',
            Devolvido: 'devolvido',
            Cancelado: 'cancelado',
        }

        return estilos[status] || 'outro'
    }

    if (carregando) {
        return (
            <div className="emprestimos-carregando">
                <BookOpen size={28} />
                <p>Carregando empréstimos...</p>
            </div>
        )
    }

    return (
        <main className="pagina-emprestimos">
            <section className="emprestimos-cabecalho">
                <div className="emprestimos-icone">
                    <BookOpen size={30} />
                </div>

                <div>
                    <span className="emprestimos-subtitulo">
                        LEIA & COMPARTILHE
                    </span>
                    <h2>Empréstimos</h2>
                    <p>
                        {ehAluno
                            ? 'Consulte suas solicitações e acompanhe seus empréstimos.'
                            : 'Acompanhe solicitações e gerencie a circulação dos livros.'}
                    </p>
                </div>
            </section>

            {erro && (
                <div className="emprestimos-mensagem erro" role="alert">
                    <CircleX size={19} />
                    <span>{erro}</span>
                </div>
            )}

            {mensagem && (
                <div className="emprestimos-mensagem sucesso" role="status">
                    <CheckCircle2 size={19} />
                    <span>{mensagem}</span>
                </div>
            )}

            <section className="emprestimos-conteudo">
                <div className="emprestimos-titulo">
                    <div>
                        <h3>{ehAluno ? 'Minhas solicitações' : 'Solicitações de empréstimo'}</h3>
                        <p>
                            {ehAluno
                                ? 'Histórico e situação dos seus empréstimos.'
                                : 'Consulte os pedidos e realize as movimentações necessárias.'}
                        </p>
                    </div>

                    <span className="emprestimos-contagem">
                        {emprestimos.length} {emprestimos.length === 1 ? 'registro' : 'registros'}
                    </span>
                </div>

                {emprestimos.length === 0 ? (
                    <div className="emprestimos-vazio">
                        <BookOpen size={38} />
                        <h3>Nenhum empréstimo encontrado</h3>
                        <p>
                            Quando houver solicitações, elas aparecerão nesta área.
                        </p>
                    </div>
                ) : (
                    <div className="emprestimos-lista">
                        {emprestimos.map((emprestimo) => (
                            <article className="emprestimo-card" key={emprestimo.id}>
                                <div className="emprestimo-card-cabecalho">
                                    <div className="emprestimo-livro-icone">
                                        <BookOpen size={25} />
                                    </div>

                                    <div className="emprestimo-livro-titulo">
                                        <span>EMPRÉSTIMO #{emprestimo.id}</span>
                                        <h3>{emprestimo.tituloLivro}</h3>
                                    </div>

                                    <span className={`emprestimo-status ${obterStatus(emprestimo.status)}`}>
                                        {emprestimo.status}
                                    </span>
                                </div>

                                <div className="emprestimo-detalhes">
                                    {!ehAluno && (
                                        <div className="emprestimo-detalhe">
                                            <UserRound size={17} />
                                            <div>
                                                <span>Solicitante</span>
                                                <strong>{emprestimo.nomeSolicitante || 'Não informado'}</strong>
                                            </div>
                                        </div>
                                    )}

                                    <div className="emprestimo-detalhe">
                                        <CalendarDays size={17} />
                                        <div>
                                            <span>Data da solicitação</span>
                                            <strong>{formatarData(emprestimo.dataSolicitacao)}</strong>
                                        </div>
                                    </div>

                                    {emprestimo.dataLimiteRetirada && (
                                        <div className="emprestimo-detalhe">
                                            <Clock3 size={17} />
                                            <div>
                                                <span>Limite para retirada</span>
                                                <strong>{formatarData(emprestimo.dataLimiteRetirada)}</strong>
                                            </div>
                                        </div>
                                    )}

                                    {emprestimo.dataDevolucaoPrevista && (
                                        <div className="emprestimo-detalhe">
                                            <CalendarDays size={17} />
                                            <div>
                                                <span>Devolução prevista</span>
                                                <strong>{formatarData(emprestimo.dataDevolucaoPrevista)}</strong>
                                            </div>
                                        </div>
                                    )}

                                    {emprestimo.dataDevolucao && (
                                        <div className="emprestimo-detalhe">
                                            <CheckCircle2 size={17} />
                                            <div>
                                                <span>Devolvido em</span>
                                                <strong>{formatarData(emprestimo.dataDevolucao)}</strong>
                                            </div>
                                        </div>
                                    )}
                                </div>

                                {!ehAluno && (
                                    <div className="emprestimo-acoes">
                                        {emprestimo.status === 'Pendente' && (
                                            <>
                                                <button
                                                    type="button"
                                                    className="botao-emprestimo aprovar"
                                                    disabled={processando}
                                                    onClick={() =>
                                                        executarAcao(
                                                            () => decidirEmprestimo(emprestimo.id, true),
                                                            'Empréstimo aprovado com sucesso.'
                                                        )
                                                    }
                                                >
                                                    <CheckCircle2 size={16} />
                                                    Aprovar
                                                </button>

                                                <button
                                                    type="button"
                                                    className="botao-emprestimo reprovar"
                                                    disabled={processando}
                                                    onClick={() =>
                                                        executarAcao(
                                                            () => decidirEmprestimo(emprestimo.id, false),
                                                            'Empréstimo reprovado.'
                                                        )
                                                    }
                                                >
                                                    <CircleX size={16} />
                                                    Reprovar
                                                </button>
                                            </>
                                        )}

                                        {emprestimo.status === 'Aprovado' && (
                                            <button
                                                type="button"
                                                className="botao-emprestimo aprovar"
                                                disabled={processando}
                                                onClick={() =>
                                                    executarAcao(
                                                        () => registrarRetirada(emprestimo.id),
                                                        'Retirada registrada com sucesso.'
                                                    )
                                                }
                                            >
                                                <BookOpen size={16} />
                                                Registrar retirada
                                            </button>
                                        )}

                                        {emprestimo.status === 'Ativo' && (
                                            <button
                                                type="button"
                                                className="botao-emprestimo devolver"
                                                disabled={processando}
                                                onClick={() =>
                                                    executarAcao(
                                                        () => registrarDevolucao(emprestimo.id),
                                                        'Devolução registrada com sucesso.'
                                                    )
                                                }
                                            >
                                                <RotateCcw size={16} />
                                                Registrar devolução
                                            </button>
                                        )}
                                    </div>
                                )}
                            </article>
                        ))}
                    </div>
                )}
            </section>
        </main>
    )
}

export default Emprestimos
