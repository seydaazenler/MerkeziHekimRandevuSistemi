﻿using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MHRSLite_DAL;
using MHRSLite_BLL.Contracts;
using MHRSLite_BLL.Imlementations;
using MHRSLite_BLL;
using MHRSLite_BLL.EmailService;
using MHRSLite_EL.IdentityModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using MHRSLite_EL.Enums;
using MHRSLite_EL.Mappings;
using AutoMapper;

namespace MHRSLite_UI
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
            //Aspnet Core'un Connection String bağlantısını yapabilmek için servislerine dbContext klenmesi gerekir
            services.AddDbContext<MyContext>(options =>
            {
                options.UseSqlServer(Configuration.GetConnectionString("SqlConnection"));
                
            });

            //***************Comment*************************
            //Bağımsızlık işlemi
            //IUniteOfWork görüldüğü zaman bana UnitOfWork nesnesi üret!
            services.AddScoped<IUnitOfWork, UnitOfWork>();
           
            //IEmailSender gördüğün zaman bana EmailSender nesnesi üret!
            services.AddScoped<IEmailSender, EmailSender>();
            //****************Comment************************
            services.AddScoped<IClaimsTransformation, ClaimProvider.ClaimProvider>();
            services.AddAutoMapper(typeof(Maps));
;            services.AddAuthorization(opts =>
            {
                opts.AddPolicy("GenderPolicy", policy =>
                policy.RequireClaim("gender", Genders.Bayan.ToString())
                );
            });
            //Çalışırken Razor sayfasında yapılan değişikliklerin sayfaya yenileyerek yansıması için kullanılır
            services.AddControllersWithViews(x => x.SuppressAsyncSuffixInActionNames = false).AddRazorRuntimeCompilation();
            services.AddRazorPages();
            services.AddMvc();
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromSeconds(60);
            });

            ////Google api'den alýnan clientId ve clientSecret burada projeye dahil edildi.
            //services.AddAuthentication()
            //    .AddGoogle(options =>
            //    {
            //        options.ClientId = Configuration["Authentication:Google:ClientId"];
            //        options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];
            //    });

            //****************************************
            //Şifre kuralları ve kontrolü! Canlıya çıkarsak bunlar true yada default olmalı
            services.AddIdentity<AppUser, AppRole>(
                opts =>
                {
                    opts.User.RequireUniqueEmail = true;
                    opts.Password.RequiredLength = 6;
                    opts.Password.RequireNonAlphanumeric = false;
                    opts.Password.RequireLowercase = false;
                    opts.Password.RequireUppercase = false;
                    opts.Password.RequireDigit = false;
                    opts.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789 -._@+";
                }).AddDefaultTokenProviders().AddEntityFrameworkStores<MyContext>();
            //****************************************

        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, UserManager<AppUser> userManager, RoleManager<AppRole> roleManager, IUnitOfWork unitOfWork)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }
            app.UseStaticFiles();

            app.UseRouting(); //rooting mekanizması için

            app.UseSession(); //session oturum mekanizması için

            app.UseAuthentication(); //login logout kullanabilmek için

            app.UseAuthorization(); //authorization attiribute kullanabilmek için

            CreateDefaultData.CreateData.Create(userManager,roleManager,unitOfWork,Configuration,env);
            
            app.UseEndpoints(endpoints =>
                {
                    endpoints.MapControllerRoute(
                        name: "default",
                        pattern: "{controller=Home}/{action=Index}/{id?}");
                    endpoints.MapAreaControllerRoute(
                        "management",
                        "management",
                        "management/{controller=Admin}/{action=Index}/{id?}"
                        );
                });
        }
    }
}
