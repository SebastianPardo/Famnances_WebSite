using Famnances.DataCore.Entities;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Famnances.Models.ViewModels
{
    public class SearchUser
    {
        public SearchUser(Guid? homeId, HomeInvitation invitation)
        {
            HomeId = homeId ?? Guid.Empty;
            Guests = new List<Guest> { new Guest(invitation) };
        }

        public SearchUser(Guid? homeId, List<HomeInvitation> invitations)
        {
            HomeId = homeId ?? Guid.Empty;
            Guests = invitations.Select(e => new Guest(e)).ToList();
        }

        public Guid HomeId { get; set; }
        public Guest Guest { get; set; }
        public List<Guest> Guests { get; set; }

    }

    public class Guest
    {
        public Guest() { }
        public Guest(HomeInvitation invitation)
        {
            UserId = invitation.GuestId.Value;
            Email = invitation.Guest.Email;
            Name = invitation.Guest.User.LegalName;
            InvitationId = invitation.Id;
        }

        public Guid UserId { get; set; }
        public Guid InvitationId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
