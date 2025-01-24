namespace PassXYZ.Vault.Services;

public interface IUserService<T>
{
	T GetUser(string username);
	Task AddUserAsync(T user);
	Task DeleteUserAsync(T user);
	List<string> GetUsersList();
	Task<bool> LoginAsync(T user);
	void Logout();
}