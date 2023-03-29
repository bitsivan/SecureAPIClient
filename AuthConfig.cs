using System.Globalization;
using System.Collections;
using System.IO;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace SecureAPIClient
{
    public class AuthConfig
    {
        public string Instance { get; set; }="https://login.microsoftonline.com/{0}";
        public string TenantId { get; set; }
        public string ClientId { get; set; }
        public string Authority {
            get{
                return String.Format(CultureInfo.InvariantCulture, Instance, TenantId);
            }
        }
        public string ClientSecret { get; set; }
        public string BaseAddress { get; set; }
        public string ResourceId { get; set; }

        public static AuthConfig ReadFromJsonFile(string path)
        {
            IConfiguration configuration;
            var builder= new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile(path);

            configuration=builder.Build();
            return configuration.Get<AuthConfig>();
        }

    }
}
