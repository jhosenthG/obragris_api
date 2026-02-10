namespace obragris_api.core;

public class User : BaseEntity
{
    public string Username { get; private set; } = string.Empty;
    public string Email { get; private set; } = string.Empty;
    public string PasswordHash { get; private set; } = string.Empty;
    public UserRole Role { get; private set; }
    
    //---Relacion---//
    public virtual ICollection<Project> Projects { get; private set; } = new List<Project>();

    //---Constructor Entity---//
    private User()
    {
    }
    
    //---Creador de usuarios---//
    public User(string username, string email, string passwordHash, UserRole role = UserRole.User)
    {
        Id = Guid.NewGuid();
        Username = username;
        Email = email;
        PasswordHash = passwordHash;
        Role = role;
        
        CreatedAt = DateTime.UtcNow;
        IsDeleted = false;
    }
    
    //---Metodos de negocio---//
    public void Update(string username, string email)
    {
        if (string.IsNullOrWhiteSpace(username)) throw new ArgumentNullException(nameof(username));
        if (string.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
    }

    public void ChangeRole(UserRole newRole)
    {
        Role = newRole;
    }
    public void SetNewPassword(string newPasswordHash)
    {
        if (string.IsNullOrWhiteSpace(newPasswordHash)) throw new ArgumentNullException(nameof(newPasswordHash));
        PasswordHash = newPasswordHash;
    }
    
}