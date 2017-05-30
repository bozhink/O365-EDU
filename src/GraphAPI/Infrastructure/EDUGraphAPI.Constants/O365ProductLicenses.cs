namespace EDUGraphAPI.Constants
{
    using System;

    public sealed class O365ProductLicenses
    {
        /// <summary>
        /// Microsoft Classroom Preview
        /// </summary>
        public static readonly Guid Classroom = new Guid("80f12768-d8d9-4e93-99a8-fa2464374d34");

        /// <summary>
        /// Office 365 Education for faculty
        /// </summary>
        public static readonly Guid Faculty = new Guid("94763226-9b3c-4e75-a931-5c89701abe66");

        /// <summary>
        /// Office 365 Education for students
        /// </summary>
        public static readonly Guid Student = new Guid("314c4481-f395-4525-be8b-2ec4bb1e9d91");
        
        /// <summary>
        /// Office 365 Education for faculty
        /// </summary>
        public static readonly Guid FacultyPro = new Guid("78e66a63-337a-4a9a-8959-41c6654dfb56");

        /// <summary>
        /// Office 365 Education for students
        /// </summary>
        public static readonly Guid StudentPro = new Guid("e82ae690-a2d5-4d76-8d30-7c6e01e6022e");
    }
}
