namespace Clinic.Data
{
    public static class Shared
    {
        public static readonly Guid BASE_GUID_1 = new("{00000000-0000-0000-0000-000000000001}");
        public static readonly Guid BASE_GUID_2 = new("{00000000-0000-0000-0000-000000000002}");

        public static readonly Guid ROL_SUDO_ID = new("{cbe2971e-4450-4fc4-bd6b-baf660a90dc0}");
        public static readonly Guid ROL_CHANGE_PASSWORD_ID = new("{2cf5e00f-adba-4b1c-bd22-5de7f353eaaf}");
        public static readonly Guid ROL_PATIENT = new("{5eddbc38-93c9-435d-ad78-0ab9aff09e1b}");
        public static readonly Guid ROL_DOCTOR = new("{5d92aa99-2d3b-4b88-860c-1113aee974e3}");
    }

    public class ClientOptions
    {
        public const string Client = "Client";

        public string Type { get; set; } = "Clinic";
        public string Name { get; set; } = "LUX";
        public string Acronym { get; set; } = "LUX";

        public string FullName => $"{Type} {Name}";

        public char[] NameSplit => Name.PadLeft(3, ' ').ToCharArray();
    }
}
