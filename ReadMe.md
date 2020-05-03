# AzureAD protected WebAPI being called by console application

Below sample shows steps required in azure portal 

# Register Protected WebAPI


1. Login to Azure - AAD
2. New App Registration
3. AppName - `WeatherAPI_Development` | No redirect endpoint
4. Expose an API - Create Application Id Uri
5. Application Manifest - Add the below snippet

```
"appRoles": [
    {
        "allowedMemberTypes": [
            "Application"
        ],
        "description": "Daemon apps in this role can consume the web api.",
        "displayName": "DaemonAppRole",
        "id": "6543b78e-0f43-4fe9-bf84-0ce8b74c06a3",
        "isEnabled": true,
        "lang": null,
        "origin": "Application",
        "value": "DaemonAppRole"
    }
],
```

6. Grab the following values from Overview page -

    | WeatherAPI_Development  |
    | ----------------------- |
    | Application (client) ID |
    | Directory (tenant) ID   |
    | Application ID URI      |

7. Code changes 

* Add following values `appSettings.json` 

        "AAD": {
            "ResourceId": "api://{app-resource-id}",
            "InstanceId": "https://login.microsoftonline.com/",
            "TenantId": "{tenant-id}"
        }

* Add following in `Startup.cs`

        services
            .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.Audience = Configuration["AAD:ResourceId"];
                options.Authority = $"{Configuration["AAD:InstanceId"]}{Configuration["AAD:TenantId"]}";

            });

        // Configure
        app.UseAuthentication();

* Decorate `Controllers` with `[Authorize]` attribute
-----

# Register Protected WebAPI Client


1. Login to Azure - AAD
2. New App Registration
3. AppName - `WeatherAPIClient_Development` | No redirect endpoint
4. Certificate & Secret - New Client Secret
5. API Permissions - Add a permission - My API - `WeatherAPI_Development` - Select `DaemonAppRole`

6. Grab the following values from Overview page -

    | WeatherAPIClient_Development  |
    | ----------------------- |
    | Application (client) ID |
    | Directory (tenant) ID   |
    | Application ID URI      |

7. Code changes 

* Create and add following values `appSettings.json` 

```
{
    "Instance": "https://login.microsoftonline.com/{0}",
    "TenantId": "{tenant-id",
    "ClientId": "{client/application-id}",
    "ClientSecret": "{application-secret}",
    "BaseAddress": "https://localhost:5001/weatherforecast",
    "ResourceId": "api://{resource-id}/.default" 
}
```
