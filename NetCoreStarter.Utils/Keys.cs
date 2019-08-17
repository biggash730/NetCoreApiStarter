using System;
using System.Collections.Generic;
using System.Text;

namespace NetCoreStarter.Utils
{
    public class Privileges
    {
        //Permissions on the User Model
        public const string CanManageUsers = "CanManageUsers";
        public const string CanViewUsers = "CanViewUsers";

        //Permissions on the Role
        public const string CanManageRoles = "CanManageRoles";
        public const string CanViewRoles = "CanViewRoles";

        //permissions on systems configuration
        public const string CanViewDashboard = "CanViewDashboard";
        public const string CanViewReports = "CanViewReports";
        public const string CanViewSetting = "CanViewSetting";

    }

    public class GenericProperties
    {
        public const string CreatedBy = "CreatedBy";
        public const string CreatedAt = "CreatedAt";
        public const string ModifiedAt = "ModifiedAt";
        public const string ModifiedBy = "ModifiedBy";
        public const string Locked = "Locked";
        public const string IsDeleted = "IsDeleted";
    }

    public class ExceptionMessage
    {
        public const string RecordLocked = "Record is locked and can't be deleted.";
        public const string NotFound = "Record not found.";
    }
}
