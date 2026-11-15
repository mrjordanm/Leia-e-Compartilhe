import { Link } from 'react-router-dom'
import {
    BookOpen,
    Repeat,
    HandHeart,
    Handshake,
    ArrowRight,
    Library,
} from 'lucide-react'
import { useAuth } from '../../contexts/useAuth'
import './Dashboard.css'

const atalhos = [
    {
        titulo: 'Catálogo de livros',
        descricao: 'Explore os livros disponíveis na plataforma.',
        destino: '/livros',
        icone: BookOpen,
        cor: 'azul',
    },
    {
        titulo: 'Empréstimos',
        descricao: 'Consulte e acompanhe solicitações de empréstimo.',
        destino: '/emprestimos',
        icone: Repeat,
        cor: 'verde',
    },
    {
        titulo: 'Doações',
        descricao: 'Acompanhe as doações e contribua com novos livros.',
        destino: '/doacoes',
        icone: HandHeart,
        cor: 'laranja',
    },
    {
        titulo: 'Parcerias',
        descricao: 'Consulte as parcerias da iniciativa.',
        destino: '/parcerias',
        icone: Handshake,
        cor: 'roxo',
    },
]

function Dashboard({ titulo, descricao }) {
    const { usuario } = useAuth()

    return (
        <div className="dashboard">
            <section className="dashboard-welcome">
                <div>
                    <span className="dashboard-eyebrow">
                        PLATAFORMA DE INCLUSÃO LITERÁRIA
                    </span>

                    <h2>{titulo}</h2>

                    <p>
                        Olá, {usuario?.nome}! {descricao}
                    </p>
                </div>

                <div className="dashboard-welcome-icon">
                    <Library size={42} />
                </div>
            </section>

            <section className="dashboard-section">
                <div className="dashboard-section-heading">
                    <div>
                        <h3>Acesse os módulos</h3>
                        <p>Escolha uma área para continuar.</p>
                    </div>
                </div>

                <div className="dashboard-grid">
                    {atalhos.map((atalho) => {
                        const Icone = atalho.icone

                        return (
                            <Link
                                to={atalho.destino}
                                className="dashboard-card"
                                key={atalho.destino}
                            >
                                <div className={`dashboard-card-icon ${atalho.cor}`}>
                                    <Icone size={23} />
                                </div>

                                <h4>{atalho.titulo}</h4>
                                <p>{atalho.descricao}</p>

                                <span className="dashboard-card-link">
                                    Acessar módulo
                                    <ArrowRight size={16} />
                                </span>
                            </Link>
                        )
                    })}
                </div>
            </section>

            <section className="dashboard-about">
                <div className="dashboard-about-icon">
                    <BookOpen size={22} />
                </div>
                <div>
                    <h3>Compartilhar conhecimento transforma vidas</h3>
                    <p>
                        O Leia & Compartilhe promove o acesso à leitura por meio
                        da circulação de livros, doações e colaboração.
                    </p>
                </div>
            </section>
        </div>
    )
}

export default Dashboard