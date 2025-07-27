using FCG.Domain.Exceptions;

namespace FCG.Domain.Entities
{
    public class Usuario : IEntity, IAggregateRoot
    {
        public Guid Id { get; private set; }
        public string Nome { get; private set; }
        public string Email { get; private set; }
        public DateTime CriadoEm { get; private set; }
        public DateTime? ModificadoEm { get; private set; }

        private readonly List<UsuarioJogo> _jogos = new();
        public IReadOnlyCollection<UsuarioJogo> Jogos => _jogos.AsReadOnly();

        protected Usuario() { }

        public Usuario(string nome, string email)
        {
            Id = Guid.NewGuid();
            Nome = nome;
            Email = email;
            CriadoEm = DateTime.Now;
        }

        public void AlterarNome(string nome)
        {
            Nome = nome;
            ModificadoEm = DateTime.Now;
        }

        public void AdicionarJogo(UsuarioJogo usuarioJogo)
        {
            if (_jogos.Any(x => x.JogoId == usuarioJogo.JogoId))
                throw new DomainException("Usuário já possui este jogo.");

            _jogos.Add(usuarioJogo);
        }

        public void RemoverJogo(Guid jogoId)
        {
            var jogo = _jogos.FirstOrDefault(uj => uj.JogoId == jogoId);
            if (jogo is null)
                throw new DomainException("Usuário não possui este jogo.");

            _jogos.Remove(jogo);
        }
    }
}