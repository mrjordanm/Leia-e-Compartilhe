
import { useEffect, useState } from 'react'
import { useAuth } from '../../contexts/AuthContext'
import {
    UserPlus,
    GraduationCap,
    RefreshCw,
    UserCheck,
    UserX,
    ChevronDown,
} from 'lucide-react'
import {
    cadastrarEducador,
    listarUsuarios,
    alterarStatusUsuario,
} from './usuariosService'
import './GestaoUsuarios.css'

function GestaoUsuarios() {
    const { usuario } = useAuth()
    const administrador = usuario?.tipoUsuario === 'Administrador'

    const [usuarios, setUsuarios] = useState([])
    const [filtroStatus, setFiltroStatus] = useState('Todos')
    const [filtroTipo, setFiltroTipo] = useState('Todos')
    const [cadastroAberto, setCadastroAberto] = useState(false)
    const [formulario, setFormulario] = useState({
        nome: '',
        email: '',
        senha: '',
        ru: '',
        instituicao: '',
    })
    const [carregando, setCarregando] = useState(false)
    const [mensagem, setMensagem] = useState('')
    const [tipoMensagem, setTipoMensagem] = useState('')

    async function carregarUsuarios() {
        try {
            setCarregando(true)
            const dados = await listarUsuarios()
            setUsuarios(dados)
        } catch (erro) {
            setMensagem(erro.message)
            setTipoMensagem('erro')
        } finally {
            setCarregando(false)
        }
    }

    useEffect(() => {
        carregarUsuarios()
    }, [])

    const usuariosFiltrados = usuarios.filter((item) => {
        const statusCorresponde =
            filtroStatus === 'Todos' || item.status === filtroStatus

        const tipoCorresponde =
            filtroTipo === 'Todos' || item.tipoUsuario === filtroTipo

        return statusCorresponde && tipoCorresponde
    })

    function atualizarCampo(evento) {
        const { name, value } = evento.target
        setFormulario((anterior) => ({ ...anterior, [name]: value }))
    }

    async function enviarFormulario(evento) {
        evento.preventDefault()
        setMensagem('')

        if (
            !formulario.nome.trim() ||
            !formulario.email.trim() ||
            !formulario.senha ||
            !formulario.ru.trim() ||
            !formulario.instituicao.trim()
        ) {
            setMensagem('Preencha todos os campos.')
            setTipoMensagem('erro')
            return
        }

        try {
            setCarregando(true)
            await cadastrarEducador(formulario)
            setMensagem('Educador cadastrado com sucesso!')
            setTipoMensagem('sucesso')
            setFormulario({
                nome: '',
                email: '',
                senha: '',
                ru: '',
                instituicao: '',
            })
            setCadastroAberto(false)
            await carregarUsuarios()
        } catch (erro) {
            setMensagem(erro.message)
            setTipoMensagem('erro')
        } finally {
            setCarregando(false)
        }
    }

    async function alternarStatus(usuarioAlvo) {
        const ativo = usuarioAlvo.status !== 'Ativo'

        if (!window.confirm(
            `${ativo ? 'Ativar' : 'Desativar'} a conta de ${usuarioAlvo.nome}?`
        )) return

        try {
            await alterarStatusUsuario(usuarioAlvo.id, ativo)
            setMensagem('Status atualizado com sucesso!')
            setTipoMensagem('sucesso')
            await carregarUsuarios()
        } catch (erro) {
            setMensagem(erro.message)
            setTipoMensagem('erro')
        }
    }

    return (
        <div className="gestao-usuarios">
            <section className="usuarios-introducao">
                <div className="usuarios-introducao-icone">
                    <GraduationCap size={30} />
                </div>
                <div>
                    <h2>Gestão de usuários</h2>
                    <p>
                        {administrador
                            ? 'Gerencie os acessos à plataforma Leia & Compartilhe.'
                            : 'Consulte os alunos cadastrados na plataforma.'}
                    </p>
                </div>
            </section>

            {mensagem && (
                <div className={`mensagem-usuario ${tipoMensagem}`}>
                    {mensagem}
                </div>
            )}

            {administrador && (
                <section className="usuarios-formulario-container">
                    <button
                        type="button"
                        className="usuarios-formulario-titulo"
                        onClick={() => setCadastroAberto(!cadastroAberto)}
                    >
                        <div>
                            <h3>Cadastrar educador</h3>
                            <p>Preencha os dados para criar um novo acesso.</p>
                        </div>
                        <ChevronDown
                            size={22}
                            className={cadastroAberto ? 'icone-aberto' : ''}
                        />
                    </button>

                    {cadastroAberto && (
                        <form onSubmit={enviarFormulario}>
                            <div className="campo-usuario">
                                <label htmlFor="nome">Nome completo</label>
                                <input id="nome" name="nome" value={formulario.nome} onChange={atualizarCampo} required />
                            </div>

                            <div className="campo-usuario">
                                <label htmlFor="email">E-mail</label>
                                <input id="email" name="email" type="email" value={formulario.email} onChange={atualizarCampo} required />
                            </div>

                            <div className="campo-usuario">
                                <label htmlFor="ru">RU</label>
                                <input
                                    id="ru"
                                    name="ru"
                                    inputMode="numeric"
                                    pattern="[0-9]+"
                                    value={formulario.ru}
                                    onChange={(evento) => setFormulario((anterior) => ({
                                        ...anterior,
                                        ru: evento.target.value.replace(/\D/g, ''),
                                    }))}
                                    required
                                />
                            </div>

                            <div className="campo-usuario">
                                <label htmlFor="instituicao">Instituição</label>
                                <input id="instituicao" name="instituicao" value={formulario.instituicao} onChange={atualizarCampo} maxLength={150} required />
                            </div>

                            <div className="campo-usuario">
                                <label htmlFor="senha">Senha inicial</label>
                                <input id="senha" name="senha" type="password" value={formulario.senha} onChange={atualizarCampo} required />
                            </div>

                            <button type="submit" className="botao-cadastrar-educador" disabled={carregando}>
                                {carregando ? 'Processando...' : 'Cadastrar educador'}
                            </button>
                        </form>
                    )}
                </section>
            )}

            <section className="usuarios-formulario-container">
                <div className="usuarios-formulario-titulo">
                    <div>
                        <h3>{administrador ? 'Usuários cadastrados' : 'Alunos cadastrados'}</h3>
                        <p>{usuariosFiltrados.length} registro(s) encontrado(s)</p>
                    </div>
                    <button type="button" onClick={carregarUsuarios} title="Atualizar lista">
                        <RefreshCw size={20} />
                    </button>
                </div>

                
                <div className="usuarios-filtro">
                    <label htmlFor="filtroStatus">Status:</label>
                    <select
                        id="filtroStatus"
                        value={filtroStatus}
                        onChange={(evento) => setFiltroStatus(evento.target.value)}
                    >
                        <option value="Todos">Todos</option>
                        <option value="Ativo">Ativos</option>
                        <option value="Inativo">Inativos</option>
                    </select>

                    {administrador && (
                        <>
                            <label htmlFor="filtroTipo">Tipo de usuário:</label>
                            <select
                                id="filtroTipo"
                                value={filtroTipo}
                                onChange={(evento) => setFiltroTipo(evento.target.value)}
                            >
                                <option value="Todos">Todos</option>
                                <option value="Administrador">Administradores</option>
                                <option value="Educador">Educadores</option>
                                <option value="Aluno">Alunos</option>
                            </select>
                        </>
                    )}
                </div>

                {carregando && usuarios.length === 0 ? (
                    <p>Carregando usuários...</p>
                ) : usuariosFiltrados.length === 0 ? (
                    <p>Nenhum usuário encontrado para esse filtro.</p>
                ) : (
                    <div className="usuarios-lista">
                        {usuariosFiltrados.map((item) => (
                            <article className="usuario-card" key={item.id}>
                                <div>
                                    <h4>{item.nome}</h4>
                                    <p>{item.email}</p>
                                    <p>RU: {item.ru || 'Não informado'}</p>
                                    <span>{item.tipoUsuario}</span>
                                    <span className={item.status === 'Ativo' ? 'status-ativo' : 'status-inativo'}>
                                        {item.status}
                                    </span>
                                </div>

                                {administrador && item.id !== usuario?.id && (
                                    <button
                                        type="button"
                                        onClick={() => alternarStatus(item)}
                                        disabled={carregando}
                                    >
                                        {item.status === 'Ativo'
                                            ? <><UserX size={16} /> Desativar</>
                                            : <><UserCheck size={16} /> Ativar</>}
                                    </button>
                                )}
                            </article>
                        ))}
                    </div>
                )}
            </section>
        </div>
    )
}

export default GestaoUsuarios