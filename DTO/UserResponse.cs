namespace OtusSocialNetwork.DTO;

public record UserResponse(string id, string first_name, string second_name, DateTime birthdate, string biography, string city, string gender);