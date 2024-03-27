
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace Budget.Spendings.Api;

public class GenericApiBuilder : ApiBuilder
{
    public GenericApiBuilder(string[] args) : base(args)
    {
    }

    protected override void AddIdentityProvider()
    {
        Builder.Services.AddAuthentication(options =>
        {
            options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.Authority = Builder.Configuration.GetValue<string>("Api:GenericHost:Idp:Authority");
            options.Audience = Builder.Configuration.GetValue<string>("Api:GenericHost:Idp:Audience");
            options.RequireHttpsMetadata = true;

            if (Builder.Configuration.GetValue<bool>("Api:GenericHost:Idp:SetUsernameFromClaim"))
            {
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context => Task.Run(() =>
                    {
                        var claims = context?.Principal?.Claims.ToList();

                        var nameClaim = context?.Principal?.FindFirst(
                            Builder.Configuration.GetValue<string>("Api:GenericHost:Idp:UsernameClaim") ?? string.Empty
                        );

                        if (nameClaim != null)
                        {
                            var identity = context?.Principal?.Identity as ClaimsIdentity;
                            identity?.AddClaim(new Claim(ClaimTypes.Name, nameClaim.Value));
                        }
                    })
                };
            }

            if (Builder.Configuration.GetValue<bool>("Api:GenericHost:Idp:IgnoreCertificateValidation"))
            {
                options.BackchannelHttpHandler = new HttpClientHandler()
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                };
            }
        });
    }


    public override WebApplication Build()
    {
        AddUserSecrets();
        AddIdentityProvider();
        AddControllers();
        AddSwagger();
        AddRequiredServices();
        AddCors();
        ConfigureKestrel();

        return Builder.Build();
    }
}