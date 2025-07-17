using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FCG.Infra.Security.Constants
{
    public class Roles
    {
        public const string USUARIO = nameof(USUARIO);
        public const string ADMINISTRADOR = nameof(ADMINISTRADOR);

        public static List<string> ObterListaRoles()
        {
            return new List<string>()
            {
                USUARIO,
                ADMINISTRADOR
            };
        }
    }
}