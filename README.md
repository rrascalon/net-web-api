Documentation will come soon
----------------------------

Added Net.Web.Api.Sdk.Web.Examples project
------------------------------------------
    - Create A JWT Token API Service
    - Validate a JWT Token Service
    - Revoke a JWT Token Service (Using LiteDB, future version will feature provider to different DBs or Cache Services) 
    - Upload File Service
    

Startup steps
-------------

1- Create an ASP.NET Web Application (.NET Framework) with framework version 4.8

2- Choose Empty application

3- Remove the nuget package 'Microsoft.CodeDom.Providers.DotNetCompilerPlatform' if present

4- Add the reference to the Net.Web.Api.Sdk library or add it from Nuget

5- Add a Global Application Class in the ASP.NET Web Application 

6- Add the following methods in:

    - Application_Start
    
        GlobalConfiguration.Configuration.RegisterWebApi();
        
    - Application_End
    
        GlobalConfiguration.Configuration.UnRegisterWebApi();
        
7- Set the startup page ASP.NET Web Application to swagger/ui/index




