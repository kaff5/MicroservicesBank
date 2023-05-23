using CoreClient;

namespace EmployeeClient.ViewModels
{
	public class OperationViewModel
	{
		public string Id { get; set; }
		public long Amount { get; set; }
		public string PerfomedAt { get; set; }
        public OperationStatus Status { get; set; }
        public long FromBillId { get; set; }
        public long ToBillId { get; set; }
	}
}
