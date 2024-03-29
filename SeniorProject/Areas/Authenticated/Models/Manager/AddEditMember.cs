﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeniorProject.Areas.Authenticated.Models.Manager
{
    public class AddEditMember
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int MemberID { get; set; }

        public string MemberRole { get; set; }

        public bool ActiveStatus { get; set; }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string State { get; set; }



        public string UserName { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }




    }
}
