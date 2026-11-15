
import { useEffect, useState } from 'react'
import { listarRegistrosAuditoria } from './auditoriaService'
import { ClipboardList, RefreshCw } from 'lucide-react'
import './Auditoria.css'

function Auditoria() {
    const [registros, setRegistros] = useState([])
    const [carregando, setCarregando] = useState(true)
    const [erro, setErro] = useState('')

    async function carregarRegistros() {
        try {
            setCarregando(true)
            setErro('')
            const dados = await listarRegistrosAuditoria()
            setRegistros(dados)
        } catch (error) {
            setErro(error.message)
        } finally {
            setCarregando(false)
        }
    }

    useEffect(() => {
        carregarRegistros()
    }, [])

    function formatarData(data) {
        return new Date(data).toLocaleString('pt-BR')
    }

    return (
        <main className="auditoria-page">
            <header className="auditoria-header">
                <div className="auditoria-icon">
                    <ClipboardList size={28} />
                </div>
                <div>
                    <span className="auditoria-eyebrow">GESTÃO DO SISTEMA</span>
                    <h1>Auditoria</h1>
                    <p>Histórico de ações realizadas na plataforma.</p>
                </div>
                <button onClick={carregarRegistros} className="auditoria-refresh">
                    <RefreshCw size={17} />
                    Atualizar
                </button>
            </header>

            {erro && <div className="auditoria-alerta">{erro}</div>}

            <section className="auditoria-content">
                <div className="auditoria-content-header">
                    <div>
                        <h2>Registros de atividade</h2>
                        <p>Acompanhe as operações realizadas pelos usuários.</p>
                    </div>
                    <span className="auditoria-count">{registros.length} registros</span>
                </div>

                {carregando ? (
                    <p className="auditoria-vazio">Carregando registros...</p>
                ) : registros.length === 0 ? (
                    <p className="auditoria-vazio">Nenhuma atividade registrada.</p>
                ) : (
                    <div className="auditoria-tabela-wrapper">
                        <table className="auditoria-tabela">
                            <thead>
                                <tr>
                                    <th>Data e hora</th>
                                    <th>Responsável</th>
                                    <th>Usuário envolvido</th>
                                    <th>Ação</th>
                                    <th>Entidade</th>
                                    <th>ID</th>
                                    <th>Detalhes</th>
                                </tr>
                            </thead>
                            <tbody>
                                {registros.map(registro => (
                                    <tr key={registro.id}>
                                        <td>{formatarData(registro.dataHora)}</td>
                                        <td>{registro.nomeResponsavel}</td>
                                        <td>{registro.nomeUsuarioAlvo || '-'}</td>
                                        <td>{registro.acao}</td>
                                        <td>{registro.entidade}</td>
                                        <td>{registro.entidadeId ?? '-'}</td>
                                        <td>{registro.detalhes || '-'}</td>
                                    </tr>
                                ))}
                            </tbody>
                        </table>
                    </div>
                )}
            </section>
        </main>
    )
}

export default Auditoria
