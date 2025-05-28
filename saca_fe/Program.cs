using SACA_Common.Configurations;
using Saca_Common;
using SACA_FE;
using SACA_FE.Utils;
var builder = WebApplication.CreateBuilder(args);
AppSettings.Instance.SetConfiguration(builder.Configuration);
// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddServices();
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(3000); // ← QUAN TRỌNG
});
builder.Services.AddAuthentication("SACA")
    .AddCookie("SACA", options =>
    {
        options.LoginPath = "/";
        options.AccessDeniedPath = "/Account/AccessDenied";       
        // Cho phép gửi cookie trong môi trường không bảo mật (HTTP)
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax; // Hoặc None nếu dùng cross-origin
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Đặt là Always nếu dùng HTTPS
    });

builder.Services.AddAuthorization();
var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting(); // ??t tr??c Authentication & Authorization

app.UseMiddleware<TokenAuthenticationMiddleware>();
app.UseAuthentication();
app.UseAuthorization(); // ??t sau Authentication

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authen}/{action=Index}/{id?}"
);

app.Run();
