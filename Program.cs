using thomasmack.dev.Backend;

namespace thomasmack.dev
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddRouting(options =>
            {
                options.LowercaseUrls = true;
                options.LowercaseQueryStrings = true;
            });
            builder.Services.AddRazorPages();

            builder.Services.Configure<SmtpSettings>(
                builder.Configuration.GetSection("SmtpSettings")
            );

            builder.Services.AddTransient<IEmailService, EmailService>();
            builder.Services.AddMemoryCache();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseStatusCodePagesWithReExecute("/Error");
            app.UseAuthorization();

            app.MapGet("/index", () => Results.Redirect("/", permanent: true));

            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.MapFallbackToPage("/Error");

            app.Run();
        }
    }


}
