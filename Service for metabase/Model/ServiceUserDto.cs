using System.ComponentModel.DataAnnotations;

namespace Service_for_metabase.Model;

public class ServiceUserDto
{
	[Required(ErrorMessage = "Не указано имя пользователя")]
	public string Username { get; set; }
	
	[Required(ErrorMessage = "Не указан пароль")]
	[DataType(DataType.Password)]
	public string Password { get; set; }
}