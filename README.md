<img src="http://i61.tinypic.com/ji00hl.jpg" border="0" alt="Image and video hosting by TinyPic">
# LoLUniverse
League of Legends web application utilizing [RiotApi.NET C# library](https://github.com/sdesyllas/RiotApi.NET)
<a href="http://tinypic.com?ref=35d3lox" target="_blank"><img src="http://i59.tinypic.com/35d3lox.jpg" border="0" alt="Image and video hosting by TinyPic"></a>

# Description
This is a web application open source project created with ASP.NET MVC that utilizing [RiotApi.NET C# library](https://github.com/sdesyllas/RiotApi.NET) and provide users with Summaries, Stats, Champion details and other stuff. The project is created with the ASP.NET MVC
framework and it is using advanced mechanics of caching, NoSQL performance solution and user account system.

# Installation
In order to have this web application up and running you need to configure the following services in web.config
* MS Sql Server connection string (for User Management and Asp.Net Identity)
* RavenDB Connection string (for NoSql store of League of Legends entities)
* email smtp settings (for user account emails like two factor authentication)
* Modify the Migrations/Configuration.cs Seed method for initial administrator user creation

# Technologies and Libraries used
* [RiotApi.Net](https://github.com/sdesyllas/RiotApi.NET)
* [RavenDB NoSQL Database](http://ravendb.net/)

<img src="http://ravendb.net/content/images/ravenMoon.png" alt="powered by RavenDB" width="85" height="85"/>
