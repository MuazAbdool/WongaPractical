using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AuthSystem.Application.DTOs;

namespace AuthSystem.IntegrationTests;

public class AuthControllerIntegrationTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

    public AuthControllerIntegrationTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Register_WithValidData_Returns201()
    {
        var request = new RegisterRequest
        {
            FirstName = "Integration",
            LastName = "Test",
            Email = $"integration_{Guid.NewGuid()}@test.com",
            Password = "password123"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, _jsonOptions);
        Assert.NotNull(authResponse?.Token);
        Assert.Equal(request.Email, authResponse?.User?.Email);
    }

    [Fact]
    public async Task Register_WithDuplicateEmail_Returns409()
    {
        var email = $"duplicate_{Guid.NewGuid()}@test.com";
        var request = new RegisterRequest
        {
            FirstName = "Test",
            LastName = "User",
            Email = email,
            Password = "password123"
        };

        await _client.PostAsJsonAsync("/api/auth/register", request);
        var response = await _client.PostAsJsonAsync("/api/auth/register", request);

        Assert.Equal(HttpStatusCode.Conflict, response.StatusCode);
    }

    [Fact]
    public async Task Login_WithValidCredentials_Returns200WithToken()
    {
        var email = $"login_{Guid.NewGuid()}@test.com";
        var password = "password123";

        await _client.PostAsJsonAsync("/api/auth/register", new RegisterRequest
        {
            FirstName = "Login",
            LastName = "Test",
            Email = email,
            Password = password
        });

        var loginRequest = new LoginRequest { Email = email, Password = password };
        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(content, _jsonOptions);
        Assert.NotNull(authResponse?.Token);
    }

    [Fact]
    public async Task Login_WithInvalidCredentials_Returns401()
    {
        var loginRequest = new LoginRequest
        {
            Email = "nonexistent@test.com",
            Password = "wrongpassword"
        };

        var response = await _client.PostAsJsonAsync("/api/auth/login", loginRequest);

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMe_WithoutToken_Returns401()
    {
        var response = await _client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
    }

    [Fact]
    public async Task GetMe_WithValidToken_Returns200()
    {
        var email = $"getme_{Guid.NewGuid()}@test.com";
        var registerRequest = new RegisterRequest
        {
            FirstName = "GetMe",
            LastName = "Test",
            Email = email,
            Password = "password123"
        };

        var registerResponse = await _client.PostAsJsonAsync("/api/auth/register", registerRequest);
        var registerContent = await registerResponse.Content.ReadAsStringAsync();
        var authResponse = JsonSerializer.Deserialize<AuthResponse>(registerContent, _jsonOptions);

        _client.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", authResponse!.Token);

        var meResponse = await _client.GetAsync("/api/auth/me");

        Assert.Equal(HttpStatusCode.OK, meResponse.StatusCode);
        var meContent = await meResponse.Content.ReadAsStringAsync();
        var userResponse = JsonSerializer.Deserialize<UserResponse>(meContent, _jsonOptions);
        Assert.Equal(email, userResponse?.Email);
    }
}
