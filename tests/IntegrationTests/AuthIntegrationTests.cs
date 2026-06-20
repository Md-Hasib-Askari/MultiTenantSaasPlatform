using Microsoft.AspNetCore.Mvc.Testing;
using Testcontainers.PostgreSql;

namespace IntegrationTests;

public class AuthIntegrationTests : IAsyncLifetime
{
    private readonly PostgreSqlContainer _postgres = new PostgreSqlBuilder("postgres:16-alpine")
        .WithDatabase("mtsp_auth_test")
        .WithUsername("mtsp")
        .WithPassword("mtsp")
        .Build();

    private WebApplicationFactory<Program> _factory = null!;
    private HttpClient _client = null!;
    private string _connectionString = null!;

    public async Task InitializeAsync()
    {
        await _postgres.StartAsync();
        _connectionString = _postgres.GetConnectionString();

        _factory = new WebApplicationFactory<Program>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("ConnectionStrings:Postgres", _connectionString);
            builder.UseSetting("ConnectionStrings:Redis", "localhost:6379");
        });
    }

    public Task DisposeAsync()
    {
        throw new NotImplementedException();
    }

    // -----------------------------
    // Registration Tests
    // -----------------------------

    [Fact]
    public async Task Register_WithValidData_ReturnsTokens() { }

    [Fact]
    public async Task Register_WithDuplicateEmail_ReturnsBadRequest() { }

    [Fact]
    public async Task Register_WithNonexistentTenant_ReturnsBadRequest() { }

    [Fact]
    public async Task Register_WithWeakPassword_ReturnsBadRequest() { }

    // -----------------------------
    // Login Tests
    // -----------------------------

    [Fact]
    public async Task Login_WithValidCredentials_ReturnsTokens() { }

    [Fact]
    public async Task Login_WithWrongPassword_ReturnsUnauthorized()
    {
        //Given

        //When

        //Then
    }

    [Fact]
    public async Task Login_WithNonexistentUser_ReturnsUnauthorized()
    {
        //Given

        //When

        //Then
    }

    // -----------------------------
    // Token Refresh Tests
    // -----------------------------

    [Fact]
    public async Task Refresh_WithValidToken_ReturnsNewTokens()
    {
        //Given

        //When

        //Then
    }

    [Fact]
    public async Task Refresh_WithUsedToken_ReturnsUnauthorized()
    {
        //Given

        //When

        //Then
    }

    [Fact]
    public async Task Refresh_WithBogusToken_ReturnsUnauthorized()
    {
        //Given

        //When

        //Then
    }

    // -----------------------------
    // Role Verification in JWT Claims
    // -----------------------------

    [Fact]
    public async Task Register_AssignsMemberRole()
    {
        //Given

        //When

        //Then
    }

    [Fact]
    public async Task Token_ContainsAllRequiredClaims()
    {
        //Given

        //When

        //Then
    }
}
