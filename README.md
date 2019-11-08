# Switching Connections between Cassandra or Apollo in C#
This application shows how to use configure a Java application to connect to Cassandra or an Apollo database at runtime using environment variables.

Contributors: [Dave Bechberger](https://github.com/bechbd) 

## Objectives
* To demonstrate how to specify at runtime between a Cassandra (DSE/DDAC/C*) client configuration and an Apollo configuration for the same application.

## Project Layout
* [Program.cs](/Program.cs) - The main application file which contains all the logic to switch between the configurations

## How this Sample Works
This sample uses environment variables to specify the configuration parameters and whether to use a Cassandra (DSE/DDAC/C*) configuration or an Apollo configuration.  
All the logic to switch between the configurations occurs in the `getClientConfiguration` method.  
* If you specify the `USEAPOLLO` environment variable and it is `true`:
    * The environment variables are checked to see that `DBUSERNAME`, `DBPASSWORD`, `SECURECONNECTBUNDLEPATH`, and `KEYSPACE` exist
		* If they exist then the Apollo connection is created
		* If they do not exist then an error is thrown
* If you so not specify the `USEAPOLLO` environment variable or it is `false`:
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
* A Cassandra cluster or an Apollo database to connect to with the appropriate connection information

### Running

To connect to an Apollo database you first need to download the secure connect bundle following the instructions found [here](https://docs.datastax.com/en/landing_page/doc/landing_page/cloud.html).
This first step in the process is to build the application.  This can be done using the following command:

`dotnet build`

Once you have compiled the connection information and built the application you can run this to connect to Apollo by using the command below, with the appropriate configuration added:

`USEAPOLLO=true DBUSERNAME=XXX DBPASSWORD=XXX KEYSPACE=XXX SECURECONNECTBUNDLEPATH="/valid/path/to/secureconnectbundle.zip" dotnet run`

If you would like to connect to a Cassandra cluster use the command below, with the appropriate configuration added:

`CONTACTPOINTS=XX.XX.XX.XX dotnet run`

Once you run this against either option you will get output that specifies the number of hosts and their IP addresses printed to the console:

```Connected to cluster with 3 host(s) XX.XX.XX.XX:9042,XX.XX.XX.XX:9042,XX.XX.XX.XX:9042```
