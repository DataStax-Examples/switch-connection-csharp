using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using Cassandra;

namespace switching_connection_configurations_csharp
{
    class Program
    {
        private static void Main(string[] args)
        {
            new Program().MainAsync(args).GetAwaiter().GetResult();
        }

        private async Task MainAsync(string[] args)
        {
            var session = await GetClientConfiguration();
            List<Host> hostSet = session.Cluster.Metadata.AllHosts().ToList();
            String hosts = String.Join(",", hostSet.Select(h => h.Address.ToString()));
            Console.WriteLine($"Connected to cluster with {hostSet.Count()} host(s) {hosts}");

            await session.Cluster.ShutdownAsync();
        }

        /// <summary>
        /// This method shows how you can switch between a Cassandra or an Astra connection at runtime.
        /// In this example we used environment variables but in reality this could be done using configuration files, command line arguments, etc.
        /// Additionally for a production use case we would want to add additional error handling/checking around that connection
        /// process which was omitted here for simplicities sake.
        /// </summary>
        /// <returns></returns>
        private async Task<ISession> GetClientConfiguration()
        {
            Builder builder = null;
            if (!String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("USEASTRA")) &&
                Boolean.Parse(System.Environment.GetEnvironmentVariable("USEASTRA")))
            {
                //Since this is Astra make sure we have all the needed connection information, else throw an error
                if (!String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("DBUSERNAME")) &&
                !String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("DBPASSWORD")) &&
                !String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("SECURECONNECTBUNDLEPATH")) &&
                !String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("KEYSPACE")))
                {
                    builder = Cluster.Builder()
                        .WithCloudSecureConnectionBundle(System.Environment.GetEnvironmentVariable("SECURECONNECTBUNDLEPATH"));
                }
                else
                {
                    throw new ArgumentException("You must have the DBUSERNAME, DBPASSWORD, SECURECONNECTBUNDLEPATH, and KEYSPACE environment variables set to use Astra as your database.");
                }
            }
            else
            {
                //Since this is Cassandra all we must have is the Contact points
                if (!String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("CONTACTPOINTS")))
                {
                    builder = Cluster.Builder().AddContactPoint(System.Environment.GetEnvironmentVariable("CONTACTPOINTS"));
                }
                else
                {
                    throw new ArgumentException("You must have the CONTACTPOINTS environment variable set to use Astra as your database.");
                }
            }

            //Add Credentials - This is required for Astra
            if (!String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("DBUSERNAME")) &&
                !String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("DBPASSWORD")))
            {
                builder.WithCredentials(System.Environment.GetEnvironmentVariable("DBUSERNAME"),
                        System.Environment.GetEnvironmentVariable("DBPASSWORD"));
            }

            //Add Keyspace if it exists - This is required for Astra
            if (!String.IsNullOrEmpty(System.Environment.GetEnvironmentVariable("KEYSPACE")))
            {
                return await builder.Build().ConnectAsync(System.Environment.GetEnvironmentVariable("KEYSPACE"));
            }
            else
            {
                return await builder.Build().ConnectAsync();
            }
        }
    }
}
