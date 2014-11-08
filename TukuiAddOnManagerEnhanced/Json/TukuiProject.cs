using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TukuiAddOnManagerEnhanced.Json
{
	public class TukuiProject
	{
		/*
		name (STRING): return the name of the addon.
		author (STRING): return author name of the project.
		url (STRING): return the download url of the addon.
		version (STRING): return the latest version of the addon.
		changelog (STRING): return the url of the changelog on Tukui website
		ticket (STRING): return the url of: report bugs & suggestion
		id (INTEGER): return addon id number.
		git (STRING): return the url address of the git repository.
		updated (STRING): return last time an update was pushed.
		*/

		public string name
		{
			get;
			set;
		}

		public string author
		{
			get;
			set;
		}
		
		public string url
		{
			get;
			set;
		}
		
		public string version
		{
			get;
			set;
		}
		
		public string changelog
		{
			get;
			set;
		}
		
		public string ticket
		{
			get;
			set;
		}
		
		public string id
		{
			get;
			set;
		}
		
		public string git
		{
			get;
			set;
		}
		
		public string updated
		{
			get;
			set;
		}
	}
}
