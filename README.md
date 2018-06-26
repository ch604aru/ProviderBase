# ProviderBase

Solution for multiple projects which loads and handles website requests based on configurations from SQL Server database. This project is intended to use a master database as the entry point and then to direct further requests to the database with data. This allows for a single website to direct and handle different requests based on it's URL to share IIS website space.

## Projects

* ProviderBase.Data
	* Data layer for SQL Server communications using Dapper. Uses Generics or basic object to construct, execute the query and return the requires object. Also handles multiple result sets for complex objects (object within an object).
* ProviderBase.Framework
	* Main website entities for controls, handlers, modules for delivering website content
* ProviderBase.FrameWork.Forum
	* Entities and Handlers not delivering a core purpose. ie satelite systems
* ProviderBase.Web
	* Web layer currently used for utility methods for making web request (API) or Html resource fetching
* ProviderBase.WPF
	* Placeholder for WPF project in the future
* ProviderBaseTest
	* Console application for testing in solution projects
* ProviderBaseWebsite
	* Web form project for website forms and resources (Javascript, Html, Styles etc)
	
## Dependencies

* Dapper
	* Micro ORM for SQL Server query execution and object retrieval (ProviderBase.Data)
* FiftyOne.Foundation
	* Mobile device detection (ProviderBase.Framework)
* Newtonsoft.Json
	* Serialize and Deserialize objects (ProviderBase.Web)

## Notes

Project started July 2017

## License

MIT License - See LICENSE file for more information
