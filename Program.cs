using IMEAutomationDBOperations.Data;
using IMEAutomationDBOperations.Services; // Yeni servisler için bu satır gerekli
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string is not configured properly.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddScoped<IRepository>(provider => new SqlRepository(connectionString));

// --- HATA DÜZELTMESİ BURADA ---
// Bu hatalı satırı kaldırıyoruz:
// builder.Services.AddScoped<DatabaseService>(); 

// Yerine 7 yeni servisi ekliyoruz:
builder.Services.AddScoped<DatabaseSetupService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<StudentService>();
builder.Services.AddScoped<SupervisorService>();
builder.Services.AddScoped<CompanyService>();
builder.Services.AddScoped<InternshipOperationsService>();
builder.Services.AddScoped<StudentDashboardService>();
// --- DÜZELTME BİTTİ ---


builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.SlidingExpiration = true;
    });

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddControllersWithViews();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=KonyaTecnicalUnivercityIMEAutomation}/{id?}");

app.Run();