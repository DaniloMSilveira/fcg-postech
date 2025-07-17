using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCG.Domain.Entities
{
    public interface IEntity
    {
        Guid Id { get; }
        public DateTime CriadoEm { get; }
        public DateTime? ModificadoEm { get; }
    }
}