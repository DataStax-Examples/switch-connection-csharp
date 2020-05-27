# Switch Connections between Apache Cassandra™ and Astra databases
This application shows how to use the [C# DataStax Driver](https://docs.datastax.com/en/developer/csharp-driver/latest) to connect to an on-prem Cassandra database or a Astra database in the cloud at runtime using environment variables.

Contributor(s): [Dave Bechberger](https://github.com/bechbd)

## Objectives
* Shows the differences between a Cassandra connection configuration and an Astra connection configuration in a single app.
* Provide a demonstration of how to configure the database connection at runtime.
* See the [documentation](https://docs.datastax.com/en/developer/csharp-driver/latest/features/cloud/) for more details about the Astra connection configuration for the C# Driver

## Project Layout
* [Program.cs](/Program.cs) - The main application file which contains the logic to switch between the configurations

## How this Sample Works
This sample uses environment variables to specify the configuration parameters and whether to use a Cassandra (DSE/DDAC/C*) configuration or an Astra configuration.  
All the logic to switch between the configurations occurs in the [`GetClientConfiguration`](https://github.com/DataStax-Examples/switch-connection-csharp/blob/master/Program.cs#L33) method.  
* If you specify the `USEASTRA` environment variable and it is `true`:
    * The environment variables are checked to see that `DBUSERNAME`, `DBPASSWORD`, `SECURECONNECTBUNDLEPATH`, and `KEYSPACE` exist
		* If they exist then the Astra connection is created
		* If they do not exist then an error is thrown
* If you so not specify the `USEASTRA` environment variable or it is `false`:
	* The environment variables are checked to see that `CONTACTPOINTS` and `DATACENTER` exist
		* If they exist then the standard connection is created
		* If they do not exist then an error is thrown
		* If `DBUSERNAME` and `DBPASSWORD` exist then credentials are added to the configuration
		* If `KEYSPACE` exists then it is added to the configuration

While these criteria added to the configuration are commonly used configuration parameters you are able to specify any additional ones in the code. 

Additionally while this example uses environment variables to control which configuration is selected this could also be done via the use of configuration files or command line parameters.

For clarity this sample does not contain any of the normal error handling process you would want to wrap around connecting to a cluster to handle likely errors that would occur.

## Setup and Running

### Prerequisites
* .NET Core 2.1
* A Cassandra cluster or an Astra database to connect to with the appropriate connection information

### Running

To connect to an Astra database you first need to download the secure connect bundle following the instructions found [here](https://docs.datastax.com/en/astra/aws/doc/dscloud/astra/dscloudObtainingCredentials.html).
This first step in the process is to build the application.  This can be done using the following command:

`dotnet build`

Once you have compiled the connection information and built the application you can run this to connect to Apollo by using the command below, with the appropriate configuration added:

`USEASTRA=true DBUSERNAME=XXX DBPASSWORD=XXX KEYSPACE=XXX SECURECONNECTBUNDLEPATH="/valid/path/to/secureconnectbundle.zip" dotnet run`

If you would like to connect to a Cassandra cluster use the command below, with the appropriate configuration added:

`CONTACTPOINTS=XX.XX.XX.XX dotnet run`

Once you run this against either option you will get output that specifies the number of hosts and their IP addresses printed to the console:

```Connected to cluster with 3 host(s) XX.XX.XX.XX:9042,XX.XX.XX.XX:9042,XX.XX.XX.XX:9042```
