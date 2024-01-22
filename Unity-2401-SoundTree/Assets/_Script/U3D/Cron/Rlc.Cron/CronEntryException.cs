using System;

namespace Rlc.Cron
{
	public class CronEntryException : Exception
	{
		public CronEntryException(string message)
			: base(message)
		{

		}
	}
}