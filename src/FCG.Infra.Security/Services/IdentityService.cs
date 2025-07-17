using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using FCG.Infra.Security.Configurations;
using FCG.Infra.Security.Constants;
using FCG.Infra.Security.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace FCG.Infra.Security.Services
{
    public class IdentityService : IIdentityService
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly JwtOptions _jwtOptions;

        public IdentityService(SignInManager<IdentityUser> signInManager,
                               UserManager<IdentityUser> userManager,
                               IOptions<JwtOptions> jwtOptions)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _jwtOptions = jwtOptions.Value;
        }

        public async Task<IdentityRegisterResponse> CadastrarUsuario(string email, string senha)
        {
            var identityUser = new IdentityUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true
            };

            var result = await _userManager.CreateAsync(identityUser, senha);
            if (!result.Succeeded)
                return new IdentityRegisterResponse(false, result.Errors.Select(r => r.Description).ToList());

            await _userManager.SetLockoutEnabledAsync(identityUser, false);
            await _userManager.AddToRoleAsync(identityUser, Roles.USUARIO);

            return new IdentityRegisterResponse(true);
        }

        public async Task<IdentityTokenResponse> Login(string email, string senha)
        {
            var result = await _signInManager.PasswordSignInAsync(email, senha, false, true);
            if (!result.Succeeded)
            {
                if (result.IsLockedOut || result.IsNotAllowed)
                    return new IdentityTokenResponse("Conta bloqueada.");
                else if (result.RequiresTwoFactor)
                    return new IdentityTokenResponse("É necessário realizar o duplo fator de autenticação.");
                else
                    return new IdentityTokenResponse("Credenciais inválidas.");
            }

            return await GerarCredenciais(email);
        }

        #region PRIVATE

        private async Task<IdentityTokenResponse> GerarCredenciais(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            var accessTokenClaims = await ObterClaims(user, adicionarClaimsUsuario: true);
            var refreshTokenClaims = await ObterClaims(user, adicionarClaimsUsuario: false);

            var dataExpiracaoAccessToken = DateTime.Now.AddSeconds(_jwtOptions.AccessTokenExpiration);
            var dataExpiracaoRefreshToken = DateTime.Now.AddSeconds(_jwtOptions.RefreshTokenExpiration);

            var accessToken = GerarToken(accessTokenClaims, dataExpiracaoAccessToken);
            var refreshToken = GerarToken(refreshTokenClaims, dataExpiracaoRefreshToken);

            return new IdentityTokenResponse(accessToken, refreshToken);
        }

        private string GerarToken(IEnumerable<Claim> claims, DateTime dataExpiracao)
        {
            var jwt = new JwtSecurityToken(
                issuer: _jwtOptions.Issuer,
                audience: _jwtOptions.Audience,
                claims: claims,
                notBefore: DateTime.Now,
                expires: dataExpiracao,
                signingCredentials: _jwtOptions.SigningCredentials);

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }

        private async Task<IList<Claim>> ObterClaims(IdentityUser user, bool adicionarClaimsUsuario)
        {
            var claims = new List<Claim>();

            claims.Add(new Claim(JwtRegisteredClaimNames.Sub, user.Id));
            claims.Add(new Claim(JwtRegisteredClaimNames.Email, user.Email));
            claims.Add(new Claim(JwtRegisteredClaimNames.Name, user.UserName));
            claims.Add(new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName));
            claims.Add(new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Nbf, DateTime.Now.ToString()));
            claims.Add(new Claim(JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds().ToString()));

            if (adicionarClaimsUsuario)
            {
                var userClaims = await _userManager.GetClaimsAsync(user);
                var roles = await _userManager.GetRolesAsync(user);

                claims.AddRange(userClaims);

                foreach (var role in roles)
                    claims.Add(new Claim("role", role));
            }

            return claims;
        }

        #endregion
    }
}