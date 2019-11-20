namespace NetCoreStarter.Utils
{
    public class Privileges
    {
        //Permissions on the User Model
        public const string CanCreateUsers = "CanCreateUsers";
        public const string CanViewUsers = "CanViewUsers";
        public const string CanUpdateUsers = "CanUpdateUsers";
        public const string CanDeleteUsers = "CanDeleteUsers";

        //Permissions on the Role
        public const string CanCreateRoles = "CanCreateRoles";
        public const string CanViewRoles = "CanViewRoles";
        public const string CanUpdateRoles = "CanUpdateRoles";
        public const string CanDeleteRoles = "CanDeleteRoles";

        //permissions on Settings
        public const string CanCreateSettings = "CanCreateSettings";
        public const string CanViewSettings = "CanViewSettings";
        public const string CanUpdateSettings = "CanUpdateSettings";
        public const string CanDeleteSettings = "CanDeleteSettings";


        //permissions on systems configuration
        public const string CanViewDashboard = "CanViewDashboard";
        public const string CanViewReports = "CanViewReports";
        public const string CanViewAdministration = "CanViewAdministration";

    }

    public class GenericProperties
    {
        public const string Administrator = "Administrator";
        public const string Privilege = "Privilege";
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
