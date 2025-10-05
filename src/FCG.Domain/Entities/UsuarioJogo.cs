namespace FCG.Domain.Entities
{
    public class UsuarioJogo : IEntity
    {
        public Guid Id { get; private set; }
        public Guid UsuarioId { get; private set; }
        public virtual Usuario Usuario { get; private set; }
        public Guid JogoId { get; private set; }
        public virtual Jogo Jogo { get; private set; }
        public decimal PrecoCompra { get; private set; }
        public Guid? PromocaoId { get; private set; }
        public virtual Promocao? Promocao { get; private set; }
        public DateTime CriadoEm { get; private set; }
        public DateTime? ModificadoEm { get; private set; }

        protected UsuarioJogo() { }

        public UsuarioJogo(Guid usuarioId, Guid jogoId, decimal precoCompra, Guid? promocaoId)
        {
            Id = Guid.NewGuid();
            UsuarioId = usuarioId;
            JogoId = jogoId;
            PrecoCompra = precoCompra;
            PromocaoId = promocaoId;
            CriadoEm = DateTime.Now;
        }
    }
}