namespace EmployeeClient.ViewModels
{
	public class ProfileViewModel
	{
		public UserViewModel ClientInfo { get; set; }
		public List<BillViewModel> BillsClient { get; set; }
	}
}
