using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyCampusV2.DAL.Context;
using MyCampusV2.DAL.IRepositories;
using MyCampusV2.DAL.Repositories;
using MyCampusV2.DAL.UnitOfWorks;
using MyCampusV2.IServices;
using MyCampusV2.Services;
using Newtonsoft.Json;
using MyCampusV2.Services.IServices;
using MyCampusV2.Services.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MyCampusV2.Common;
using System;
using Newtonsoft.Json.Serialization;
using Microsoft.Extensions.Logging;
using MyCampusV2.CustomHelpers.ExceptionHelper;
using MyCampusV2.Helpers.Image;
using GlobalExceptionHandler.WebApi;
using MyCampusV2.Helpers.Encryption;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

namespace MyCampusV2
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            //string test = AppDomain.CurrentDomain.BaseDirectory + "Firebase Key\\firekey.json";
            //FirebaseApp.Create(new AppOptions()
            //{
            //    Credential = GoogleCredential.FromFile(test),
            //});
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials()
                    .WithExposedHeaders("Authorization")
                    );
            });

            services.AddDbContext<MyCampusCardContext>(options =>
            options
             //.UseLazyLoadingProxies()
             .UseMySql(Encryption.Decrypt(Configuration.GetConnectionString("connectionString"))));

            services.AddDbContext<MyCampusCardReportContext>(options =>
                options
                 //.UseLazyLoadingProxies()
                 .UseMySql(Encryption.Decrypt(Configuration.GetConnectionString("slaveConnectionString"))));
            //.UseMySql(Configuration.GetConnectionString("connectionString")));

            services.AddAutoMapper();

            // configure strongly typed settings objects
            var appSettingsSection = Configuration.GetSection("AppSettings");
            services.Configure<AppSettings>(appSettingsSection);

            var batchUploadSettings = Configuration.GetSection("BatchUploadSettings");
            services.Configure<BatchUploadSettings>(batchUploadSettings);

            var emailSettings = Configuration.GetSection("EmailSettings");
            services.Configure<EmailSettings>(emailSettings);

            var fileSettings = Configuration.GetSection("FileSettings");
            services.Configure<FileSettings>(fileSettings);

            // get active directory settings
            var adSettings = Configuration.GetSection("ActiveDirectory").Get<AdSettings>();
            AdSettingsStat.Domain = adSettings.Domain;
            AdSettingsStat.Url = adSettings.Url;

            // get photo path settings
            var photopathSettings = Configuration.GetSection("PhotoLocation").Get<PhotoSettings>();
            PhotoStatSettings.photopath = photopathSettings.loc;
            PhotoStatSettings.dumppath = photopathSettings.dumpdestination;

            // configure jwt authentication
            var appSettings = appSettingsSection.Get<AppSettings>();
            var key = Encoding.ASCII.GetBytes(appSettings.Secret);
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
          .AddJwtBearer(options =>
          {
              options.TokenValidationParameters = new TokenValidationParameters
              {
                  ValidateIssuer = true,
                  ValidateAudience = true,
                  ValidateLifetime = true,
                  ValidateIssuerSigningKey = true,
                  ValidIssuer = appSettings.Issuer,
                  ValidAudience = appSettings.Audience,
                  IssuerSigningKey = new SymmetricSecurityKey(key)
              };
          });

            services.AddHttpContextAccessor();
            /*
            * Repository DI 
            */
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<ICampusRepository, CampusRepository>();
            services.AddScoped<ICollegeRepository, CollegeRepository>();
            services.AddScoped<ICourseRepository, CourseRepository>();
            services.AddScoped<IDepartmentRepository, DepartmentRepository>();
            services.AddScoped<IEducationLevelRepository, EducationLevelRepository>();
            //services.AddScoped<IFailedLogsRepository, FailedLogsRepository>();
            services.AddScoped<IPersonRepository, PersonRepository>();
            services.AddScoped<IPositionRepository, PositionRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IUserRoleRepository, UserRoleRepository>();
            services.AddScoped<IFormRepository, FormRepository>();
            services.AddScoped<IVisitorRepository, VisitorRepository>();
            services.AddScoped<IAreaRepository, AreaRepository>();
            services.AddScoped<IAuditTrailRepository, AuditTrailRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();
            services.AddScoped<ITerminalRepository, TerminalRepository>();
            services.AddScoped<IReportRepository, ReportRepository>();
            services.AddScoped<INotificationRepository, NotificationRepository>();
            services.AddScoped<ITerminalConfigurationRepository, TerminalConfigurationRepository>();

            services.AddScoped<ICardDetailsRepository, CardDetailsRepository>();
            services.AddScoped<ITerminalWhiteListRepository, TerminalWhiteListRepository>();
            services.AddScoped<IDataSyncRepository, DataSyncRepository>();
            services.AddScoped<IBatchUploadRepository, BatchUploadRepository>();

            services.AddScoped<IScheduleRepository, ScheduleRepository>();
            services.AddScoped<IEmployeeTypeRepository, EmployeeTypeRepository>();
            services.AddScoped<IEmployeeSubTypeRepository, EmployeeSubTypeRepository>();

            services.AddScoped<IOfficeService, OfficeService>();
            services.AddScoped<IYearSectionService, YearSectionService>();
            services.AddScoped<IStudentSectionService, StudentSectionService>();
            services.AddScoped<ISchoolCalendarService, SchoolCalendarService>();

            services.AddScoped<IGroupService, GroupService>();
            services.AddScoped<IGroupDetailsService, GroupDetailsService>();

            services.AddScoped<IEmergencyLogoutService, EmergencyLogoutService>();

            services.AddScoped<IPAPAccountService, PAPAccountService>();
            services.AddScoped<IPAPAccountLinkedStudentsService, PAPAccountLinkedStudentsService>();

            services.AddScoped<IAnnouncementsService, AnnouncementsService>();
            services.AddScoped<IAnnouncementsRecipientsService, AnnouncementsRecipientsService>();

            services.AddScoped<ICalendarService, CalendarService>();
            services.AddScoped<ICalendarRecipientsService, CalendarRecipientsService>();

            services.AddScoped<IDigitalReferencesService, DigitalReferencesService>();

            /*
           * Services DI 
           */
            services.AddScoped<ICampusService, CampusService>();
            services.AddScoped<ICollegeService, CollegeService>();
            services.AddScoped<ICourseService, CourseService>();
            services.AddScoped<IDepartmentService, DepartmentService>();
            services.AddScoped<IEducationLevelService, EducationLevelService>();
            services.AddScoped<IFailedLogsService, FailedLogService>();
            services.AddScoped<IPersonService, PersonService>();
            services.AddScoped<IPositionService, PositionService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IUserRoleService, UserRoleService>();
            services.AddScoped<IFormService, FormService>();
            //services.AddScoped<IVisitorService, VisitorService>();
            services.AddScoped<IAreaService, AreaService>();
            services.AddScoped<IAuditTrailService, AuditTrailService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<ITerminalService, TerminalService>();
            services.AddScoped<IReportService, ReportService>();
            services.AddScoped<INotificationService, NotificationService>();
            services.AddScoped<ITerminalConfigurationService, TerminalConfigurationService>();
            services.AddScoped<ICurrentUser, CurrentWebUserService>();
            services.AddTransient<IImageHandler, ImageHandler>();
            services.AddTransient<IImageWriter, ImageWriter>();

            services.AddScoped<ICardDetailsService, CardDetailsService>();
            services.AddScoped<ITerminalWhiteListService, TerminalWhiteListService>();
            services.AddScoped<IDataSyncService, DataSyncService>();
            services.AddScoped<IBatchUploadService, BatchUploadService>();

            services.AddScoped<IScheduleService, ScheduleService>();
            services.AddScoped<IEmployeeTypeService, EmployeeTypeService>();
            services.AddScoped<IEmployeeSubTypeService, EmployeeSubTypeService>();

            services.AddScoped<IOfficeRepository, OfficeRepository>();
            services.AddScoped<IYearSectionRepository, YearSectionRepository>();
            services.AddScoped<IStudentSectionRepository, StudentSectionRepository>();

            services.AddScoped<IGroupRepository, GroupRepository>();
            services.AddScoped<IGroupDetailsRepository, GroupDetailsRepository>();

            services.AddScoped<IEmergencyLogoutRepository, EmergencyLogoutRepository>();

            services.AddScoped<IPAPAccountRepository, PAPAccountRepository>();
            services.AddScoped<IPAPAccountLinkedStudentsRepository, PAPAccountLinkedStudentsRepository>();

            services.AddScoped<IAnnouncementsRepository, AnnouncementsRepository>();
            services.AddScoped<IAnnouncementsRecipientsRepository, AnnouncementsRecipientsRepository>();

            services.AddScoped<ICalendarRepository, CalendarRepository>();
            services.AddScoped<ICalendarRecipientsRepository, CalendarRecipientsRepository>();

            services.AddScoped<IDigitalReferencesRepository, DigitalReferencesRepository>();


            services.AddMvcCore().AddAuthorization().AddJsonFormatters(options => options.ContractResolver = new CamelCasePropertyNamesContractResolver()).
            SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options => {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                }).AddDataAnnotations();
        }
        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseCors("CorsPolicy");
            app.UseAuthentication();
            app.UseMvc();

            app.UseStaticFiles();
            app.UseHttpsRedirection();

           /* app.UseGlobalExceptionHandler(x => {
                x.ContentType = "application/json";
                x.ResponseBody(s => JsonConvert.SerializeObject(new
                {
                    Message = "An error occurred whilst processing your request"
                }));
            });

            app.Map("/error", x => x.Run(y => throw new Exception()));*/
            //loggerFactory.AddFile("Logs/MyCampus-{Date}.txt");



            /*app.UseMvc(routes =>
            {
                routes.MapRoute("default", "{controller=Home}/{action=Index}/{id?}");
            });*/

        }
    }
}
