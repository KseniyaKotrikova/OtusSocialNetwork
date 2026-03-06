namespace OtusSocialNetwork.DTO;

public record UserRegisterRequest(string first_name, string second_name, DateTime birthdate, string biography, string city, string password, string gender);