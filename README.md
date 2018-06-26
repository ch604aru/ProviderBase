# ProviderBase

Solution for multiple projects which loads and handles website requests based on configurations from SQL Server

## Projects

*ProviderBase.Data
	* Data layer for SQL Server communications using Dapper.
*ProviderBase.Framework
	* Main website entities for controls, handlers, modules for delivering website content
*ProviderBase.FrameWork.Forum
	* Entities and Handlers not delivering a core purpose. ie satelite systems
*ProviderBase.Web
	* Web layer currently used for utility methods for making web request (API) or Html resource fetching
*ProviderBase.WPF
	* Placeholder for WPF project in the future
*ProviderBaseTest
	* Console application for testing in solution projects
*ProviderBaseWebsite
	* Web form project for website forms and resources (Javascript, Styles, etc)
	
## Dependencies

*Dapper
	* Micro ORM for SQL Server query execution and object retrieval
*FiftyOne.Foundation
	* Mobile device detection
*Newtonsoft.Json
	* Serialize and Deserialize objects

## Notes

Project started July 2017

## License

MIT License - See LICENSE file for more information
