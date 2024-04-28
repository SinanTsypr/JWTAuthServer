using JWTAuthServer.Core.Configurations;
using JWTAuthServer.Core.DTOs;
using JWTAuthServer.Core.Models;
using JWTAuthServer.Core.Repositories;
using JWTAuthServer.Core.Services;
using JWTAuthServer.Core.UnitOfWork;
using JWTAuthServer.SharedLibrary.Dtos;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JWTAuthServer.Service.Services
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly List<Client> _clients;
        private readonly ITokenService _tokenService;
        private readonly UserManager<UserApp> _userManager;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IGenericRepository<UserRefreshToken> _userRefreshTokenService;

        public AuthenticationService(IOptions<List<Client>> optionsClient, ITokenService tokenService, UserManager<UserApp> userManager, IUnitOfWork unitOfWork, IGenericRepository<UserRefreshToken> userRefreshTokenService)
        {
            _clients = optionsClient.Value;
            _tokenService = tokenService;
            _userManager = userManager;
            _unitOfWork = unitOfWork;
            _userRefreshTokenService = userRefreshTokenService;
        }

        public async Task<ResponseDto<TokenDto>> CreateTokenAsync(LoginDto loginDto)
        {
            if (loginDto == null) throw new ArgumentNullException(nameof(loginDto));

            var user = await _userManager.FindByEmailAsync(loginDto.Email);

            if (user == null) return ResponseDto<TokenDto>.Fail("Email veya Password yanlış", 404, true);

            if (!await _userManager.CheckPasswordAsync(user, loginDto.Password)) return ResponseDto<TokenDto>.Fail("Email veya Password yanlış", 404, true);

            var token = _tokenService.CreateToken(user);

            var userRefreshToken = await _userRefreshTokenService.Where(x => x.UserId == user.Id).SingleOrDefaultAsync();

            if (userRefreshToken == null)
            {
                await _userRefreshTokenService.AddAsync(new UserRefreshToken { UserId = user.Id, Code = token.RefreshToken, Expration = token.RefreshTokenExpiration });
            }
            else
            {
                userRefreshToken.Code = token.RefreshToken;
                userRefreshToken.Expration = token.RefreshTokenExpiration;
            }

            await _unitOfWork.CommitAsync();

            return ResponseDto<TokenDto>.Success(token, 200);
        }

        public ResponseDto<ClientTokenDto> CreateTokenByClient(ClientLoginDto clientLoginDto)
        {
            var client = _clients.SingleOrDefault(x => x.ClientId == clientLoginDto.ClientId && x.Secret == clientLoginDto.ClientSecret);
            if (client == null)
            {
                return ResponseDto<ClientTokenDto>.Fail("ClientId veya ClientSecret bulunamadı", 404, true);
            }

            var token = _tokenService.CreateTokenByClient(client);

            return ResponseDto<ClientTokenDto>.Success(token, 200);
        }

        public async Task<ResponseDto<TokenDto>> CreateTokenByRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();

            if (existRefreshToken == null)
            {
                return ResponseDto<TokenDto>.Fail("Refresh token bulunamadı", 404, true);
            }

            var user = await _userManager.FindByIdAsync(existRefreshToken.UserId);

            if (user == null)
            {
                return ResponseDto<TokenDto>.Fail("User Id bulunamadı", 404, true);
            }

            var tokenDto = _tokenService.CreateToken(user);

            existRefreshToken.Code = tokenDto.RefreshToken;
            existRefreshToken.Expration = tokenDto.RefreshTokenExpiration;

            await _unitOfWork.CommitAsync();

            return ResponseDto<TokenDto>.Success(tokenDto, 200);
        }

        public async Task<ResponseDto<NoDataDto>> RevokeRefreshToken(string refreshToken)
        {
            var existRefreshToken = await _userRefreshTokenService.Where(x => x.Code == refreshToken).SingleOrDefaultAsync();

            if (existRefreshToken == null)
            {
                return ResponseDto<NoDataDto>.Fail("Refresh token bulunamadı", 404, true);
            }

            _userRefreshTokenService.Remove(existRefreshToken);

            await _unitOfWork.CommitAsync();

            return ResponseDto<NoDataDto>.Success(200);
        }
    }
}
