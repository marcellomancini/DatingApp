using System.ComponentModel.DataAnnotations;

namespace DatingApp.API.Models
{
    public class Credentials
    {
        private string _userName;

        [Required]
        public string UserName
        {
            get { return _userName.ToLower(); }
            set { _userName = value; }
        }
        
        [Required]
        [StringLength(16,MinimumLength=8,ErrorMessage="You must specify password between 8 and 16 characters")]
        public string Password { get; set; }
    }
}