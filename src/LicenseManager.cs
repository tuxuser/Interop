
// Implementation: MicrosoftXboxSecurityClip.exe via RegisterServiceCtrlHandlerW("MicrosoftXboxSecurityClip")
using System;

[Guid("2769c3a8-d8e3-41ba-b38b-01d05dd2374e")] // In memory: a8c36927e3d8ba41b38b01d05dd2374e
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IClipLicenseManager
{
    static readonly Guid CLSID = new Guid("7e480b22-b679-4542-9b30-d5a52da92ce5"); // In memory: 220b487e79b642459b30d5a52da92ce5

    [PreserveSig]
    int StoreLicense(string licenseBuffer, LoadLicenseType loadType, out ILicense license);

    // Deactivates the license on the PSP
    // Deletes license clip file from disk
    // Updates clip index
    [PreserveSig]
    int PurgeLicense(ILicense license);

    [PreserveSig]
    int QueryOnLicenseId(string licenseId, ClipQueryFlags queryFlags, out ILicense license);

    // Returns an array of ILicense objects
    // NOTE: Passing 0 in for the keyUid returns all keys
    [PreserveSig]
    int QueryOnKeyId(string keyUid, ClipQueryFlags queryFlags, out nint licenseArray, out int licenseCount);

    // Looks to be a query api that returns an array
    [PreserveSig]
    int Unknown38();

    // Called by XboxLicenseManagerExt.dll
    [PreserveSig]
    int ActivateLicense(ILicense license);

    [PreserveSig]
    int DeactivateLicense(ILicense license);

    // Hard coded to always return 5, may be supportedLicenseVersion?
    [PreserveSig]
    int Unknown50(out int alwaysFive);

    // Shuts down the ICLipLicenseManager (at least the event provider)
    [PreserveSig]
    int Destroy();
    
    public enum LoadLicenseType : uint {
        DEFERRED_LOAD = 0x0,
        IMMEDIATE_LOAD = 0x1
    }

    public enum ClipQueryFlags : int {
        ALL_LICENSES = 0,
        ROOT_LICENSES = 0x2
    }
}

