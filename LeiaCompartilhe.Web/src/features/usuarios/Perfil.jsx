import { useEffect, useState } from 'react'
import { UserRound, Mail, GraduationCap, Building2 } from 'lucide-react'
import { buscarPerfil } from './perfilService'
import './Perfil.css'

function Perfil() {
    const [perfil, setPerfil] = useState(null)
    const [erro, setErro] = useState('')

    useEffect(() => {
        buscarPerfil()
            .then(setPerfil)
            .catch(erro => setErro(erro.message))
    }, [])

    if (erro) return <p>{erro}</p>
    if (!perfil) return <p>Carregando perfil...</p>

    const campos = [
        { label: 'Nome completo', valor: perfil.nome, icone: UserRound },
        { label: 'E-mail', valor: perfil.email, icone: Mail },
        { label: 'RU', valor: perfil.ru || 'Não informado', icone: GraduationCap },
        { label: 'Instituição', valor: perfil.instituicao, icone: Building2 },
    ]

    return (
        <div className="pagina-perfil">
            <section className="perfil-introducao">
                <div className="perfil-avatar"><UserRound size={32} /></div>
                <div>
                    <h2>Meu perfil</h2>
                    <p>Consulte seus dados cadastrados na plataforma.</p>
                </div>
            </section>

            <section className="perfil-card">
                <h3>Informações pessoais</h3>
                <div className="perfil-grid">
                    {campos.map(campo => {
                        const Icone = campo.icone
                        return (
                            <div className="perfil-campo" key={campo.label}>
                                <Icone size={20} />
                                <div>
                                    <span>{campo.label}</span>
                                    <strong>{campo.valor}</strong>
                                </div>
                            </div>
                        )
                    })}
                </div>
            </section>
        </div>
    )
}

export default Perfil