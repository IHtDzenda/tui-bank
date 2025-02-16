using Core.Db;
using Core.Utils;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins("http://localhost:3000")  // ✅ Replace with frontend URL
            .AllowCredentials()  // ✅ Allows sending cookies
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});
var app = builder.Build();

app.UseHttpsRedirection();

app.MapGet("/test", (HttpContext context) =>
    {
    return Results.Ok("Hello world");
    });


app.MapPost("/auth", async (HttpContext context) =>
    {
    try
    {

    var data = await context.Request.ReadFromJsonAsync<AuthRequest>();

    if (data == null || string.IsNullOrEmpty(data.Email) || string.IsNullOrEmpty(data.Password))
    {
    return Results.BadRequest("Email and Password are required.");
    }

    var result = Utils.auth(data.Email, data.Password);
    var cookieOptions = new CookieOptions
    {
    HttpOnly = false,
    Secure = false,
    SameSite = SameSiteMode.None,
    Expires = DateTime.UtcNow.AddMinutes(30)  
    };

    context.Response.Cookies.Append("email", data.Email, cookieOptions);
    context.Response.Cookies.Append("password", data.Password, cookieOptions);

    }
    catch (System.Exception)
    {
      return Results.Problem("Invalid credentials", statusCode: 500);
    }

    return Results.Ok("Success");

    });
app.MapGet("/user", (HttpContext context) =>
    {
    User user;
    try
    {
    Console.WriteLine("Cookies"+context.Request.Cookies.Count);
    user = Utils.auth(context.Request.Cookies["email"], context.Request.Cookies["password"]);
    }
    catch (System.Exception)
    {
    return Results.Problem("Invalid credentials", statusCode: 500);
    }


    return Results.Ok(user);
    });

app.MapPost("/user", async (HttpContext context) =>
    {
      User user;
    try
    {
    Console.WriteLine("Cookies"+context.Request.Cookies.Count);
    user = Utils.auth(context.Request.Cookies["email"], context.Request.Cookies["password"]);
    }
    catch (System.Exception)
    {
    return Results.Problem("Invalid credentials", statusCode: 500);
    }


    return Results.Ok(user);
    });
app.Run();

app.UseCors("AllowFrontend");
