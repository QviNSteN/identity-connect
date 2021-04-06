using System;
using System.Collections.Generic;
using System.Text;
using identity_connect.Data.Enums;
using System.Linq;

namespace identity_connect.Models.Information
{
    public class UserInfo
    {
        public int UserId { get; set; }

        public string Name { get; set; }

        public string Surname { get; set; }

        public string Patronymic { get; set; }

        public GenderEnum? Gender { get; set; }

        public DateTime? Birthday { get; set; }

        public IList<IntegrationElement> Integrations { get; set; }

        public IList<PermissionModel> Permissions { get; set; }

        public string SimpleFio { get; set; }

        public string Fio { get; set; }
    }

    public class SimpleUserInfo
    {
        public int UserId { get; set; }
    }
}
