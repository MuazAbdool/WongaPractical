using AuthSystem.Application.DTOs;
using AuthSystem.Application.Interfaces;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace AuthSystem.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordService _passwordService;
    private readonly ITokenService _tokenService;
    private readonly ILogger<AuthService> _logger;

    public AuthService(
        IUserRepository userRepository,
        IPasswordService passwordService,
        ITokenService tokenService,
        ILogger<AuthService> logger)
    {
        _userRepository = userRepository;
        _passwordService = passwordService;
        _tokenService = tokenService;
        _logger = logger;
    }

    public async Task<AuthResponse> RegisterAsync(RegisterRequest request)
    {
        _logger.LogInformation("Registering user with email {Email}", request.Email);

        if (await _userRepository.EmailExistsAsync(request.Email))
        {
            _logger.LogWarning("Email already exists: {Email}", request.Email);
            throw new InvalidOperationException("Email already exists");
        }

        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = request.FirstName,
            LastName = request.LastName,
            Email = request.Email,
            PasswordHash = _passwordService.HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        var createdUser = await _userRepository.CreateAsync(user);
        var token = _tokenService.GenerateToken(createdUser);

        return new AuthResponse
        {
            Token = token,
            User = MapToUserResponse(createdUser)
        };
    }

    public async Task<AuthResponse> LoginAsync(LoginRequest request)
    {
        _logger.LogInformation("Login attempt for email {Email}", request.Email);

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            _logger.LogWarning("Login failed: user not found for email {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        if (!_passwordService.VerifyPassword(request.Password, user.PasswordHash))
        {
            _logger.LogWarning("Login failed: invalid password for email {Email}", request.Email);
            throw new UnauthorizedAccessException("Invalid credentials");
        }

        var token = _tokenService.GenerateToken(user);

        return new AuthResponse
        {
            Token = token,
            User = MapToUserResponse(user)
        };
    }

    public async Task<UserResponse> GetCurrentUserAsync(Guid userId)
    {
        _logger.LogInformation("Getting current user {UserId}", userId);

        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", userId);
            throw new KeyNotFoundException($"User with id {userId} not found");
        }

        return MapToUserResponse(user);
    }

    private static UserResponse MapToUserResponse(User user) => new()
    {
        Id = user.Id,
        FirstName = user.FirstName,
        LastName = user.LastName,
        Email = user.Email,
        CreatedAt = user.CreatedAt
    };
}
