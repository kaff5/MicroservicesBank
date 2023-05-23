using System.Runtime.InteropServices;

namespace EmployeeClient.ViewModels
{
	public class BillViewModel
	{
		public long Id { get; set; }
		public long Balance { get; set; }
		public BillType Type { get; set; }
		public StatusBill Status { get; set; }
		public string CreateAt { get; set; }
	}

	public enum BillType
	{
		DEBIT,
		CREDIT
	}

	public enum StatusBill
	{
		OPENED,
		CLOSED
	}
}
