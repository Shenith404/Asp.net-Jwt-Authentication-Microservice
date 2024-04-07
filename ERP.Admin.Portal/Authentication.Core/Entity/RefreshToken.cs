using ERP.Authentication.Core.Entity;
using System.ComponentModel.DataAnnotations.Schema;


namespace Authentication.Core.Entity
{
    public class RefreshToken
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime AddedDate { get; set; } = DateTime.UtcNow;
        public DateTime UpdateDate { get; set; } = DateTime.UtcNow;
        public int Status { get; set; }

        public string Token { get; set; }

        public string JwtId { get; set; }    //the id when jwt id has been requested

        public bool IsUsed { get; set; }     // to make sure that the token is only used once 

        public bool IsRevoked { get; set; }  // make sure the are valid

        public DateTime ExpiredDate { get; set; }

        public string  UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public UserModel User { get; set; }
    }
}