[Guid("AC46F627-6F8A-4C04-B352-AD2CE8A89E7C")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ILicense {

    [PreserveSig]
    int GetKeyId(out string keyId);

    [PreserveSig]
    int GetLicenseId(out string licenseId);

    [PreserveSig]
    int GetExpirationDate(out uint expirationDate);

    [PreserveSig]
    int GetBeginDate(out uint beginDate);

    [PreserveSig]
    int GetIssueDate(out uint issueDate);

    // AKA "UplinkKeyId"
    [PreserveSig]
    int GetRootKeyId(out string rootKeyId);

    [PreserveSig]
    int GetSubscriptionId(out string unknownGuid);

    [PreserveSig]
    int GetLicenseXml(out string someString);

    [PreserveSig]
    int GetFlags(out LicenseFlags licenseFlags);

    [PreserveSig]
    int GetPolicy(out string someString);

    [PreserveSig]
    int GetDiscId(out string discId);

    [PreserveSig]
    int Destroy();

    [Flags]
    public enum LicenseFlags : int {
        // May mean that the license exists?
        UNKNOWN1 = 0x1,

        // When set, the ILicense has a Subsription Id (BindingToIdentity)

        HAS_SUBSCRIPTION_ID = 0x2,
        // When set, indicates that the key has been loaded into the PSP
        KEY_LOADED = 0x4,

        // Whether the "Persist" field is set in the XML.
        // This determines whether the license is persisted (eg S:\Clips directory).
        // It is used to determine whether a license should be deleted from the clip folder or not.
        PERSIST = 0x10,

        // When set, the license does not have a root key because it is a root key (aka "UplinkKeyId").
        // When calling the PSP the KeyId will be used as root key if one is not available
        ROOT_KEY = 0x20,

        // Is used to target licenses when purging "offline root key material" (combined with SUBSCRIPTION_ROOT check)
        UNKNOWN30 = 0x30,
    }
}

public class LicenseManager : IDisposable {
    private readonly IClipLicenseManager manager;

    public LicenseManager() {
        COM.CoInitializeEx(IntPtr.Zero, 0);
        this.manager = COM.ActivateClass<IClipLicenseManager>(IClipLicenseManager.CLSID);
    }

    public void Dispose() {
        Marshal.ReleaseComObject(this.manager);
    }

    /// <summary>
    /// Loads a full license (base64 representation of the license.xml)
    /// </summary>
    /// <param name="licenseBuffer"></param>
    /// <returns>true on success, false otherwise</returns>
    public bool LoadLicenseBase64(string licenseBuffer) {
        this.manager.StoreLicense(licenseBuffer, IClipLicenseManager.LoadLicenseType.IMMEDIATE_LOAD, out ILicense license);
        return license != null;
    }

    public bool LoadLicenseFile(string filePath) {
        //
        // Read and convert license contents to base64
        // Just assume the license is valid, it'll fail anyway if it's not
        //
        string content = File.ReadAllText(filePath);
        if (string.IsNullOrEmpty(content)) {
            throw new InvalidDataException("Provided file is empty");
        }

        var licenseBlob = Encoding.UTF8.GetBytes(content);
        var base64Blob = Convert.ToBase64String(licenseBlob);
        return this.LoadLicenseBase64(base64Blob);
    }

    public void PrintLicenses() {

        int result = this.manager.QueryOnKeyId(null, IClipLicenseManager.ClipQueryFlags.ALL_LICENSES, out nint licenseArr, out int licenseCount);
        for (int i = 0; i < licenseCount; i++) {
            nint pLicense = Marshal.ReadIntPtr(licenseArr + i * IntPtr.Size);
            var license = ((ILicense)Marshal.GetTypedObjectForIUnknown(pLicense, typeof(ILicense)));
            PrintLicense(license);
            Console.WriteLine("---------------------------------------------");
        }

    }
    public void PrintLicense(ILicense license) {
        int result = 0;

        result = license.GetLicenseId(out string licenseId);
        Console.WriteLine($"licenseId = {licenseId}");

        result = license.GetKeyId(out string keyId);
        Console.WriteLine($"keyId = {keyId}");

        result = license.GetFlags(out ILicense.LicenseFlags licenseFlags);
        Console.WriteLine($"licenseFlags = 0x{licenseFlags:X}");

        foreach (ILicense.LicenseFlags flagVal in Enum.GetValuesAsUnderlyingType(typeof(ILicense.LicenseFlags))) {
            if ((flagVal & licenseFlags) == flagVal) {
                Console.WriteLine($"\t{Enum.GetName(typeof(ILicense.LicenseFlags), flagVal)}");
            }
        }

        if ((licenseFlags & ILicense.LicenseFlags.ROOT_KEY) == 0) {
            result = license.GetRootKeyId(out string rootKeyId);
            Console.WriteLine($"rootKeyId = {rootKeyId}");
        }

        if ((licenseFlags & ILicense.LicenseFlags.HAS_SUBSCRIPTION_ID) > 0) {
            result = license.GetSubscriptionId(out string subscriptionId);
            Console.WriteLine($"subscriptionId = {subscriptionId}");
        }

        result = license.GetDiscId(out string discId);
        Console.WriteLine($"discId = {discId}");

        result = license.GetIssueDate(out uint issueDate);
        Console.WriteLine($"issueDate = {issueDate}");

        result = license.GetBeginDate(out uint beginDate);
        Console.WriteLine($"beginDate = {beginDate}");

        result = license.GetExpirationDate(out uint expirationDate);
        Console.WriteLine($"expirationDate = {expirationDate}");

        result = license.GetLicenseXml(out string licenseXml);
        Console.WriteLine($"licenseXml = {licenseXml}");

        result = license.GetPolicy(out string policy);
        Console.WriteLine($"policy = {policy}");
    }

}