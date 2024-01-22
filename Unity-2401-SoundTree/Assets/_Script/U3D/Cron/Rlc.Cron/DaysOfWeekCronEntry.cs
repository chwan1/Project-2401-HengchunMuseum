namespace Rlc.Cron
{
	public class DaysOfWeekCronEntry : CronEntryBase
	{
		public DaysOfWeekCronEntry(string expression)
		{
			expression = expression.Replace ("7", "0");
			Initialize(expression, 0, 6);
		}
	}
}