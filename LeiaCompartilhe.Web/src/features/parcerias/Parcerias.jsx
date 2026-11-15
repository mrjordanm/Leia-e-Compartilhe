
import { useEffect, useState } from 'react'
import { Handshake, PlusCircle, ChevronDown } from 'lucide-react'
import { useAuth } from '../../contexts/useAuth'
import { listarParcerias, cadastrarParceria } from './parceriasService'
import './Parcerias.css'

function Parcerias() {
    const { usuario } = useAuth()
    const administrador = usuario?.tipoUsuario === 'Administrador'
    const [cadastroAberto, setCadastroAberto] = useState(false)
    const [parcerias, setParcerias] = useState([])
    const [carregando, setCarregando] = useState(true)
    const [enviando, setEnviando] = useState(false)
    const [mensagem, setMensagem] = useState('')
    const [tipoMensagem, setTipoMensagem] = useState('')
    const [formulario, setFormulario] = useState({
        nomeInstituicao: '',
        tipoParceria: '',
        descricao: '',
        contato: ''
    })

    async function carregarParcerias() {
        try {
            const dados = await listarParcerias()
            setParcerias(Array.isArray(dados) ? dados : [])
        } catch (erro) {
            setMensagem(erro.message)
            setTipoMensagem('erro')
        } finally {
            setCarregando(false)
        }
    }

    useEffect(() => {
        carregarParcerias()
    }, [])

    function atualizarCampo(evento) {
        const { name, value } = evento.target
        setFormulario((anterior) => ({
            ...anterior,
            [name]: value
        }))
    }

    async function enviarFormulario(evento) {
        evento.preventDefault()
        setMensagem('')

        if (!formulario.nomeInstituicao.trim() || !formulario.tipoParceria.trim()) {
            setMensagem('Preencha o nome da instituição e o tipo de parceria.')
            setTipoMensagem('erro')
            return
        }

        try {
            setEnviando(true)
            await cadastrarParceria(formulario)

            setFormulario({
                nomeInstituicao: '',
                tipoParceria: '',
                descricao: '',
                contato: ''
            })

            setMensagem('Parceria cadastrada com sucesso!')
            setTipoMensagem('sucesso')
            await carregarParcerias()
        } catch (erro) {
            setMensagem(erro.message)
            setTipoMensagem('erro')
        } finally {
            setEnviando(false)
        }
    }

    if (carregando) return <p>Carregando parcerias...</p>

    return (
        <div className="gestao-parcerias">
            <section className="parcerias-introducao">
                <div className="parcerias-introducao-icone">
                    <Handshake size={30} />
                </div>
                <div>
                    <h2>Parcerias</h2>
                    <p>
                        Consulte as instituições parceiras da plataforma
                        Leia & Compartilhe.
                    </p>
                </div>
            </section>

            {administrador && (
                <section className="parcerias-formulario-container">
                    <button
                        type="button"
                        className="parcerias-formulario-titulo"
                        onClick={() => setCadastroAberto(!cadastroAberto)}
                    >
                        <div>
                            <h3>Cadastrar parceria</h3>
                            <p>Preencha os dados para registrar uma instituição parceira.</p>
                        </div>

                        <ChevronDown
                            size={24}
                            className={cadastroAberto ? 'menu-chevron aberto' : 'menu-chevron'}
                        />
                    </button>

                    {cadastroAberto && (
                        <form onSubmit={enviarFormulario}>
                        <div className="campo-parceria">
                            <label htmlFor="nomeInstituicao">Nome da instituição</label>
                            <input
                                id="nomeInstituicao"
                                name="nomeInstituicao"
                                value={formulario.nomeInstituicao}
                                onChange={atualizarCampo}
                                placeholder="Digite o nome da instituição"
                                maxLength={150}
                                required
                            />
                        </div>

                        <div className="campo-parceria">
                            <label htmlFor="tipoParceria">Tipo de parceria</label>
                            <input
                                id="tipoParceria"
                                name="tipoParceria"
                                value={formulario.tipoParceria}
                                onChange={atualizarCampo}
                                placeholder="Ex.: Doação de livros, apoio educacional"
                                maxLength={100}
                                required
                            />
                        </div>

                        <div className="campo-parceria">
                            <label htmlFor="descricao">Descrição</label>
                            <textarea
                                id="descricao"
                                name="descricao"
                                value={formulario.descricao}
                                onChange={atualizarCampo}
                                placeholder="Descreva a parceria (opcional)"
                                maxLength={1000}
                                rows={3}
                            />
                        </div>

                        <div className="campo-parceria">
                            <label htmlFor="contato">Contato</label>
                            <input
                                id="contato"
                                name="contato"
                                value={formulario.contato}
                                onChange={atualizarCampo}
                                placeholder="E-mail ou telefone para contato"
                                maxLength={150}
                            />
                        </div>

                        {mensagem && (
                            <div className={`mensagem-parceria ${tipoMensagem}`}>
                                {mensagem}
                            </div>
                        )}

                        <button
                            type="submit"
                            className="botao-cadastrar-parceria"
                            disabled={enviando}
                        >
                            {enviando ? 'Cadastrando...' : 'Cadastrar parceria'}
                        </button>
                        </form>
                    )}
                </section>
            )}

            <section className="parcerias-listagem">
                <h3>Instituições parceiras</h3>

                {parcerias.length === 0 ? (
                    <p>Nenhuma parceria cadastrada no momento.</p>
                ) : (
                    <div className="parcerias-lista">
                        {parcerias.map((parceria) => (
                            <article className="parceria-card" key={parceria.id}>
                                <h4>{parceria.nomeInstituicao}</h4>
                                <p><strong>Tipo:</strong> {parceria.tipoParceria}</p>
                                <p><strong>Contato:</strong> {parceria.contato || 'Não informado'}</p>
                                <p><strong>Descrição:</strong> {parceria.descricao || 'Sem descrição cadastrada.'}</p>
                            </article>
                        ))}
                    </div>
                )}
            </section>
        </div>
    )
}

export default Parcerias
