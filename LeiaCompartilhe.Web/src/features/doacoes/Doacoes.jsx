import { useCallback, useEffect, useState } from 'react'
import {
    BookOpen, CalendarDays, CheckCircle2, CircleX,
    HandHeart, UserRound, Tag, ClipboardList
} from 'lucide-react'
import { useAuth } from '../../contexts/useAuth'
import {
    listarTodasDoacoes, listarMinhasDoacoes,
    cadastrarDoacao, decidirDoacao
} from './doacoesService'
import './Doacoes.css'

const formularioInicial = {
    titulo: '', autor: '', genero: '', condicao: '', observacoes: '',
}

function Doacoes() {
    const { usuario } = useAuth()
    const [doacoes, setDoacoes] = useState([])
    const [formulario, setFormulario] = useState(formularioInicial)
    const [pareceres, setPareceres] = useState({})
    const [carregando, setCarregando] = useState(true)
    const [processando, setProcessando] = useState(false)
    const [erro, setErro] = useState('')
    const [mensagem, setMensagem] = useState('')
    const ehAluno = usuario?.tipoUsuario === 'Aluno'

    const carregarDoacoes = useCallback(async () => {
        setErro('')
        try {
            const dados = ehAluno ? await listarMinhasDoacoes() : await listarTodasDoacoes()
            setDoacoes(dados)
        } catch (error) {
            setErro(error.message)
        } finally {
            setCarregando(false)
        }
    }, [ehAluno])

    useEffect(() => {
        carregarDoacoes()
    }, [carregarDoacoes])

    function atualizarCampo(evento) {
        const { name, value } = evento.target
        setFormulario(atual => ({ ...atual, [name]: value }))
    }

    async function enviarDoacao(evento) {
        evento.preventDefault()
        setProcessando(true)
        setErro('')
        setMensagem('')
        try {
            await cadastrarDoacao(formulario)
            setFormulario(formularioInicial)
            setMensagem('Doação cadastrada com sucesso!')
            await carregarDoacoes()
        } catch (error) {
            setErro(error.message)
        } finally {
            setProcessando(false)
        }
    }

    async function analisarDoacao(id, aprovada) {
        const parecer = pareceres[id] || ''
        if (!parecer.trim()) {
            setErro('Informe um parecer antes de decidir a doação.')
            return
        }

        setProcessando(true)
        setErro('')
        setMensagem('')
        try {
            await decidirDoacao(id, aprovada, parecer)
            setMensagem(aprovada ? 'Doação aprovada com sucesso.' : 'Doação reprovada.')
            await carregarDoacoes()
        } catch (error) {
            setErro(error.message)
        } finally {
            setProcessando(false)
        }
    }

    function obterStatus(status) {
        const estilos = {
            Pendente: 'pendente',
            AguardandoAnalise: 'pendente',
            Aprovada: 'aprovado',
            Aprovado: 'aprovado',
            Reprovada: 'reprovado',
            Reprovado: 'reprovado',
        }
        return estilos[status] || 'outro'
    }

    if (carregando) {
        return <div className="doacoes-carregando"><HandHeart size={28} /><p>Carregando doações...</p></div>
    }

    return (
        <main className="pagina-doacoes">
            <section className="doacoes-cabecalho">
                <div className="doacoes-icone"><HandHeart size={30} /></div>
                <div>
                    <span className="doacoes-subtitulo">LEIA & COMPARTILHE</span>
                    <h2>Doações</h2>
                    <p>{ehAluno
                        ? 'Compartilhe livros e acompanhe suas contribuições.'
                        : 'Acompanhe e analise as doações recebidas pela plataforma.'}</p>
                </div>
            </section>

            {erro && <div className="doacoes-mensagem erro" role="alert"><CircleX size={19} /><span>{erro}</span></div>}
            {mensagem && <div className="doacoes-mensagem sucesso" role="status"><CheckCircle2 size={19} /><span>{mensagem}</span></div>}

            {ehAluno && (
                <section className="doacoes-formulario">
                    <div className="doacoes-titulo">
                        <div>
                            <h3>Cadastrar nova doação</h3>
                            <p>Preencha os dados do livro que deseja compartilhar.</p>
                        </div>
                        <HandHeart size={24} />
                    </div>

                    <form onSubmit={enviarDoacao}>
                        <div className="doacoes-form-grid">
                            <div className="doacoes-campo">
                                <label htmlFor="titulo">Título do livro</label>
                                <input id="titulo" name="titulo" value={formulario.titulo} onChange={atualizarCampo} required placeholder="Nome do livro" />
                            </div>
                            <div className="doacoes-campo">
                                <label htmlFor="autor">Autor</label>
                                <input id="autor" name="autor" value={formulario.autor} onChange={atualizarCampo} required placeholder="Nome do autor" />
                            </div>
                            <div className="doacoes-campo">
                                <label htmlFor="genero">Gênero</label>
                                <input id="genero" name="genero" value={formulario.genero} onChange={atualizarCampo} required placeholder="Ex.: Romance" />
                            </div>
                            <div className="doacoes-campo">
                                <label htmlFor="condicao">Condição do livro</label>
                                <select id="condicao" name="condicao" value={formulario.condicao} onChange={atualizarCampo} required>
                                    <option value="">Selecione a condição</option>
                                    <option value="Novo">Novo</option>
                                    <option value="Bom">Bom</option>
                                    <option value="Regular">Regular</option>
                                    <option value="Ruim">Ruim</option>
                                </select>
                            </div>
                            <div className="doacoes-campo campo-completo">
                                <label htmlFor="observacoes">Observações</label>
                                <textarea id="observacoes" name="observacoes" value={formulario.observacoes} onChange={atualizarCampo} placeholder="Informações adicionais sobre o livro (opcional)" rows="3" />
                            </div>
                        </div>
                        <button className="botao-doacao" type="submit" disabled={processando}>
                            <HandHeart size={17} />
                            {processando ? 'Enviando...' : 'Cadastrar doação'}
                        </button>
                    </form>
                </section>
            )}

            <section className="doacoes-conteudo">
                <div className="doacoes-titulo">
                    <div>
                        <h3>{ehAluno ? 'Minhas doações' : 'Doações recebidas'}</h3>
                        <p>{ehAluno ? 'Acompanhe o andamento das suas contribuições.' : 'Consulte e analise os livros oferecidos.'}</p>
                    </div>
                    <span className="doacoes-contagem">{doacoes.length} {doacoes.length === 1 ? 'registro' : 'registros'}</span>
                </div>

                {doacoes.length === 0 ? (
                    <div className="doacoes-vazio">
                        <BookOpen size={38} />
                        <h3>Nenhuma doação encontrada</h3>
                        <p>Quando houver doações, elas aparecerão nesta área.</p>
                    </div>
                ) : (
                    <div className="doacoes-lista">
                        {doacoes.map(doacao => (
                            <article className="doacao-card" key={doacao.id}>
                                <div className="doacao-card-cabecalho">
                                    <div className="doacao-livro-icone"><BookOpen size={25} /></div>
                                    <div className="doacao-livro-titulo">
                                        <span>DOAÇÃO #{doacao.id}</span>
                                        <h3>{doacao.titulo}</h3>
                                    </div>
                                    <span className={`doacao-status ${obterStatus(doacao.status)}`}>{doacao.status || 'Não informado'}</span>
                                </div>

                                <div className="doacao-detalhes">
                                    {!ehAluno && (
                                        <div className="doacao-detalhe">
                                            <UserRound size={17} />
                                            <div><span>Doador</span><strong>{doacao.nomeDoador || 'Não informado'}</strong></div>
                                        </div>
                                    )}
                                    <div className="doacao-detalhe">
                                        <BookOpen size={17} />
                                        <div><span>Autor</span><strong>{doacao.autor}</strong></div>
                                    </div>
                                    <div className="doacao-detalhe">
                                        <Tag size={17} />
                                        <div><span>Gênero</span><strong>{doacao.genero}</strong></div>
                                    </div>
                                    <div className="doacao-detalhe">
                                        <ClipboardList size={17} />
                                        <div><span>Condição</span><strong>{doacao.condicao}</strong></div>
                                    </div>
                                </div>

                                {doacao.observacoes && (
                                    <p className="doacao-observacoes"><strong>Observações:</strong> {doacao.observacoes}</p>
                                )}

                                {!ehAluno && (doacao.status === 'Pendente' || doacao.status === 'AguardandoAnalise') && (
                                    <div className="doacao-analise">
                                        <label htmlFor={`parecer-${doacao.id}`}>Parecer da análise</label>
                                        <textarea
                                            id={`parecer-${doacao.id}`}
                                            value={pareceres[doacao.id] || ''}
                                            onChange={evento => setPareceres(atuais => ({ ...atuais, [doacao.id]: evento.target.value }))}
                                            placeholder="Descreva o parecer sobre a doação..."
                                            rows="2"
                                        />
                                        <div className="doacao-acoes">
                                            <button type="button" className="botao-doacao-acao aprovar" disabled={processando} onClick={() => analisarDoacao(doacao.id, true)}>
                                                <CheckCircle2 size={16} /> Aprovar
                                            </button>
                                            <button type="button" className="botao-doacao-acao reprovar" disabled={processando} onClick={() => analisarDoacao(doacao.id, false)}>
                                                <CircleX size={16} /> Reprovar
                                            </button>
                                        </div>
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

export default Doacoes