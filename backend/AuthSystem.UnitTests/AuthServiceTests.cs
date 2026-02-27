using AuthSystem.Application.DTOs;
using AuthSystem.Application.Interfaces;
using AuthSystem.Application.Services;
using AuthSystem.Domain.Entities;
using AuthSystem.Domain.Interfaces;
using Microsoft.Extensions.Logging;
using Moq;

namespace AuthSystem.UnitTests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordService> _passwordServiceMock;
    private readonly Mock<ITokenService> _tokenServiceMock;
    private readonly Mock<ILogger<AuthService>> _loggerMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordServiceMock = new Mock<IPasswordService>();
        _tokenServiceMock = new Mock<ITokenService>();
        _loggerMock = new Mock<ILogger<AuthService>>();

        _authService = new AuthService(
            _userRepositoryMock.Object,
            _passwordServiceMock.Object,
            _tokenServiceMock.Object,
            _loggerMock.Object);
    }

    [Fact]
    public async Task Register_WhenEmailAlreadyExists_ThrowsInvalidOperationException()
    {
        _userRepositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(true);

        await Assert.ThrowsAsync<InvalidOperationException>(() =>
            _authService.RegisterAsync(new RegisterRequest
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                Password = "password123"
            }));
    }

    [Fact]
    public async Task Register_WithValidData_ReturnsAuthResponse()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PasswordHash = "hashedpw",
            CreatedAt = DateTime.UtcNow
        };

        _userRepositoryMock.Setup(r => r.EmailExistsAsync(It.IsAny<string>())).ReturnsAsync(false);
        _passwordServiceMock.Setup(p => p.HashPassword(It.IsAny<string>())).Returns("hashedpw");
        _userRepositoryMock.Setup(r => r.CreateAsync(It.IsAny<User>())).ReturnsAsync(user);
        _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<User>())).Returns("test-token");

        var result = await _authService.RegisterAsync(new RegisterRequest
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            Password = "password123"
        });

        Assert.NotNull(result);
        Assert.Equal("test-token", result.Token);
        Assert.Equal("john@example.com", result.User.Email);
    }

    [Fact]
    public async Task Login_WithInvalidEmail_ThrowsUnauthorizedAccessException()
    {
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _authService.LoginAsync(new LoginRequest
            {
                Email = "notfound@example.com",
                Password = "password123"
            }));
    }

    [Fact]
    public async Task Login_WithInvalidPassword_ThrowsUnauthorizedAccessException()
    {
        var user = new User { Id = Guid.NewGuid(), Email = "john@example.com", PasswordHash = "hash" };
        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        _passwordServiceMock.Setup(p => p.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(false);

        await Assert.ThrowsAsync<UnauthorizedAccessException>(() =>
            _authService.LoginAsync(new LoginRequest
            {
                Email = "john@example.com",
                Password = "wrongpassword"
            }));
    }

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsAuthResponse()
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow
        };

        _userRepositoryMock.Setup(r => r.GetByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
        _passwordServiceMock.Setup(p => p.VerifyPassword(It.IsAny<string>(), It.IsAny<string>())).Returns(true);
        _tokenServiceMock.Setup(t => t.GenerateToken(It.IsAny<User>())).Returns("test-token");

        var result = await _authService.LoginAsync(new LoginRequest
        {
            Email = "john@example.com",
            Password = "password123"
        });

        Assert.NotNull(result);
        Assert.Equal("test-token", result.Token);
    }

    [Fact]
    public async Task GetCurrentUser_WhenNotFound_ThrowsKeyNotFoundException()
    {
        _userRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<Guid>())).ReturnsAsync((User?)null);

        await Assert.ThrowsAsync<KeyNotFoundException>(() =>
            _authService.GetCurrentUserAsync(Guid.NewGuid()));
    }

    [Fact]
    public async Task GetCurrentUser_WhenFound_ReturnsUserResponse()
    {
        var userId = Guid.NewGuid();
        var user = new User
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            PasswordHash = "hash",
            CreatedAt = DateTime.UtcNow
        };

        _userRepositoryMock.Setup(r => r.GetByIdAsync(userId)).ReturnsAsync(user);

        var result = await _authService.GetCurrentUserAsync(userId);

        Assert.NotNull(result);
        Assert.Equal(userId, result.Id);
        Assert.Equal("john@example.com", result.Email);
    }
}
