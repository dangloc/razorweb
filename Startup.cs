using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Album.Mail;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using razorwebapp.models;

namespace razorwebapp
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();
            var mailsetting = Configuration.GetSection("MailSettings");
            services.Configure<MailSettings>(mailsetting);
            services.AddSingleton<IEmailSender, SendMailService>();

            services.AddRazorPages();
            services.AddDbContext<MyWebContext>(options => {
                string connectionString = Configuration.GetConnectionString("MyWebContext");
                options.UseSqlServer(connectionString);
            });

            // dang ki Identity
             services.AddIdentity<AppUser, IdentityRole>()
             .AddEntityFrameworkStores<MyWebContext>()
             .AddDefaultTokenProviders();


             services.ConfigureApplicationCookie(options => {
                 options.LoginPath="/login/";
                 options.LogoutPath="/logout/";
                 options.AccessDeniedPath="/khongduoctruycap.html";
             });

             services.AddAuthorization(options => {
                options.AddPolicy("AllowEditRole", policyBuilder => {
                        policyBuilder.RequireAuthenticatedUser();
                        // policyBuilder.RequireRole("Admin");
                        // policyBuilder.RequireRole("Editor");
                        policyBuilder.RequireClaim("canedit", "user");
                });
             });

            // services.AddDefaultIdentity<AppUser>()
            // .AddEntityFrameworkStores<MyWebContext>()
            // .AddDefaultTokenProviders();

            // Truy cập IdentityOptions
            services.Configure<IdentityOptions> (options => {
                // Thiết lập về Password
                options.Password.RequireDigit = false; // Không bắt phải có số
                options.Password.RequireLowercase = false; // Không bắt phải có chữ thường
                options.Password.RequireNonAlphanumeric = false; // Không bắt ký tự đặc biệt
                options.Password.RequireUppercase = false; // Không bắt buộc chữ in
                options.Password.RequiredLength = 3; // Số ký tự tối thiểu của password
                options.Password.RequiredUniqueChars = 1; // Số ký tự riêng biệt

                // Cấu hình Lockout - khóa user
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes (5); // Khóa 5 phút
                options.Lockout.MaxFailedAccessAttempts = 5; // Thất bại 5 lầ thì khóa
                options.Lockout.AllowedForNewUsers = true;

                // Cấu hình về User.
                options.User.AllowedUserNameCharacters = // các ký tự đặt tên user
                    "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                options.User.RequireUniqueEmail = true;  // Email là duy nhất

                // Cấu hình đăng nhập.
                options.SignIn.RequireConfirmedEmail = true;            // Cấu hình xác thực địa chỉ email (email phải tồn tại)
                options.SignIn.RequireConfirmedPhoneNumber = false;   // Xác thực số điện thoại
                options.SignIn.RequireConfirmedAccount = true;   

            });
            services.AddAuthentication().AddGoogle(options=> {
                var gconfig = Configuration.GetSection("Authentication:Google");
                options.ClientId = gconfig["ClientId"];
                options.ClientSecret=gconfig["ClientSecret"];
                options.CallbackPath="/dang-nhap-tu-google";
            })
            .AddFacebook(options=> {
                var fconfig = Configuration.GetSection("Authentication:Facebook");
                options.AppId = fconfig["AppId"];
                options.AppSecret = fconfig["AppSecret"];
                options.CallbackPath = "/dang-nhap-tu-fakebook"; 
            })
            // .AddMicrosoftAccount()
            // .AddTwitter()
            ;
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();
            });

        }
    }
}

/* 
Identity:  
    - Authentication: xac thuc danh tinh => login, logout...


    -Authorization: xac thuc quyen truy cap
        - role-based authorization - xac thuc quyen theo vai tro
        -role(vai tro) : (Admin, Editor, vip, Manager, Menber...)

        Areas/Admin/Pages/Role
        index
        create
        edit
        Delete
    dotnet new page -n Index -o Areas/Admin/Pages/Role -na App.Admin.Role
    dotnet new page -n Create -o Areas/Admin/Pages/Role -na App.Admin.Role
    dotnet new page -n EditUserRoleClaim -o Areas/Admin/Pages/User -na App.Admin.User


    -quan li User: Signup, userm, 
Identity/Account/Login
Identity/Account/Manage\

dotnet aspnet-codegenerator identity -dc razorwebapp.models.MyWebContext
*/