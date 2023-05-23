namespace EmployeeClient.ViewModels
{
	public class CreditViewModel
	{
		public long Id { get; set; }
		public long BillId { get; set; }
		public long InitialSum { get; set; }
		public long RemainingSum { get; set; }
		public long Duration { get; set; }
		public long Dept { get; set; }
		public TariffInfoModel TariffInfoModel { get; set;}
		public List<LatePaymentModel> LatePaymentModels { get; set; }

	}
	public class TariffInfoModel
	{
		public long Id { get; set; }
		public string Name { get; set; }
		public long Percentage { get; set; }
	}

	public class LatePaymentModel
	{
		public long Amount { get; set; }
		public string Date { get; set; }
	}
}
