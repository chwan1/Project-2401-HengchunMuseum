namespace Rlc.Cron
{
	public class MonthsCronEntry : CronEntryBase
	{
		public MonthsCronEntry(string expression)
		{
			Initialize(expression, 1, 12);
		}
	}
}